using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter.Writer
{
    public interface IChangePixelQuery
    {
        public void RunDefault(IWriter writer);
    }
}
