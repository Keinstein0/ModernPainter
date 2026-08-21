using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernPainter.Console.Reader.MousePositioning
{
    internal interface IMouseDriver
    {
        void Initialize();
        void Update();
        (int charX, int virtualY, bool isPressed)? GetState();
        void Shutdown();
    }
}