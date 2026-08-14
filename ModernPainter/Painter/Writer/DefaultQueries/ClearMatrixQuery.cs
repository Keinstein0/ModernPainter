using ModernPainter.Core.Painter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter.Writer.DefaultQueries
{
    public class ClearMatrixQuery : IChangePixelQuery
    {
        protected Color _color;
        
        public ClearMatrixQuery(Color? color)
        {
            if (color == null)
            {
                _color = new Color("#000000");
            }
            else
            {
                _color = color.Value;
            }
        }

        public ClearMatrixQuery(ClearMatrixQuery q)
        {
            _color = q._color;
        }
        
        
        public void RunDefault(IWriter writer)
        {
            var size = writer.GetSize();

            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    writer.ChangePixel(new Vector2D(x, y), _color, PhysicalPixel.PIXEL);
                }
            }
        }
    }
}

