using ModernPainter.Painter.Data;
using ModernPainter.Painter.Writer.ConsoleWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Writer.Queries
{
    internal class WriteTextQuery : IChangePixelQuery
    {
        string _text;
        Point2D _point;
        Color _colorForeground;
        Color _colorBackground;
        
        public WriteTextQuery(string text, Point2D point, Color foreground, Color background)
        {
            _text = text;
            _point = point;

            _colorForeground = foreground;
            _colorBackground = background;
        }

        // This function is a fallback and not suitable for use since text parsing is highly client dependant
        public void RunDefault(IWriter writer)
        {
            for (int i = 0; i < _text.Length; i++)
            {
                char c = _text[i];
                Point2D charpoint = _point;

                charpoint.X += i;

                writer.ChangePixel(charpoint, new Color("#00000000"), c);
            }
        }

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
        }
    }
}
