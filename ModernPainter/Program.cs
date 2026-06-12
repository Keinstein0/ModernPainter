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

            while (true)
            {
                painter.Clear(new Color(showColor ? "#0000a5" : "#000000"));
                //showColor = !showColor;

                painter.WriteText(new Point2D(4, 4), ":(");

                string fpsText = painter.FPS.ToString();
                painter.WriteText(new Point2D(0, 0), fpsText, new Color(255, 255, 255));

                painter.Update();
                Thread.Sleep(5);
            }
        }


    }
}