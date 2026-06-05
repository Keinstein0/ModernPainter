using ModernPainter.Painter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Writer
{
    internal interface IWriter
    {
        public void ChangePixel(Point2D point, PhysicalColor color);
        public PhysicalColor GetPixel(Point2D point);

        public Rectangle2D GetSize();
        public void RenderFrame();




    }
}
