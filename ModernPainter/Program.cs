using ModernPainter.Painter;
using ModernPainter.Painter.Data;
using ModernPainter.Painter.Writer.ConsoleWriter;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ModernPainter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ModernPainter.Painter.ModernPainter painter = new(new ConsoleWriter());

            

            while (true)
            {
                painter.Clear();
                painter.ChangePixel(new Point2D(1, 1), new Color("#0000ff"));
                painter.Update();
                Thread.Sleep(10);

            }
            


        }
    }
}
