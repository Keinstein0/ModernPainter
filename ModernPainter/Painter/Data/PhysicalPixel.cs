using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Data
{
    internal class PhysicalPixel
    {
        public char Character { get; set; } = PIXEL;

        public const char PIXEL = '▄';
        public PhysicalColor ForegroundColor { get; set; }
        public PhysicalColor BackgroundColor {  get; set; }

    }
}
