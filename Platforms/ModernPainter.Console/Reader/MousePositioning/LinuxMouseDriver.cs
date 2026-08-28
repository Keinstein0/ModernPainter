using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ModernPainter.Console.Reader.MousePositioning
{
    public class LinuxMouseDriver : IMouseDriver
    {
        private Stream _tty;
        private readonly byte[] _readBuffer = new byte[4096];
        private readonly List<byte> _buffer = new List<byte>();
        private readonly object _bufferLock = new object();
        private int _totalBytesReceived;
        private DateTime _lastHeartbeat = DateTime.MinValue;

        private (int charX, int virtualY, bool isPressed)? _currentState;
        private Thread _readerThread;
        private CancellationTokenSource _cts;
        private bool _disposed;
        private int _cellHeightPx = -1;
        private int _cellWidthPx = -1;

        public void Initialize()
        {
            // Open the controlling terminal explicitly so stty and our reads
            // operate on the same device the user sees. Console.OpenStandardInput()
            // can resolve to a different fd when running under IDE terminals or tmux.
            _tty = System.IO.File.Open("/dev/tty", System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);

            bool sttyOk = false;
            try
            {
                // -F /dev/tty explicitly targets the controlling terminal so stty
                // modifies the same device the user sees, regardless of whether
                // our process's stdin is a different fd (e.g. under VS Code, tmux).
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "stty",
                    Arguments = "-F /dev/tty -echo raw -icanon min 0 time 0",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using var proc = System.Diagnostics.Process.Start(psi);
                proc.WaitForExit();
                sttyOk = proc.ExitCode == 0;
            }
            catch { }

            // Fingerprint the terminal environment and which device we're bound to.
            var diag = new System.Text.StringBuilder();
            diag.AppendLine($"[LinuxMouseDriver] stty raw mode: {(sttyOk ? "OK" : "FAILED")}");
            diag.AppendLine($"[LinuxMouseDriver] stdin is TTY: {System.Console.IsInputRedirected == false}");
            diag.AppendLine($"[LinuxMouseDriver] stdout is TTY: {System.Console.IsOutputRedirected == false}");
            diag.AppendLine($"[LinuxMouseDriver] TERM: {Environment.GetEnvironmentVariable("TERM")}");
            diag.AppendLine($"[LinuxMouseDriver] tty device: /dev/tty");
            System.IO.File.WriteAllText("/tmp/modernpainter_mouse_debug.log", diag.ToString());

            // 1000: press/release tracking, 1002: button-event, 1003: any-event (motion),
            // 1006: SGR mode, 1016: SGR-Pixels (reports position in device pixels).
            // Write config through the tty stream and flush immediately so the terminal
            // receives it before any render output.
            byte[] config = Encoding.ASCII.GetBytes("\x1b[?1000h\x1b[?1002h\x1b[?1003h\x1b[?1006h\x1b[?1016h");
            _tty.Write(config, 0, config.Length);
            _tty.Flush();

            // Request character-cell size in device pixels (CSI 16 t → CSI 6 ; H ; W t)
            // so pixel-mode (1016) coordinates can be mapped back to the 2-virtual-pixel-per-cell grid.
            byte[] cellSizeQuery = Encoding.ASCII.GetBytes("\x1b[16t");
            _tty.Write(cellSizeQuery, 0, cellSizeQuery.Length);
            _tty.Flush();

            _cts = new CancellationTokenSource();
            _readerThread = new Thread(ReadLoop) { IsBackground = true, Name = "LinuxMouseDriver" };
            _readerThread.Start(_cts.Token);
        }

        public void Update()
        {
            lock (_bufferLock)
            {
                if (_buffer.Count == 0) return;
                ParseBuffer(_buffer);
            }
        }

        public (int charX, int virtualY, bool isPressed)? GetState() => _currentState;

        public void Shutdown()
        {
            _cts?.Cancel();
            _readerThread?.Join(500);
            _cts?.Dispose();

            byte[] disable = Encoding.ASCII.GetBytes("\x1b[?1000l\x1b[?1002l\x1b[?1003l\x1b[?1006l\x1b[?1016l");
            _tty.Write(disable, 0, disable.Length);
            _tty.Flush();

            try
            {
                System.Diagnostics.Process.Start("stty", "-F /dev/tty sane").WaitForExit();
            }
            catch { }

            _tty.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Background thread that continuously drains /dev/tty in raw mode.
        /// Blocking reads are fine here — the thread just waits for bytes and appends them.
        /// A heartbeat is logged every 2 s so we can distinguish "no bytes arriving"
        /// from "reader thread died."
        /// </summary>
        private void ReadLoop(object? tokenObj)
        {
            var ct = (CancellationToken)tokenObj!;
            try
            {
                while (!ct.IsCancellationRequested && !_disposed)
                {
                    int bytesRead = _tty.Read(_readBuffer, 0, _readBuffer.Length);
                    if (bytesRead <= 0)
                    {
                        Thread.Sleep(10);
                        // Heartbeat: log every 2 s even when no bytes arrive,
                        // so we can prove the reader thread is alive vs dead.
                        if ((DateTime.Now - _lastHeartbeat).TotalSeconds >= 2)
                        {
                            _lastHeartbeat = DateTime.Now;
                            System.IO.File.AppendAllText("/tmp/modernpainter_mouse_debug.log",
                                $"[LinuxMouseDriver] heartbeat: alive, totalBytes={_totalBytesReceived}, buffer={_buffer.Count}\n");
                        }
                        continue;
                    }

                    lock (_bufferLock)
                    {
                        _buffer.AddRange(new ArraySegment<byte>(_readBuffer, 0, bytesRead));
                    }
                    _totalBytesReceived += bytesRead;
                    if (_totalBytesReceived == 1)
                    {
                        System.IO.File.AppendAllText("/tmp/modernpainter_mouse_debug.log",
                            "[LinuxMouseDriver] FIRST byte received!\n");
                    }
                    if (_totalBytesReceived % 100 == 0)
                    {
                        var hex = BitConverter.ToString(_readBuffer, 0, bytesRead);
                        System.IO.File.AppendAllText("/tmp/modernpainter_mouse_debug.log",
                            $"[LinuxMouseDriver] {_totalBytesReceived} bytes received (last chunk: {hex})\n");
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (IOException) { }
        }

        /// <summary>
        /// Parses mouse escape sequences from the front of the buffer, consuming
        /// complete events and leaving any incomplete tail in place for the next call.
        ///
        /// Supports three protocols:
        ///   • X10 (mode 1000): ESC [ M &lt;button+32&gt; &lt;col+32&gt; &lt;row+32&gt;
        ///   • SGR  (mode 1006): ESC [ &lt;B;X;YM  or  ESC [ &lt;B;X;ym
        ///   • SGR-Pixels (mode 1016): same as SGR but X/Y are device pixels;
        ///     cell size is discovered via CSI 16 t (CSI 6 ; H ; W t).
        /// </summary>
        private void ParseBuffer(List<byte> buffer)
        {
            int i = 0;
            int parsedEvents = 0;

            while (i < buffer.Count)
            {
                // Must start with ESC (0x1B)
                if (buffer[i] != 0x1B) { i++; continue; }

                // Need at least ESC [ (2 more bytes)
                if (i + 1 >= buffer.Count) break;

                if (buffer[i + 1] == '[') // CSI
                {
                    // ── X10 protocol (mode 1000): ESC [ M + 3 raw bytes ──
                    if (i + 2 < buffer.Count && buffer[i + 2] == 'M') // 0x4D
                    {
                        // ESC [ M b x y  —  b=button+32, x=col+32, y=row+32
                        if (i + 5 < buffer.Count) // need 3 bytes after the M
                        {
                            int button = buffer[i + 3] - 32;
                            int col     = buffer[i + 4] - 32;
                            int row     = buffer[i + 5] - 32;

                            int charX = col;
                            int virtualY = row * 2;
                            // button & 32 set → release; button 0-3 → press;
                            // button == 32 is motion (no button) — treat as pressed,
                            // matching the SGR convention where motion (button==32)
                            // keeps the fill rectangle active.
                            bool isPressed = (button == 32) || ((button & 32) == 0);

                            _currentState = (charX, virtualY, isPressed);
                            parsedEvents++;
                            i += 6; // consume ESC [ M + 3 bytes
                            continue;
                        }

                        // Incomplete X10 event — preserve the tail for next tick.
                        break;
                    }

                    // ── SGR protocol (mode 1006): ESC [ &lt;B;X;YM / ESC [ &lt;B;X;ym ──
                    if (i + 2 < buffer.Count && buffer[i + 2] == '<') // 0x3C
                    {
                        // Find the terminator M (0x4D) or m (0x6D)
                        int terminatorIdx = -1;
                        for (int j = i + 3; j < buffer.Count; j++)
                        {
                            if (buffer[j] == 0x4D || buffer[j] == 0x6D) // M or m
                            {
                                terminatorIdx = j;
                                break;
                            }
                        }

                        if (terminatorIdx == -1)
                        {
                            // Incomplete SGR sequence — preserve the tail.
                            break;
                        }

                        // Payload is the text between '&lt;' and the terminator.
                        int payloadLen = terminatorIdx - (i + 3);
                        string payload = Encoding.ASCII.GetString(
                            buffer.ToArray(), i + 3, payloadLen);
                        char action = (char)buffer[terminatorIdx]; // 'M' or 'm'

                        string[] parts = payload.Split(';');
                        if (parts.Length == 3 &&
                            int.TryParse(parts[0], out int button) &&
                            int.TryParse(parts[1], out int rawX) &&
                            int.TryParse(parts[2], out int rawY))
                        {
                            // Terminal coordinates are 1-based; convert to 0-based.
                            int zeroBasedX = rawX - 1;
                            int zeroBasedY = rawY - 1;

                            int charX;
                            int virtualY;
                            if (_cellHeightPx > 0 && _cellWidthPx > 0)
                            {
                                // 1016 (SGR-Pixels) active: zeroBasedX/Y are device pixels.
                                int cellRow = zeroBasedY / _cellHeightPx;
                                int offsetInCell = zeroBasedY % _cellHeightPx;
                                int half = _cellHeightPx / 2;
                                // top half of a cell → even virtualY (Background), bottom half → odd (Foreground),
                                // matching the 2-virtual-pixels-per-cell convention used by ConsoleWriter.
                                virtualY = cellRow * 2 + (offsetInCell >= half ? 1 : 0);
                                charX = zeroBasedX / _cellWidthPx;
                            }
                            else
                            {
                                // Cell mode (1006): zeroBasedX/Y are character cells.
                                charX = zeroBasedX;
                                virtualY = zeroBasedY * 2;
                            }

                            // 'M' = press or drag; 'm' = release.
                            // button 0 = left press, button 32 = motion (no button).
                            bool isPressed = (action == 'M') && (button == 0 || button == 32);

                            _currentState = (charX, virtualY, isPressed);
                            parsedEvents++;
                        }

                        i = terminatorIdx + 1; // consume everything up to and including terminator
                        continue;
                    }

                    // ── Dimension reply (CSI 16 t → CSI 6 ; H ; W t) ──
                    if (i + 2 < buffer.Count && buffer[i + 2] == 0x36) // '6'
                    {
                        // Parse CSI 6 ; cellHeight ; cellWidth t
                        int termT = -1;
                        for (int j = i + 3; j < buffer.Count; j++)
                        {
                            if (buffer[j] == 0x74) // 't'
                            {
                                termT = j;
                                break;
                            }
                        }
                        if (termT == -1) break;
                        string payload = Encoding.ASCII.GetString(
                            buffer.ToArray(), i + 2, termT - (i + 2));
                        string[] parts = payload.Split(';');
                        if (parts.Length == 3 && parts[0] == "6" &&
                            int.TryParse(parts[1], out int h) &&
                            int.TryParse(parts[2], out int w) &&
                            h > 0 && w > 0)
                        {
                            _cellHeightPx = h;
                            _cellWidthPx = w;
                        }
                        i = termT + 1;
                        continue;
                    }

                    // ── Other CSI sequence — skip to terminator or end ──
                    int termIdx = -1;
                    for (int j = i + 2; j < buffer.Count; j++)
                    {
                        if (buffer[j] >= 0x40 && buffer[j] <= 0x7E)
                        {
                            termIdx = j;
                            break;
                        }
                    }

                    if (termIdx == -1)
                    {
                        // Incomplete CSI — preserve the tail.
                        break;
                    }

                    i = termIdx + 1; // skip the whole unrecognized CSI
                    continue;
                }
                else
                {
                    // Non-CSI escape (e.g. Alt+key). Discard the leading ESC + one more byte.
                    int skip = Math.Min(2, buffer.Count - i);
                    i += skip;
                    continue;
                }
            }

            // Remove all bytes up to the current parse position in one shot.
            // Anything from position i onward (incomplete tail) stays in the buffer.
            if (i > 0)
            {
                buffer.RemoveRange(0, i);
            }

            if (parsedEvents > 0)
            {
                var s = _currentState.Value;
                System.IO.File.AppendAllText("/tmp/modernpainter_mouse_debug.log",
                    $"[LinuxMouseDriver] parsed {parsedEvents} event(s), state=({s.charX},{s.virtualY},pressed={s.isPressed})\n");
            }
        }
    }
}
