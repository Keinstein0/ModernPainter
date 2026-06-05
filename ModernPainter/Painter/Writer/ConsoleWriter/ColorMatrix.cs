using ModernPainter.Painter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Writer.ConsoleWriter
{
    internal class ColorMatrix
    {
        public int XSize { get; private set; }
        public int YSize { get; private set; }


        private PhysicalPixel[] _physicalPixels;

        public ColorMatrix(int xsize, int ysize)
        {
            Resize(xsize, ysize);
        }

        public void UpdatePhysicalPixel(PhysicalPixel pixel,  int newX, int newY)
        {
            XSize = newX;
            YSize = newY;
            var newPixels = new PhysicalPixel[XSize * YSize];


            for (int i = 0; i < _physicalPixels.Length; i++) // Translation logic???????
            {        
                newPixels[i] = new PhysicalPixel();
            }
        }

        public void Resize(int newX, int newY)
        {

        }

        public byte[] AsBytes()
        {
            return null;
        }

    }
}
