using ModernPainter.Core.Painter.Data;
using ModernPainter.Core.Painter.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Console.Reader
{
    internal class ConsoleReader : IReader
    {
        public Vector2D GetMousePosition()
        {
            throw new NotImplementedException();
        }

        public bool KeyDown(Key key)
        {
            if (ConsoleMap.Map.ContainsKey(key))
            {
                return GetKeyboardKey(key);
            }
            else
            {
                return GetMouseKey(key);
            }
        }

        public IKeyboardTrace StartKeyTrace()
        {
            throw new NotImplementedException();
        }





        private bool GetKeyboardKey(Key key)
        {
            bool result = false;
            ConsoleKey ckey = ConsoleMap.Map[key];
            bool isModifier = ckey.Equals(ConsoleKey.NoName);


            // On windows get Locks
            if (System.OperatingSystem.IsWindows())
            {
                if (key == Key.CapsLock)
                {
                    return System.Console.CapsLock;
                }
                if (key == Key.NumLock)
                {
                    return System.Console.NumberLock;
                }
            }


            // Normal keys
            if (System.Console.KeyAvailable)
            {
                var keyInfo = System.Console.ReadKey(intercept: true);
                result = keyInfo.Key == ckey;


                // modifier
                if (isModifier)
                {
                    bool shiftPressed = (keyInfo.Modifiers & ConsoleModifiers.Shift) != 0;
                    bool altPressed = (keyInfo.Modifiers & ConsoleModifiers.Alt) != 0;
                    bool ctrlPressed = (keyInfo.Modifiers & ConsoleModifiers.Control) != 0;

                    if (key == Key.Shift && shiftPressed) return true;
                    if (key == Key.Alt && altPressed) return true;
                    if (key == Key.Control && ctrlPressed) return true;
                }
            }

            return result;
        }


        private bool GetMouseKey(Key key)
        {
            return false;
        }
    }
}
