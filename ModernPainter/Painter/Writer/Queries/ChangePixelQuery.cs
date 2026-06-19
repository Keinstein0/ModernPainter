using ModernPainter.Painter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Writer.Queries
{
    internal class ChangePixelQuery : IChangePixelQuery
    {
        private Vector2D _point; // virtual point
        private Color _color;

        public ChangePixelQuery(Vector2D pt, Color color)
        {
            _point = pt;
            _color = color;
        }
        
        public void RunDefault(IWriter writer)
        {
            writer.ChangePixel(_point, _color);
        }
    }

}
