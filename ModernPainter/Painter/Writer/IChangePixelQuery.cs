using ModernPainter.Painter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Writer
{
    internal interface IChangePixelQuery
    {
        public void RunDefault(IWriter writer);
    }
}
