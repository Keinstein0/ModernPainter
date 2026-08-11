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
    public class ConsoleOptClearMatrixQuery : ClearMatrixQuery, IUpgradeQuery
    {
        public ConsoleOptClearMatrixQuery(ClearMatrixQuery r) : base(r) { }


        public bool RunConsoleOptimized(ColorMatrix matrix)
        {
            PhysicalPixel p = new PhysicalPixel();
            p.BackgroundColor = new PhysicalColor(base._color);
            p.ForegroundColor = new PhysicalColor(base._color);

            Span<PhysicalPixel> rowSpan = matrix.PhysicalPixels.AsSpan(0, matrix.PhysicalPixels.Length);
            rowSpan.Fill(p);

            return true;
        }
    }
}
