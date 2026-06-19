using ModernPainter.Painter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Writer
{
    internal interface IWriter
    {
        // Configuration
        public void SetExpansionPixelColor(Color c);

        
        
        /// <summary>
        /// Changes the pixel at the (virtual) point to be a certain color and character
        /// </summary>
        /// <param name="point">virtual point of the pixel</param>
        /// <param name="color">color to set the pixel to</param>
        /// <param name="character">character to set the pixel to</param>
        internal void ChangePixel(Vector2D point, Color color, char? character = null);

        /// <summary>
        /// Runs a query to it's best ability
        /// </summary>
        /// <param name="query"></param>
        public void RunQuery(IChangePixelQuery query);


        /// <summary> Data Scientist
        /// Get the color of the pixel at (virtual) point
        /// </summary>
        /// <param name="point">virtual point of the pixel</param>
        /// <returns></returns>
        public PhysicalColor GetPixel(Vector2D point);

        /// <summary>
        /// Get the character of the pixel at (virtual) point
        /// </summary>
        /// <param name="point">virtual point of the pixel</param>
        /// <returns></returns>
        public char? GetChar(Vector2D point);

        /// <summary>
        /// Get the size of the current window in virtual pixels
        /// </summary>
        /// <returns></returns>
        public Rectangle2D GetSize();

        /// <summary>
        /// Update the current content of the screen to the buffer
        /// </summary>
        public void RenderFrame();




    }
}
