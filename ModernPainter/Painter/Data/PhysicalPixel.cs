using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Data
{
    internal class PhysicalPixel
    {
        public const char PIXEL = '▄';

        private char _actualCharacter = PIXEL;
        private char _defaultCharacter = PIXEL;

        private PhysicalColor _actualForegroundColor = new PhysicalColor();
        private PhysicalColor _defaultForegroundColor = new PhysicalColor();

        private PhysicalColor _actualBackgroundColor = new PhysicalColor();
        private PhysicalColor _defaultBackgroundColor = new PhysicalColor();

        /// <summary>
        /// Toggle if the actual or default value is shown
        /// </summary>
        public bool ShowDefault { get; set; } = true;

        public char Character
        {
            get => ShowDefault ? _defaultCharacter : _actualCharacter;
            set { _actualCharacter = value; ShowDefault = false; }
        }

        public PhysicalPixel Clone()
        {
            return new PhysicalPixel()
            {
                _actualCharacter = _actualCharacter,
                _defaultCharacter = _defaultCharacter,

                _actualForegroundColor = _actualForegroundColor,
                _defaultForegroundColor = _defaultForegroundColor,

                _actualBackgroundColor = _actualBackgroundColor,
                _defaultBackgroundColor = _defaultBackgroundColor,

                ShowDefault = ShowDefault,
            };
        }

        public PhysicalColor ForegroundColor
        {
            get => ShowDefault ? _defaultForegroundColor : _actualForegroundColor;
            set { _actualForegroundColor = value; ShowDefault = false; }
        }

        public PhysicalColor BackgroundColor
        {
            get => ShowDefault ? _defaultBackgroundColor : _actualBackgroundColor;
            set { _actualBackgroundColor = value; ShowDefault = false; }
        }

        /// <summary>
        /// Saves the current state of the pixel as its default texture
        /// </summary>
        public void SaveAsDefault()
        {
            _defaultCharacter = _actualCharacter;
            _defaultForegroundColor = _actualForegroundColor;
            _defaultBackgroundColor = _actualBackgroundColor;
        }
    }
}
