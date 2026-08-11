using ModernPainter.Core.Painter.Data;

namespace ModernPainter.Core.Painter.Writer.DefaultQueries
{
    public class BlitImageQuery : IChangePixelQuery
    {
        private ModernImage _image;
        private Rectangle2D _sourceRectangle;
        private Rectangle2D _destinationRectangle;
        private Vector2D _size;

        public BlitImageQuery(ModernImage image, Rectangle2D dest, Rectangle2D? source = null)
        {
            _image = image;
            _destinationRectangle = dest;
            _size = new Vector2D(_destinationRectangle.Width, _destinationRectangle.Height);


            _sourceRectangle = source == null ? _image.DefaultSourceRectangle : (Rectangle2D)source;
        }

        public BlitImageQuery(BlitImageQuery q)
        {
            _image = q._image;
            _destinationRectangle = q._destinationRectangle;
            _size = q._size;
            _sourceRectangle = q._sourceRectangle;
        }

        public void RunDefault(IWriter writer)
        {
            Color[][] export = _image.Export(_sourceRectangle, _size);


            for (int y = 0; y < export[0].Length;  y++)
            {
                for (int x = 0; x < export.Length; x++)
                {
                    writer.ChangePixel(new Vector2D(y + _destinationRectangle.Y, x + _destinationRectangle.X), export[x][y]);
                }
            }
        }


    }
}
