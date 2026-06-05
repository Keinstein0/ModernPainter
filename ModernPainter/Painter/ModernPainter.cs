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

        public ModernPainter(IWriter output)
        {
            _writer = output;
        }
    }
}
