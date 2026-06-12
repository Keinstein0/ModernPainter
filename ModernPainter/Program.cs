using ModernPainter.Painter.Data;
using ModernPainter.Painter.Writer.ConsoleWriter;
using System.Text;

namespace ModernPainter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            ColorMatrix matrix = new ColorMatrix(Console.WindowWidth, Console.WindowHeight);
            int lastW = Console.WindowWidth;
            int lastH = Console.WindowHeight;


            for (int x = 0; x < lastW; x++)
            {
                for (int y = 0; y < lastH; y++)
                {
                    PhysicalPixel l = new PhysicalPixel()
                    {
                        Character = PhysicalPixel.PIXEL,
                        BackgroundColor = new PhysicalColor("#ffffff"), // Red
                        ForegroundColor = new PhysicalColor("#000000")  // Blue
                    };
                    l.SaveAsDefault();
                    l.BackgroundColor = new PhysicalColor("#000000");
                    l.ForegroundColor = new PhysicalColor("#ffffff");
                    matrix.UpdatePhysicalPixel(l, x, y);

                }
            }

            PhysicalPixel templatePX = new PhysicalPixel()
            {
                Character = PhysicalPixel.PIXEL,
                BackgroundColor = new PhysicalColor("#ffffff"), // Red
                ForegroundColor = new PhysicalColor("#000000")  // Blue
            };
            templatePX.SaveAsDefault();
            templatePX.BackgroundColor = new PhysicalColor("#000000");
            templatePX.ForegroundColor = new PhysicalColor("#ffffff");




            var stdout = Console.OpenStandardOutput();

            Console.CursorVisible = false;

            int frameCount = 0;
            double fps = 0;
            string fpsString = "FPS: 0";
            DateTime lastFpsUpdate = DateTime.Now;
            while (true)
            {
                frameCount++;
                double elapsedSeconds = (DateTime.Now - lastFpsUpdate).TotalSeconds;

                if (elapsedSeconds >= 1.0) // Update the string once every second
                {
                    fps = frameCount / elapsedSeconds;
                    fpsString = $"FPS: {fps:F1} w:{lastW} h:{lastH * 2}"; // Formats to 1 decimal place, e.g., "FPS: 60.2"

                    frameCount = 0;
                    lastFpsUpdate = DateTime.Now;


                }

                matrix.ClearToDefault();

                
                for (int x = 0; x < lastW; x++)
                {
                    for (int y = 0; y < lastH; y++)
                    {
                        var p = matrix.GetPixel(x, y);
                        p.ShowDefault = new Random().Next(0,2) == 0;
                    }
                }



                for (int i = 0; i < fpsString.Length; i++)
                {
                    char c = fpsString[i];
                    PhysicalPixel letter = new PhysicalPixel()
                    {
                        Character = c,
                        BackgroundColor = new PhysicalColor("#000000"), // Red
                        ForegroundColor = new PhysicalColor("#ffffff")  // Blue
                    };
                    matrix.UpdatePhysicalPixel(letter, 3+i, 0);
                }



                if (lastW != Console.WindowWidth || lastH != Console.WindowHeight)
                {
                    lastW = Console.WindowWidth;
                    lastH = Console.WindowHeight;

                    matrix.Resize(lastW, lastH, templatePX);
                }

                /*
                PhysicalPixel change = new PhysicalPixel()
                {
                    Character = 'O',
                    BackgroundColor = new PhysicalColor("#ff0000"), // Red
                    ForegroundColor = new PhysicalColor("#0000ff")  // Blue
                };

                // Fill your test points
                matrix.UpdatePhysicalPixel(change, 0, 0);
                matrix.UpdatePhysicalPixel(change, (lastW - 1) / 2, (lastH - 1) / 2);
                matrix.UpdatePhysicalPixel(change, lastW - 1, lastH - 1);
                */

                // Compile the frame (make sure AsBytes() prepends "\x1b[H")
                var buffer = matrix.AsBytes();

                // REMOVED: Console.Clear() is gone from the hot-path loop!

                // Blast the data over the existing frame cleanly
                stdout.Write(buffer, 0, buffer.Length);
                stdout.Flush();

                //Thread.Sleep(1);
            }
        }
    }
}
