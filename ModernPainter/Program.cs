using ModernPainter.Painter;
using ModernPainter.Painter.Data;
using ModernPainter.Painter.Writer.ConsoleWriter;
using System.Diagnostics;

namespace ModernPainter
{
    internal class Program
    {
        private static long _lastMs = 0;

        static void Main(string[] args)
        {
            ModernPainter.Painter.ModernPainter painter = new(new ConsoleWriter());
            Stopwatch stopwatch = Stopwatch.StartNew();
            bool showColor = false;

            Vector2D pos = new Vector2D(5,5);
            Vector2D vel = new Vector2D(1,1);

            Rectangle2D sourceRect = new Rectangle2D(0, 0, 10, 10);

            ModernImage img = new ModernImage("C:\\Users\\alex\\OneDrive\\Bilder\\Screenshots 1\\Screenshot 2026-06-15 164531.png");

            while (true)
            {
                painter.Clear(new Color("#000000"));

                var rect = painter.GetFrame();

                painter.BlitImage(img, rect);




                //painter.FillRectangle(rect, new Color("#500000"));
                //showColor = !showColor;

                /*Rectangle2D r = new()
                {
                    XPosition = 0,
                    YPosition = 0,
                    XSize = 60,
                    YSize = 60
                };
                painter.FillRectangle(r, new Color("#80000080"));

                painter.WriteText(new Point2D(4, 4), ":(");*/

                //painter.ChangePixel(pos, new Color("#00ff00"));


                if (pos.X <= rect.X || pos.X >= rect.XMax -1)
                {
                    vel.X *= -1;
                }

                pos.X += vel.X;


                if (pos.Y >= rect.YMax -1 || pos.Y <= rect.Y)
                {
                    vel.Y *= -1;
                }

                pos.Y += vel.Y;




                string fpsText = painter.FPS.ToString();
                painter.WriteText(new Vector2D(0, 0), fpsText, new Color(255, 255, 255));

                painter.Update();
                //Thread.Sleep(5);
            }
        }


    }
}