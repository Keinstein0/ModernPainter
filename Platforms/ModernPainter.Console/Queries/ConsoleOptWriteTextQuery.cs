using ModernPainter.Console.Writer;
using ModernPainter.Core.Painter.Writer.DefaultQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Console.Queries
{
    internal class ConsoleOptWriteTextQuery : WriteTextQuery, IUpgradeQuery
    {
        public ConsoleOptWriteTextQuery(WriteTextQuery r) : base(r) { }

        public bool RunConsoleOptimized(ColorMatrix matrix)
        {
            int x = base._point.X;
            int y = base._point.Y;

            int actualY = (int)Math.Floor((double)y / 2);
            int actualX = x;


            for (int i = 0; i < base._text.Length; i++)
            {
                char c = base._text[i];

                actualX = x + i;

                if (actualX >= matrix.XSize || actualY >= matrix.YSize) // catch out of range
                {
                    continue;
                }

                var px = matrix.GetPixel(actualX, actualY);
                px.Character = c;
                px.BackgroundColor.MergeColor(base._colorBackground);
                px.ForegroundColor.MergeColor(base._colorForeground);
                matrix.UpdatePhysicalPixel(px, actualX, actualY);
            }

            return true;
        }
    }
}
