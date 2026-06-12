using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Data
{
    internal struct PhysicalPixel
    {
        public PhysicalPixel()
        {

        }
        
        public const char PIXEL = '▄';

        public char Character = PIXEL;

        public PhysicalColor ForegroundColor = new PhysicalColor();

        public PhysicalColor BackgroundColor = new PhysicalColor();
    }
}
