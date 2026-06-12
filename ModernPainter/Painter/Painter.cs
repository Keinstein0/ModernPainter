using ModernPainter.Painter.Data;
using ModernPainter.Painter.Writer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter
{
    public class Painter
    {
        private IWriter _writer;

        public Rectangle2D ScreenBounds
        {
            get
            {
                return _writer.GetSize();
            }
        }

        public Painter()
        {

        }
    }
}
