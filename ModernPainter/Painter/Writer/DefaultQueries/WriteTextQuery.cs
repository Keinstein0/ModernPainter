using ModernPainter.Core.Painter.Data;
using ModernPainter.Core.Painter.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter.Writer.DefaultQueries
{
    public class WriteTextQuery : IChangePixelQuery
    {
        protected string _text;
        protected Vector2D _point;
        protected Color _colorForeground;
        protected Color _colorBackground;
        
        public WriteTextQuery(string text, Vector2D point, Color foreground, Color background)
        {
            _text = text;
            _point = point;

            _colorForeground = foreground;
            _colorBackground = background;
        }

        public WriteTextQuery(WriteTextQuery t)
        {
            _text = t._text;
            _point = t._point;
            _colorForeground = t._colorForeground;
            _colorBackground = t._colorBackground;
        }

        // This function is a fallback and not suitable for use since text parsing is highly client dependant
        public void RunDefault(IWriter writer)
        {
            int yOffset = 0;
            
            for (int i = 0; i < _text.Length; i++)
            {
                char c = _text[i];

                if (c == '\n')
                {
                    yOffset++;
                    continue;
                }

                Vector2D charpoint = _point;

                charpoint.X += i;
                charpoint.Y += yOffset * 2;

                writer.ChangePixel(charpoint, new Color("#00000000"), c);
            }
        }
        /*
        [QueryFor(typeof(ConsoleWriter.ConsoleWriter))]
        public void RunConsole(ColorMatrix matrix)
        {
            int x = _point.X;
            int y = _point.Y;

            int actualY = (int)Math.Floor((double)y / 2);
            int actualX = x;


            for (int i = 0; i < _text.Length; i++)
            {
                char c = _text[i];

                actualX = x + i;

                if (actualX >= matrix.XSize || actualY >= matrix.YSize) // catch out of range
                {
                    continue;
                }

                var px = matrix.GetPixel(actualX, actualY);
                px.Character = c;
                px.BackgroundColor.MergeColor(_colorBackground);
                px.ForegroundColor.MergeColor(_colorForeground);
                matrix.UpdatePhysicalPixel(px, actualX, actualY);
            }
        }*/
    }
}
