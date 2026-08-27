using System;
using System.IO;
using System.Text;
using System.Threading;

namespace ModernPainter.Console.Reader.MousePositioning
{
    public class LinuxMouseDriver : IMouseDriver
    {
        private readonly Stream _stdin = System.Console.OpenStandardInput();
        private readonly byte[] _readBuffer = new byte[4096];
        private readonly StringBuilder _textBuffer = new StringBuilder();
        private readonly object _bufferLock = new object();

        private (int charX, int virtualY, bool isPressed)? _currentState;
        private Thread _readerThread;
        private CancellationTokenSource _cts;
        private bool _disposed;

        public void Initialize()
        {
            try
            {
                System.Diagnostics.Process.Start("stty", "-echo raw -icanon min 0 time 0").WaitForExit();
            }
            catch { }

            // 1000: press/release tracking, 1002: button-event, 1003: any-event (motion), 1006: SGR mode
            System.Console.Write("\x1b[?1000h\x1b[?1002h\x1b[?1003h\x1b[?1006h");

            _cts = new CancellationTokenSource();
            _readerThread = new Thread(ReadLoop) { IsBackground = true, Name = "LinuxMouseDriver" };
            _readerThread.Start(_cts.Token);
        }

        public void Update()
        {
            string snapshot;
            lock (_bufferLock)
            {
                snapshot = _textBuffer.ToString();
                _textBuffer.Clear();
            }

            if (snapshot.Length == 0) return;
            ParseBuffer(snapshot);
        }

        public (int charX, int virtualY, bool isPressed)? GetState() => _currentState;

        public void Shutdown()
        {
            _cts?.Cancel();
            _readerThread?.Join(500);
            _cts?.Dispose();

            System.Console.Write("\x1b[?1000l\x1b[?1002l\x1b[?1003l\x1b[?1006l");
            try
            {
                System.Diagnostics.Process.Start("stty", "sane").WaitForExit();
            }
            catch { }

            _disposed = true;
        }

        private void ReadLoop(object? tokenObj)
        {
            var ct = (CancellationToken)tokenObj!;
            try
            {
                while (!ct.IsCancellationRequested && !_disposed)
                {
                    int bytesRead = _stdin.Read(_readBuffer, 0, _readBuffer.Length);
                    if (bytesRead <= 0)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    string chunk = Encoding.ASCII.GetString(_readBuffer, 0, bytesRead);
                    lock (_bufferLock)
                    {
                        _textBuffer.Append(chunk);
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (IOException) { }
        }

        private void ParseBuffer(string content)
        {
            while (content.Length > 0)
            {
                int escIdx = content.IndexOf('\x1b');

                if (escIdx == -1)
                {
                    break;
                }

                if (escIdx > 0)
                {
                    content = content.Substring(escIdx);
                    continue;
                }

                if (content.Length < 2)
                {
                    break;
                }

                if (content[1] == '[')
                {
                    int terminatorIdx = -1;
                    for (int i = 2; i < content.Length; i++)
                    {
                        char c = content[i];
                        if (c >= 0x40 && c <= 0x7E)
                        {
                            terminatorIdx = i;
                            break;
                        }
                    }

                    if (terminatorIdx == -1)
                    {
                        break;
                    }

                    string sequence = content.Substring(0, terminatorIdx + 1);
                    ProcessSingleSequence(sequence);
                    content = content.Substring(terminatorIdx + 1);
                }
                else
                {
                    content = content.Substring(Math.Min(2, content.Length));
                }
            }
        }

        private void ProcessSingleSequence(string sequence)
        {
            // Mouse input SGR (1006): \x1b[<B;X;YM or \x1b[<B;X;ym
            // With 1003 enabled, motion events are also reported in this format.
            if (sequence.StartsWith("\x1b[<") && (sequence.EndsWith("M") || sequence.EndsWith("m")))
            {
                char action = sequence[sequence.Length - 1]; // 'M' = press/drag, 'm' = release
                string payload = sequence.Substring(3, sequence.Length - 4); // strip "\x1b[<" and terminator
                string[] parts = payload.Split(';');

                if (parts.Length == 3 &&
                    int.TryParse(parts[0], out int button) &&
                    int.TryParse(parts[1], out int rawX) &&
                    int.TryParse(parts[2], out int rawY))
                {
                    // 1006 SGR mode reports 1-based character coordinates; convert to 0-based.
                    rawX -= 1;
                    rawY -= 1;

                    // 1006 character mode: rawX = character column, rawY = character row.
                    // Our virtual coordinate system uses 2 virtual pixel rows per character row.
                    int charX = rawX;
                    int virtualY = rawY * 2;

                    bool isPressed = (action == 'M') && (button == 0 || button == 32);
                    _currentState = (charX, virtualY, isPressed);
                }
            }
        }
    }
}
