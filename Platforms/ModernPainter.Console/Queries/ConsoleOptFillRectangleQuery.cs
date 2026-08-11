using ModernPainter.Console.Writer;
using ModernPainter.Core.Painter.Data;
using ModernPainter.Core.Painter.Writer.DefaultQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Console.Queries
{
    public class ConsoleOptFillRectangleQuery : FillRectangleQuery, IUpgradeQuery
    {
        public ConsoleOptFillRectangleQuery(FillRectangleQuery r) : base(r) { }

        

        private ColorMatrix _matrix;

        public bool RunConsoleOptimized(ColorMatrix matrix)
        {
            _matrix = matrix;
            
            if (_color.Alpha != 255)
            {
                return false;
            }
            
            
            int virtualHeight = _matrix.YSize * 2;

            int startX = Math.Max(0, _rectangle.X);
            int startY = Math.Max(0, _rectangle.Y);
            int endX = Math.Min(_matrix.XSize, _rectangle.XMax);
            int endY = Math.Min(virtualHeight, _rectangle.YMax);

            if (startX >= endX || startY >= endY) return false;

            int drawWidth = endX - startX;
            Span<PhysicalPixel> matrixSpan = _matrix.PhysicalPixels.AsSpan();
            PhysicalColor targetColor = new PhysicalColor(_color);

            int startPy = startY / 2;
            int endPy = (endY - 1) / 2; // Inclusive upper physical bound

            for (int pY = startPy; pY <= endPy; pY++)
            {
                int rowStartIndex = pY * _matrix.XSize;

                int virtualTop = pY * 2;
                int virtualBottom = virtualTop + 1;

                bool fillTop = virtualTop >= startY && virtualTop < endY;
                bool fillBottom = virtualBottom >= startY && virtualBottom < endY;

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
