using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter.Reader
{
    public abstract class KeyboardTrace
    {
        public string Content = String.Empty;

        public abstract void End();
    }
}
