using ModernPainter.Console.Reader.MousePositioning;
using ModernPainter.Core.Painter.Data;
using ModernPainter.Core.Painter.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Console.Reader
{
    internal class ConsoleReader : IReader, IDisposable
    {
        private KeyboardTrace? _keyboardTrace = null;
        private IMouseDriver _mouseDriver = null;
        
        public ConsoleReader()
        {
            bool isWindows = OperatingSystem.IsWindows();
            _mouseDriver = isWindows ? new WindowsMouseDriver() : new LinuxMouseDriver();

            _mouseDriver.Initialize();
        }

        public void Dispose()
        {
            _mouseDriver.Shutdown();
        }

        public Vector2D GetMousePosition()
        {
            var state = _mouseDriver.GetState();
            if (state.HasValue)
            {
                (int x, int y, bool isClick) = state.Value;
                return new Vector2D(x, y);
            }
            else
            {
                return new Vector2D(0, 0); // error on retrieving mouse position
            }
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

        public void Update()
        {
            _mouseDriver.Update();
        }
    }
}
