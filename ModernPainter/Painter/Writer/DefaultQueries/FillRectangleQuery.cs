using ModernPainter.Core.Painter.Data;
using ModernPainter.Core.Painter.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter.Writer.DefaultQueries
{
    public class FillRectangleQuery : IChangePixelQuery
    {
        protected Rectangle2D _rectangle;
        protected Color _color;
        
        public FillRectangleQuery(Rectangle2D rectangle, Color color)
        {
            _rectangle = rectangle;
            _color = color;
        }

        public FillRectangleQuery(FillRectangleQuery f)
        {
            _rectangle = f._rectangle;
            _color = f._color;
        }
        
        
        public void RunDefault(IWriter writer)
        {
            for (int x = _rectangle.X; x < _rectangle.XMax; x++)
            {
                for (int y = _rectangle.Y; y < _rectangle.YMax; y++)
                {
                    writer.ChangePixel(new Vector2D(x, y), _color);
                }
            }
        }
    }
}
