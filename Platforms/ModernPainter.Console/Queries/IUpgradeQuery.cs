using ModernPainter.Console.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Console.Queries  
{
    internal interface IUpgradeQuery
    {        
        bool RunConsoleOptimized(ColorMatrix matrix);
    }
}
