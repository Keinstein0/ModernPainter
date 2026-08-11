using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter.Reader
{
    public interface IKeyboardTrace
    {
        public string GetContent();

        public void End();
    }
}
