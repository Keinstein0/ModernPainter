using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Data
{
    internal class PhysicalColor
    {
        private int _red;
        private int _green;
        private int _blue;

        public PhysicalColor(int r, int g, int b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        public PhysicalColor()
        {
            Red = 0;
            Green = 0;
            Blue = 0;
        }

        public PhysicalColor(string hexCode)
        {
            if (string.IsNullOrWhiteSpace(hexCode))
                throw new ArgumentException("Hex code cannot be empty.");

            string cleanHex = hexCode.StartsWith("#") ? hexCode.Substring(1) : hexCode;

            if (cleanHex.Length != 6)
                throw new ArgumentException("Hex code must be 6 characters long (excluding optional #).");

            Red = int.Parse(cleanHex.Substring(0, 2), NumberStyles.HexNumber);
            Green = int.Parse(cleanHex.Substring(2, 2), NumberStyles.HexNumber);
            Blue = int.Parse(cleanHex.Substring(4, 2), NumberStyles.HexNumber);
        }

        public void MergeColor(Color color)
        {
            float alpha = color.Alpha / 255f;
            float reverseAlpha = 1f - alpha;

            Red = (int)((color.Red * alpha) + (Red * reverseAlpha));
            Green = (int)((color.Green * alpha) + (Green * reverseAlpha));
            Blue = (int)((color.Blue * alpha) + (Blue * reverseAlpha));
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

        public string Hex
        {
            get
            {
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
