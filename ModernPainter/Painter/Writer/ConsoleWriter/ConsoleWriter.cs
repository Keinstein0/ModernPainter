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
        private ColorMatrix _matrix;

        private int _virtualWidth { get { return _width; } }
        private int _virtualHeight { get { return _height * 2; } }

        public ConsoleWriter()
        {
            _width = Console.WindowWidth;
            _height = Console.WindowHeight;

            

           
            _stdout = Console.OpenStandardOutput();
            _matrix = new ColorMatrix(_width, _height);



            Console.CursorVisible = false;
        }

        public void ChangePixel(Point2D point, PhysicalColor color, char? character = null)
        {



        }

        public PhysicalColor GetPixel(Point2D point)
        {
            throw new NotImplementedException();
        }

        public char? GetChar(Point2D point)
        {
            throw new NotImplementedException();
        }

        public Rectangle2D GetSize()
        {
            return new Rectangle2D()
            {
                XPosition = 0,
                YPosition = 0,

                XSize = _virtualWidth,
                YSize = _virtualHeight
            };
        }

        public void RenderFrame()
        {
            throw new NotImplementedException();
            
        }
    }
}
