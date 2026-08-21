using ModernPainter.Core.Painter.Data;
using ModernPainter.Core.Painter.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Console.Reader
{
    internal class ConsoleReader : IReader
    {
        private KeyboardTrace? _keyboardTrace = null;
        
        
        public Vector2D GetMousePosition()
        {
            throw new NotImplementedException();
        }
        public async Task<KeyboardTrace> StartKeyTrace()
        {
            if (_keyboardTrace != null)
            {
                return _keyboardTrace;
            }

            ConsoleTrace trace = new ConsoleTrace();
            await trace.Start();

            return trace;
        }
    }
}
