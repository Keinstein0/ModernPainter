using ModernPainter.Painter.Data;
using ModernPainter.Painter.Writer;
using ModernPainter.Painter.Writer.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter
{
    internal class ModernPainter
    {
        private IWriter _writer;

        public int TargetFPS = 60;
        public double FPS { get; private set; }
        public int AntiAliasingDots = 0;


        // Fps stuffs
        private int _frameCount = 0;
        DateTime _lastFpsUpdate = DateTime.Now;

        public ModernPainter(IWriter output)
        {
            _writer = output;
        }

        public void ChangePixel(Point2D point, Color color)
        {
            IChangePixelQuery query = new ChangePixelQuery(point, color);
            _writer.RunQuery(query);
        }

        public void WriteText(Point2D point, string text, Color? foregroundColor = null, Color? backgroundColor = null)
        {
            foregroundColor = foregroundColor == null ? new Color("#ffffff") : foregroundColor;
            backgroundColor = backgroundColor == null ? new Color("#00000000") : backgroundColor;

            IChangePixelQuery query = new WriteTextQuery(text, point, foregroundColor, backgroundColor);
            _writer.RunQuery(query);
        }

        public void Clear(Color? c = null)
        {
            IChangePixelQuery query = new ClearMatrixQuery(c);
            _writer.RunQuery(query);
        }

        public void Update()
        {
            _frameCount++;
            _writer.RenderFrame();

            double elapsedSeconds = (DateTime.Now - _lastFpsUpdate).TotalSeconds;
            if (elapsedSeconds >= 1.0)
            {
                FPS = _frameCount / elapsedSeconds;

                _frameCount = 0;
                _lastFpsUpdate = DateTime.Now;
            }
        }

        
    }
}
