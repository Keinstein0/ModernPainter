using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Reader
{
    public class CustomMapCheck : IMapCheck
    {
        private Func<bool> _action;
        
        public CustomMapCheck(Func<bool> action)
        {
            _action = action;
        }
        
        public bool Invoke()
        {
            return _action();
        }
    }
}