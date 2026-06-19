using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StbImageSharp;

namespace ModernPainter.Painter.Data
{
    internal class InternalImage
    {
        private byte[] _rawRgbaData;

        private int _width;
        private int _height;

        private Rectangle2D _source;
        private Vector2D _size;

        public InternalImage(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                _rawRgbaData = image.Data;
                _width = image.Width;
                _height = image.Height;

                _source = new Rectangle2D(0, 0, _width, _height);
                _size = new Vector2D(_width, _height);
            }
        }

        public InternalImage(byte[] rawRgbData, int width, int height, Rectangle2D src, Vector2D dst)
        {
            _rawRgbaData = rawRgbData;
            _width = width;
            _height = height;
            _source = src;
            _size = dst;
        }

        public Rectangle2D GetSize()
        {
            return new Rectangle2D(0, 0, _width, _height);
        }

        public bool RefreshNeeded(Rectangle2D newSource, Vector2D newDest)
        {
            bool srcEqual = _source.Equals(newSource);
            bool dstEqual = _size.Equals(newDest);

            return !(srcEqual && dstEqual); // if both rectangles have stayed the same no refresh needed
        }

        public InternalImage Copy()
        {
            byte[] dataCopy = new byte[_rawRgbaData.Length];
            Buffer.BlockCopy(_rawRgbaData, 0, dataCopy, 0, _rawRgbaData.Length);

            return new InternalImage(dataCopy, _width, _height, _source, _size);
        }

        public void Crop(Rectangle2D rect)
        {
            // Cast properties safely from your Rectangle2D object
            int cropX = (int)rect.X;
            int cropY = (int)rect.Y;
            int cropWidth = (int)rect.Width;
            int cropHeight = (int)rect.Height;

            // Ensure bounds don't blow out past current image limits
            cropX = Math.Clamp(cropX, 0, _width);
            cropY = Math.Clamp(cropY, 0, _height);
            cropWidth = Math.Min(cropWidth, _width - cropX);
            cropHeight = Math.Min(cropHeight, _height - cropY);

            byte[] croppedData = new byte[cropWidth * cropHeight * 4];

            // Extract rows from the original image and pack them seamlessly 
            for (int y = 0; y < cropHeight; y++)
            {
                int sourceIndex = (((cropY + y) * _width) + cropX) * 4;
                int destIndex = (y * cropWidth) * 4;

                Buffer.BlockCopy(_rawRgbaData, sourceIndex, croppedData, destIndex, cropWidth * 4);
            }

            // Update state completely to the new cropped constraints
            _rawRgbaData = croppedData;
            _width = cropWidth;
            _height = cropHeight;
            _source = new Rectangle2D(0, 0, _width, _height);
            _size = new Vector2D(_width, _height);
        }

        public void Stretch(Vector2D targetSize)
        {
            int destWidth = (int)targetSize.X;
            int destHeight = (int)targetSize.Y;

            byte[] stretchedData = new byte[destWidth * destHeight * 4];

            float xRatio = (float)_width / destWidth;
            float yRatio = (float)_height / destHeight;

            // Check if we are shrinking in both directions
            bool isShrinking = destWidth < _width && destHeight < _height;

            if (isShrinking)
            {
                // --- OPTIMIZED DOWN-SAMPLING (BOX FILTER) ---
                // Perfect for shrinking; prevents pixel shimmering/aliasing
                for (int dstY = 0; dstY < destHeight; dstY++)
                {
                    float srcYStart = dstY * yRatio;
                    float srcYEnd = (dstY + 1) * yRatio;
                    int yStart = (int)Math.Floor(srcYStart);
                    int yEnd = Math.Clamp((int)Math.Ceiling(srcYEnd), 0, _height);

                    for (int dstX = 0; dstX < destWidth; dstX++)
                    {
                        float srcXStart = dstX * xRatio;
                        float srcXEnd = (dstX + 1) * xRatio;
                        int xStart = (int)Math.Floor(srcXStart);
                        int xEnd = Math.Clamp((int)Math.Ceiling(srcXEnd), 0, _width);

                        long rSum = 0, gSum = 0, bSum = 0, aSum = 0;
                        int pixelCount = 0;

                        // Average ALL pixels within the source "box" that maps to this 1 destination pixel
                        for (int y = yStart; y < yEnd; y++)
                        {
                            int rowOffset = y * _width * 4;
                            for (int x = xStart; x < xEnd; x++)
                            {
                                int idx = rowOffset + (x * 4);
                                rSum += _rawRgbaData[idx];
                                gSum += _rawRgbaData[idx + 1];
                                bSum += _rawRgbaData[idx + 2];
                                aSum += _rawRgbaData[idx + 3];
                                pixelCount++;
                            }
                        }

                        int destIdx = ((dstY * destWidth) + dstX) * 4;
                        if (pixelCount > 0)
                        {
                            stretchedData[destIdx] = (byte)(rSum / pixelCount);
                            stretchedData[destIdx + 1] = (byte)(gSum / pixelCount);
                            stretchedData[destIdx + 2] = (byte)(bSum / pixelCount);
                            stretchedData[destIdx + 3] = (byte)(aSum / pixelCount);
                        }
                    }
                }
            }
            else
            {
                // --- STANDARD BILINEAR INTERPOLATION (UP-SAMPLING) ---
                // Kept as a fallback just in case you ever scale an image up
                for (int dstY = 0; dstY < destHeight; dstY++)
                {
                    float srcY = dstY * yRatio;
                    int yFloor = Math.Clamp((int)Math.Floor(srcY), 0, _height - 1);
                    int yCeil = Math.Clamp(yFloor + 1, 0, _height - 1);
                    float yWeight = srcY - yFloor;

                    for (int dstX = 0; dstX < destWidth; dstX++)
                    {
                        float srcX = dstX * xRatio;
                        int xFloor = Math.Clamp((int)Math.Floor(srcX), 0, _width - 1);
                        int xCeil = Math.Clamp(xFloor + 1, 0, _width - 1);
                        float xWeight = srcX - xFloor;

                        int idx00 = ((yFloor * _width) + xFloor) * 4;
                        int idx10 = ((yFloor * _width) + xCeil) * 4;
                        int idx01 = ((yCeil * _width) + xFloor) * 4;
                        int idx11 = ((yCeil * _width) + xCeil) * 4;

                        int destIdx = ((dstY * destWidth) + dstX) * 4;

                        stretchedData[destIdx] = (byte)((_rawRgbaData[idx00] * (1 - xWeight) * (1 - yWeight)) + (_rawRgbaData[idx10] * xWeight * (1 - yWeight)) + (_rawRgbaData[idx01] * (1 - xWeight) * yWeight) + (_rawRgbaData[idx11] * xWeight * yWeight));
                        stretchedData[destIdx + 1] = (byte)((_rawRgbaData[idx00 + 1] * (1 - xWeight) * (1 - yWeight)) + (_rawRgbaData[idx10 + 1] * xWeight * (1 - yWeight)) + (_rawRgbaData[idx01 + 1] * (1 - xWeight) * yWeight) + (_rawRgbaData[idx11 + 1] * xWeight * yWeight));
                        stretchedData[destIdx + 2] = (byte)((_rawRgbaData[idx00 + 2] * (1 - xWeight) * (1 - yWeight)) + (_rawRgbaData[idx10 + 2] * xWeight * (1 - yWeight)) + (_rawRgbaData[idx01 + 2] * (1 - xWeight) * yWeight) + (_rawRgbaData[idx11 + 2] * xWeight * yWeight));
                        stretchedData[destIdx + 3] = (byte)((_rawRgbaData[idx00 + 3] * (1 - xWeight) * (1 - yWeight)) + (_rawRgbaData[idx10 + 3] * xWeight * (1 - yWeight)) + (_rawRgbaData[idx01 + 3] * (1 - xWeight) * yWeight) + (_rawRgbaData[idx11 + 3] * xWeight * yWeight));
                    }
                }
            }

            // Commit the structural change
            _rawRgbaData = stretchedData;
            _width = destWidth;
            _height = destHeight;
            //_source = new Rectangle2D(0, 0, _width, _height);
            _size = new Vector2D(_width, _height);
        }

        public Color[][] Export()
        {
            Color[][] pixelMatrix = new Color[_height][];

            for (int y = 0; y < _height; y++)
            {
                pixelMatrix[y] = new Color[_width];
                int rowOffset = y * _width * 4;

                for (int x = 0; x < _width; x++)
                {
                    int index = rowOffset + (x * 4);

                    pixelMatrix[y][x] = new Color(
                        _rawRgbaData[index],
                        _rawRgbaData[index + 1],
                        _rawRgbaData[index + 2],
                        _rawRgbaData[index + 3]
                    );
                }
            }

            return pixelMatrix;
        }
    }
}
