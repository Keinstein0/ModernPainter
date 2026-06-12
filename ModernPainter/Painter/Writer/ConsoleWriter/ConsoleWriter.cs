using ModernPainter.Painter.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private PhysicalPixel _expansionPixel = new(); // instance to default (black)
        

        private int _virtualWidth { get { return _width; } }
        private int _virtualHeight { get { return _height * 2; } }

        public ConsoleWriter()
        {
            _width = Console.WindowWidth;
            _height = Console.WindowHeight;
           
            _stdout = Console.OpenStandardOutput();
            _matrix = new ColorMatrix(_width, _height);


            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
        }

        // Setters
        public void ChangePixel(Point2D point, Color color, char? character = null) // ONLY he can do conversion (and the individual queries)
        {
            int x = point.X;
            int y = point.Y;

            int actualY = (int)Math.Floor((double)y / 2);

            var pixel = _matrix.GetPixel(x, actualY);
            
            if (y % 2 != 0)
            {
                pixel.ForegroundColor.MergeColor(color);
            }
            else
            {
                pixel.BackgroundColor.MergeColor(color);
            }

            if (character != null)
            {
                pixel.Character = (char)character;
            }

            _matrix.UpdatePhysicalPixel(pixel, x, actualY);
        }

        void IWriter.RunQuery(IChangePixelQuery query)
        {
            MethodInfo optimizedMethod = query.GetType().GetMethods()
            .FirstOrDefault(m => m.GetCustomAttributes<QueryForAttribute>()
                .Any(attr => attr.DatabaseType.IsAssignableFrom(this.GetType())));

            if (optimizedMethod == null)
            {
                query.RunDefault(this);
            }
            else
            {
                optimizedMethod.Invoke(query, new[] { _matrix });
            }
        }


        // Getters
        public PhysicalColor GetPixel(Point2D point)
        {
            int x = point.X;
            int y = point.Y;

            int actualY = (int)Math.Floor((double)y / 2);

            var pixel = _matrix.GetPixel(x, actualY);

            if (y % 2 != 0)
            {
                return pixel.ForegroundColor;
            }
            else
            {
                return pixel.BackgroundColor;
            }
        }

        public char? GetChar(Point2D point)
        {
            int x = point.X;
            int y = point.Y;

            int actualY = (int)Math.Floor((double)y / 2);

            var pixel = _matrix.GetPixel(x, actualY);

            return pixel.Character == PhysicalPixel.PIXEL ? null : pixel.Character;
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

        // Runners
        public void RenderFrame()
        {
            if (_width != Console.WindowWidth || _height != Console.WindowHeight)
            {
                Resize();
            }

            var buffer = _matrix.AsBytes();

            _stdout.Write(buffer, 0, buffer.Length);
            _stdout.Flush();
        }


        private void Resize()
        {
            _width = Console.WindowWidth;
            _height = Console.WindowHeight;

            _matrix.Resize(_width, _height, _expansionPixel);
        }

        // Configuration
        public void SetExpansionPixelColor(Color c)
        {
            _expansionPixel.BackgroundColor.MergeColor(c);
            _expansionPixel.ForegroundColor.MergeColor(c);
        }
    }

}
