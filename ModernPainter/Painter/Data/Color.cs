using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Data
{
    internal class Color
    {
        private int _red;
        private int _green;
        private int _blue;
        private int _alpha;

        public Color(int r, int g, int b, int alpha = 255)
        {
            Red = r;
            Green = g;
            Blue = b;
            Alpha = alpha;
        }

        public Color(string hexCode)
        {
            if (string.IsNullOrWhiteSpace(hexCode))
                throw new ArgumentException("Hex code cannot be empty.");

            string cleanHex = hexCode.StartsWith("#") ? hexCode.Substring(1) : hexCode;

            if (cleanHex.Length != 6 && cleanHex.Length != 8)
                throw new ArgumentException("Hex code must be 6/8 characters long (excluding optional #).");

            Red = int.Parse(cleanHex.Substring(0, 2), NumberStyles.HexNumber);
            Green = int.Parse(cleanHex.Substring(2, 2), NumberStyles.HexNumber);
            Blue = int.Parse(cleanHex.Substring(4, 2), NumberStyles.HexNumber);
            Alpha = int.Parse(cleanHex.Substring(6,2), NumberStyles.HexNumber);
        }

        public int Red
        {
            get => _red;
            set => _red = Clamp(value);
        }

        public int Green
        {
            get => _green;
            set => _green = Clamp(value);
        }

        public int Blue
        {
            get => _blue;
            set => _blue = Clamp(value);
        }

        public int Alpha
        {
            get => _blue;
            set => _blue = Clamp(value);
        }

        public string Hex
        {
            get
            {
                string alpha = Alpha == 255 ? "" : $"{Alpha:X2}"; // TODO may be the wrong way around

                return $"#{Red:X2}{Green:X2}{Blue:X2}";
            }
        }

        private int Clamp(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return value;
        }
    }
}
