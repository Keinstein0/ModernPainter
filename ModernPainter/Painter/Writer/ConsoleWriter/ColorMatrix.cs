using ModernPainter.Painter.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Writer.ConsoleWriter
{
    internal class ColorMatrix
    {
        public int XSize { get; private set; }
        public int YSize { get; private set; }


        public PhysicalPixel[] PhysicalPixels;

        public ColorMatrix(int xsize, int ysize)
        {
            Resize(xsize, ysize);
        }

        public void UpdatePhysicalPixel(PhysicalPixel pixel,  int x, int y)
        {
            int actualIndex = (XSize * y) + x;

            try
            {
                PhysicalPixels[actualIndex] = pixel;
            }
            catch
            {
                throw new Exception($"Coordinates out of range");
            }
        }

        public PhysicalPixel GetPixel(int x, int y)
        {
            int actualIndex = (XSize * y) + x;

            try
            {
                return PhysicalPixels[actualIndex];
            }
            catch
            {
                throw new Exception($"Coordinates out of range");
            }
        }

        public void Resize(int newX, int newY, PhysicalPixel? extensionTemplate = null)
        {
            var newPixels = new PhysicalPixel[newX * newY];

            for (int i = 0; i < newPixels.Length; i++)
            {
                if (extensionTemplate != null)
                {
                    newPixels[i] = extensionTemplate.Clone();
                }
                else
                {
                    newPixels[i] = new PhysicalPixel()
                    {
                        Character = PhysicalPixel.PIXEL,
                        BackgroundColor = new PhysicalColor(),
                        ForegroundColor = new PhysicalColor()
                    };
                }
            }


            int minX = Math.Min(XSize, newX);
            int minY = Math.Min(YSize, newY);

            for (int y = 0; y < minY; y++)
            {
                for (int x = 0; x < minX; x++)
                {
                    int oldIndex = y * XSize + x;
                    int newIndex = y * newX + x;

                    newPixels[newIndex] = PhysicalPixels[oldIndex];
                }
            }

            XSize = newX;
            YSize = newY;
            PhysicalPixels = newPixels;
        }

        public void ClearToDefault()
        {
            foreach (PhysicalPixel pixel in PhysicalPixels)
            {
                pixel.ShowDefault = true;
            }
        }

        private byte[] _exportBuffer = new byte[1024*768];

        public byte[] AsBytes()
        {
            int writeIndex = 0;

            // 1. Tell the terminal to reset its cursor to (0,0) instantly via ANSI
            ReadOnlySpan<byte> homeCursor = "\x1b[H"u8;
            homeCursor.CopyTo(_exportBuffer.AsSpan(writeIndex));
            writeIndex += homeCursor.Length;

            PhysicalColor lastForeground = default;
            PhysicalColor lastBackground = default;
            bool firstPixel = true;

            // 2. Iterate through the grid row by row
            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    PhysicalPixel currentPixel = PhysicalPixels[(y * XSize) + x];

                    // Emit foreground color ANSI code ONLY if it changed
                    if (firstPixel || currentPixel.ForegroundColor != lastForeground)
                    {
                        writeIndex += AppendAnsiColor(_exportBuffer, writeIndex, isBackground: false, currentPixel.ForegroundColor);
                        lastForeground = currentPixel.ForegroundColor;
                    }

                    // Emit background color ANSI code ONLY if it changed
                    if (firstPixel || currentPixel.BackgroundColor != lastBackground)
                    {
                        writeIndex += AppendAnsiColor(_exportBuffer, writeIndex, isBackground: true, currentPixel.BackgroundColor);
                        lastBackground = currentPixel.BackgroundColor;
                    }

                    firstPixel = false;

                    ReadOnlySpan<char> charSpan = stackalloc char[] { currentPixel.Character };
                    int bytesWritten = Encoding.UTF8.GetBytes(charSpan, _exportBuffer.AsSpan(writeIndex));
                    writeIndex += bytesWritten;
                }

                // End of row: Add a newline character so the terminal jumps to the next row safely
                if (y < YSize - 1)
                {
                    _exportBuffer[writeIndex++] = (byte)'\r';
                    _exportBuffer[writeIndex++] = (byte)'\n';
                }
            }

            // Slice out exactly the number of bytes we used
            return _exportBuffer.AsSpan(0, writeIndex).ToArray();
        }

        private int AppendAnsiColor(byte[] buffer, int offset, bool isBackground, PhysicalColor color)
        {
            int start = offset;
            buffer[offset++] = 0x1B; // ESC character
            buffer[offset++] = (byte)'[';
            buffer[offset++] = isBackground ? (byte)'4' : (byte)'3'; // 48 = BG, 38 = FG
            buffer[offset++] = (byte)'8';
            buffer[offset++] = (byte)';';
            buffer[offset++] = (byte)'2'; // Indicates TrueColor (RGB) mode
            buffer[offset++] = (byte)';';

            // Mathematically translate colors directly to ASCII bytes
            offset += FastWriteInt(buffer, offset, (byte)color.Red);
            buffer[offset++] = (byte)';';
            offset += FastWriteInt(buffer, offset, (byte)color.Green);
            buffer[offset++] = (byte)';';
            offset += FastWriteInt(buffer, offset, (byte)color.Blue);
            buffer[offset++] = (byte)'m';

            return offset - start;
        }

        private int FastWriteInt(byte[] buffer, int offset, byte value)
        {
            if (value >= 100)
            {
                buffer[offset] = (byte)('0' + (value / 100));
                buffer[offset + 1] = (byte)('0' + ((value / 10) % 10));
                buffer[offset + 2] = (byte)('0' + (value % 10));
                return 3;
            }
            if (value >= 10)
            {
                buffer[offset] = (byte)('0' + (value / 10));
                buffer[offset + 1] = (byte)('0' + (value % 10));
                return 2;
            }
            buffer[offset] = (byte)('0' + value);
            return 1;
        }

    }
}
