using ModernPainter.Painter.Data;
using ModernPainter.Painter.Writer;
using ModernPainter.Painter.Writer.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter
{
    internal class ModernPainter
    {
        private IWriter _writer;

        public ModernPainter(IWriter output)
        {
            _writer = output;
        }

        public void ChangePixel(Point2D point, Color color)
        {
            IChangePixelQuery query = new ChangePixelQuery(point, color);
            _writer.RunQuery(query);
        }

        public void Clear()
        {

        }


        public void Update()
        {
            _writer.RenderFrame();
        }
    }
}
