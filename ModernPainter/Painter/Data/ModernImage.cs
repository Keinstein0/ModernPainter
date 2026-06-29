/*using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;*/


/*using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;*/

namespace ModernPainter.Core.Painter.Data
{
    public class ModernImage
    {

        InternalImage _original;

        InternalImage _scaled;

        public Rectangle2D DefaultSourceRectangle { get; private set; }
        public Vector2D DefaultSizeVector { get; private set; }

        /// <summary>
        /// Loads the image from the specified path
        /// </summary>
        /// <param name="path">path of the image</param>
        /// <param name="defautSourceRect">default source rectangle</param>
        /// <param name="defaultDestRect">default destination rectangle</param>
        /// <exception cref="FileNotFoundException"></exception>
        public ModernImage(string path, Rectangle2D? defautSourceRect = null, Vector2D? defaultSize = null)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Image file at path \"{path}\" not found");
            }
            _original = new InternalImage(path);

            if (defautSourceRect == null)
            {
                DefaultSourceRectangle = _original.GetSize();
            }
            else
            {
                DefaultSourceRectangle = (Rectangle2D)defautSourceRect;
            }

            if (defaultSize == null)
            {
                DefaultSizeVector = new Vector2D(DefaultSourceRectangle.Width, DefaultSourceRectangle.Height);
            }
            else
            {
                DefaultSizeVector = (Vector2D)defaultSize;
            }


            _scaled = _original.Copy();
        }

        /// <summary>
        /// Returns the size of the originally loaded image
        /// </summary>
        /// <returns></returns>
        public Rectangle2D GetSize()
        {
            return _original.GetSize();
        }

        /// <summary>
        /// Prefires the resizing logic for better process control
        /// </summary>
        /// <param name="defaultSource">Source rectangle</param>
        /// <param name="defaultSize">Size</param>
        public void SetDefaultRects(Vector2D defaultSize, Rectangle2D? defaultSource = null)
        {
            if (defaultSource == null)
            {
                defaultSource = _original.GetSize();
            }



            DefaultSizeVector = defaultSize;
            DefaultSourceRectangle = (Rectangle2D)defaultSource;

            if (_scaled.RefreshNeeded((Rectangle2D)defaultSource, DefaultSizeVector))
            {
                _scaled = _original.Copy();

                _scaled.Crop((Rectangle2D)defaultSource);
                _scaled.Stretch(DefaultSizeVector);
            }
        }

        public Color[][] Export(Rectangle2D source, Vector2D size)
        {
            if (_scaled.RefreshNeeded(source, size))
            {
                _scaled = _original.Copy();

                _scaled.Crop(source);
                _scaled.Stretch(size);
            }

            return _scaled.Export();
        }
    }
}
