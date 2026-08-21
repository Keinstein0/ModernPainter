using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter.Reader
{
    public abstract class KeyboardTrace
    {
        public volatile string Content = String.Empty; // the humble anxiety inducer 3000

        public abstract void End();
    }
}
