using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Data
{
    public struct Rectangle2D
    {
        /// <summary>
        /// Start position of the rectangle on the X Axis
        /// </summary>
        public int X;
        /// <summary>
        /// Start position of the rectangle on the Y Axis
        /// </summary>
        public int Y;
        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public int Width;
        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public int Height;

        public int XMax { get { return X + Width; } } 
        public int YMax { get { return Y + Height; } }

        public Rectangle2D(int xPosition, int yPosition, int xSize, int ySize)
        {
            X = xPosition;
            Y = yPosition;
            Width = xSize;
            Height = ySize;
        }
        public Rectangle2D() { }
    }


}

