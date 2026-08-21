using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernPainter.Painter.Reader
{
    public class Mapping
    {
        private Dictionary<string, IMapCheck> _mapping = new();

        public void AddMapping(string key, IMapCheck mapCheck)
        {
            _mapping[key] = mapCheck;
        }

        public bool GetMapping(string key)
        {
            if (_mapping.ContainsKey(key))
            {
                return _mapping[key].Invoke();
            }

            throw new Exception($"Mapping for key '{key}' not found.");
        }
    }
}