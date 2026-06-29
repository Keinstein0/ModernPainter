using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter.Data
{
    public struct PhysicalPixel
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
