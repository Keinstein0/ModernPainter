using ModernPainter.Painter.Data;
using ModernPainter.Painter.Writer.ConsoleWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Writer.Queries
{
    internal class FillRectangleQuery : IChangePixelQuery
    {
        Rectangle2D _rectangle;
        Color _color;
        
        public FillRectangleQuery(Rectangle2D rectangle, Color color)
        {
            _rectangle = rectangle;
            _color = color;
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

        public bool FillRectangle(ColorMatrix matrix)
        {
            if (_color.Alpha != 255)
            {
                return false;
            }
            
            
            int virtualHeight = matrix.YSize * 2;

            int startX = Math.Max(0, _rectangle.X);
            int startY = Math.Max(0, _rectangle.Y);
            int endX = Math.Min(matrix.XSize, _rectangle.XMax);
            int endY = Math.Min(virtualHeight, _rectangle.YMax);

            if (startX >= endX || startY >= endY) return false;

            int drawWidth = endX - startX;
            Span<PhysicalPixel> matrixSpan = matrix.PhysicalPixels.AsSpan();
            PhysicalColor targetColor = new PhysicalColor(_color);

            int startPy = startY / 2;
            int endPy = (endY - 1) / 2; // Inclusive upper physical bound

            for (int pY = startPy; pY <= endPy; pY++)
            {
                int rowStartIndex = pY * matrix.XSize;

                int virtualTop = pY * 2;
                int virtualBottom = virtualTop + 1;

                bool fillTop = (virtualTop >= startY && virtualTop < endY);
                bool fillBottom = (virtualBottom >= startY && virtualBottom < endY);

                if (fillTop && fillBottom)
                {
                    PhysicalPixel solidPixel = new PhysicalPixel
                    {
                        ForegroundColor = targetColor,
                        BackgroundColor = targetColor,
                        Character = '▀' 
                    };

                    matrixSpan.Slice(rowStartIndex + startX, drawWidth).Fill(solidPixel);
                }
                else // edge rows
                {
                    Span<PhysicalPixel> rowSlice = matrixSpan.Slice(rowStartIndex + startX, drawWidth);

                    for (int i = 0; i < rowSlice.Length; i++)
                    {
                        ref PhysicalPixel pixel = ref rowSlice[i];
                        if (fillTop) pixel.ForegroundColor = targetColor;
                        if (fillBottom) pixel.BackgroundColor = targetColor;
                    }
                }
            }

            return true;
        }
    }
}
