using ModernPainter.Core.Painter.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter.Reader
{
    public interface IReader
    {
        public bool KeyDown(Key key);
        public IKeyboardTrace StartKeyTrace();
        public Vector2D GetMousePosition();
    }
}
