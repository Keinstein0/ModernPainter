using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModernPainter.Console.Reader.MousePositioning
{
    public class LinuxMouseDriver : IMouseDriver
    {
        
        private static readonly Regex SgrMouseRegex = new Regex(@"\x1b\[\<(\d+);(\d+);(\d+)([Mm])", RegexOptions.Compiled);
        private static readonly Regex CellSizeRegex = new Regex(@"\x1b\[6;(\d+);(\d+)t", RegexOptions.Compiled);

        private int _fontWidthPx = 8;   // Fallback
        private int _fontHeightPx = 16; // Fallback

        private readonly Stream _stdin = System.Console.OpenStandardInput();
        private readonly byte[] _readBuffer = new byte[1024];
        private readonly StringBuilder _textBuffer = new StringBuilder();

        private (int charX, int virtualY, bool isPressed)? _currentState;
        private readonly StringBuilder _buffer = new StringBuilder();

        public void Initialize()
        {
            
            try
            {
                System.Diagnostics.Process.Start("stty", "-echo raw -icanon min 0 time 0").WaitForExit();
            }
            catch { }
            
            // 1000: enable mouse tracking, 1006: SGR mode, 1016: SGR-Pixel mode
            System.Console.Write("\x1b[?1000h\x1b[?1006h\x1b[?1016h");
            // Ask terminal for character cell dimensions in pixels
            System.Console.Write("\x1b[16t");
        }

        public void Update()
        {
            // 1. Read directly from standard input stream without Console.ReadKey stripping \x1b
            while (System.Console.KeyAvailable || _stdin.Length > 0)
            {
                int bytesRead = _stdin.Read(_readBuffer, 0, _readBuffer.Length);
                if (bytesRead <= 0) break;

                _textBuffer.Append(Encoding.ASCII.GetString(_readBuffer, 0, bytesRead));
            }

            if (_textBuffer.Length == 0) return;

            string input = _textBuffer.ToString();
            ParseRawBuffer(input);

            // Keep buffer empty
            _textBuffer.Clear();
        }

        public (int charX, int virtualY, bool isPressed)? GetState() => _currentState;

        public void Shutdown()
        {
            System.Console.Write("\x1b[?1000l\x1b[?1006l\x1b[?1016l");
            try
            {
                System.Diagnostics.Process.Start("stty", "sane").WaitForExit();
            }
            catch { }
        }


        private void ParseRawBuffer(string input)
        {
            // Check font metrics response (\x1b[6;H;Wt)
            int sizeIdx = input.IndexOf("\x1b[6;");
            if (sizeIdx != -1)
            {
                int endIdx = input.IndexOf('t', sizeIdx);
                if (endIdx != -1)
                {
                    string[] parts = input.Substring(sizeIdx + 4, endIdx - (sizeIdx + 4)).Split(';');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int h) && int.TryParse(parts[1], out int w))
                    {
                        if (h > 0 && w > 0)
                        {
                            _fontHeightPx = h;
                            _fontWidthPx = w;
                        }
                    }
                }
            }

            // Process mouse input: Now the sequence correctly starts with \x1b[<
            int mouseIdx = input.LastIndexOf("\x1b[<");
            if (mouseIdx != -1)
            {
                int endIdx = -1;
                for (int i = mouseIdx; i < input.Length; i++)
                {
                    if (input[i] == 'M' || input[i] == 'm')
                    {
                        endIdx = i;
                        break;
                    }
                }

                if (endIdx != -1)
                {
                    char action = input[endIdx]; // 'M' = click/move, 'm' = release
                    string payload = input.Substring(mouseIdx + 3, endIdx - (mouseIdx + 3));
                    string[] parts = payload.Split(';');

                    if (parts.Length == 3 &&
                        int.TryParse(parts[0], out int button) &&
                        int.TryParse(parts[1], out int rawX) &&
                        int.TryParse(parts[2], out int rawY))
                    {
                        rawX -= 1; // 1-based ANSI to 0-based
                        rawY -= 1;

                        int charX;
                        int virtualY;

                        // Notice your raw values in image: (103, 27) vs (149, 37)
                        // If coordinates exceed standard column count, terminal is operating in 1016 pixel mode
                        if (rawY > System.Console.WindowHeight * 2)
                        {
                            charX = rawX / _fontWidthPx;
                            int halfCellHeight = _fontHeightPx / 2;
                            virtualY = rawY / halfCellHeight;
                        }
                        else
                        {
                            // 1006 Character Mode Fallback
                            charX = rawX;
                            virtualY = rawY * 2;
                        }

                        bool isPressed = (action == 'M') && (button == 0 || button == 32);
                        _currentState = (charX, virtualY, isPressed);
                    }
                }
            }
        }
    }
}