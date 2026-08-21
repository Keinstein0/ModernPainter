using ModernPainter.Core.Painter.Data;
using ModernPainter.Core.Painter.Reader;
using ModernPainter.Core.Painter.Writer;
using ModernPainter.Core.Painter.Writer.DefaultQueries;
using ModernPainter.Painter.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter
{
    public class ModernPainter
    {
        private IWriter _writer;
        private IReader _reader;
        private Mapping _keyMapping;

        public int TargetFPS = 60;
        public double FPS { get; private set; }
        public int AntiAliasingDots = 0;


        // Fps stuffs
        private int _frameCount = 0;
        DateTime _lastFpsUpdate = DateTime.Now;

        public ModernPainter(IWriter output, IReader input)
        {
            _writer = output;
            _reader = input;
            _keyMapping = new Mapping();
        }


        public void ChangePixel(Vector2D point, Color color)
        {
            IChangePixelQuery query = new ChangePixelQuery(point, color);
            RunQuery(query);
        }

        public void FillRectangle(Rectangle2D pos, Color color)
        {
            IChangePixelQuery query = new FillRectangleQuery(pos, color);
            RunQuery(query);
        }

        public void BlitImage(ModernImage img, Rectangle2D destinationRectangle, Rectangle2D? sourceRectangle = null)
        {
            IChangePixelQuery query = new BlitImageQuery(img, destinationRectangle, sourceRectangle);
            RunQuery(query);
        }

        public void WriteText(Vector2D point, string text, Color? foregroundColor = null, Color? backgroundColor = null)
        {
            foregroundColor = foregroundColor == null ? new Color("#ffffff") : foregroundColor;
            backgroundColor = backgroundColor == null ? new Color("#00000000") : backgroundColor;

            IChangePixelQuery query = new WriteTextQuery(text, point, foregroundColor.Value, backgroundColor.Value);
            RunQuery(query);
        }

        public void Clear(Color? c = null)
        {
            IChangePixelQuery query = new ClearMatrixQuery(c);
            RunQuery(query);
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

        public Rectangle2D GetFrame()
        {
            return _writer.GetSize();
        }

        private void RunQuery(IChangePixelQuery query)
        {
            bool response = _writer.RunOptQuery(query);
            if (!response)
            {
                _writer.RunQuery(query);
            }
        }



        // Reader related
        public bool KeyDown(string key) => _keyMapping.GetMapping(key);
        public Vector2D MousePosition  => _reader.GetMousePosition();
        public async Task<KeyboardTrace> StartKeyboardTrace() => await _reader.StartKeyTrace();
    }
}
