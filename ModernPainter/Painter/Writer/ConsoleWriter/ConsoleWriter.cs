using ModernPainter.Painter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Writer.ConsoleWriter
{
    internal class ConsoleWriter : IWriter
    {
        private int _width;
        private int _height;
        private readonly Stream _stdout;

        private byte[] _screenBuffer;
        private int _lineStride;

        public ConsoleWriter()
        {
            _width = Console.WindowWidth;
            _height = Console.WindowHeight;

            _stdout = Console.OpenStandardOutput();



            Console.CursorVisible = false;
        }

        public void ChangePixel(Point2D point, PhysicalColor color)
        {



        }

        public PhysicalColor GetPixel(Point2D point)
        {
            throw new NotImplementedException();
        }

        public Rectangle2D GetSize()
        {
            throw new NotImplementedException();
        }

        public void RenderFrame()
        {
            throw new NotImplementedException();
            
        }
    }
}
