using ModernPainter.Painter.Data;
using ModernPainter.Painter.Writer.ConsoleWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Writer.Queries
{
    internal class ClearMatrixQuery : IChangePixelQuery
    {
        Color _color;
        
        public ClearMatrixQuery(Color? color)
        {
            if (color == null)
            {
                _color = new Color("#000000");
            }
            else
            {
                _color = color;
            }
        }
        
        
        public void RunDefault(IWriter writer)
        {
            var size = writer.GetSize();

            for (int x = 0; x < size.XSize; x++)
            {
                for (int y = 0; y < size.YSize; y++)
                {
                    writer.ChangePixel(new Point2D(x, y), _color, PhysicalPixel.PIXEL);
                }
            }
        }

        [QueryFor(typeof(ConsoleWriter.ConsoleWriter))]
        public void RunConsole(ColorMatrix matrix)
        {
            PhysicalPixel p = new PhysicalPixel();
            p.BackgroundColor = new PhysicalColor(_color);
            p.ForegroundColor = new PhysicalColor(_color);
            
            Span<PhysicalPixel> rowSpan = matrix.PhysicalPixels.AsSpan(0, matrix.PhysicalPixels.Length-10);
            rowSpan.Fill(p);
        }
    }
}

