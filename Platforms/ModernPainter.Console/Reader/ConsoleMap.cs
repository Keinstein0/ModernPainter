using System;
using System.Collections.Generic;

namespace ModernPainter.Core.Painter.Data
{
    internal static class ConsoleMap
    {
        public static readonly Dictionary<Key, ConsoleKey> Map = new()
        {
            { Key.Unknown, (ConsoleKey)0 },

            // Alphanumerics
            { Key.A, ConsoleKey.A },
            { Key.B, ConsoleKey.B },
            { Key.C, ConsoleKey.C },
            { Key.D, ConsoleKey.D },
            { Key.E, ConsoleKey.E },
            { Key.F, ConsoleKey.F },
            { Key.G, ConsoleKey.G },
            { Key.H, ConsoleKey.H },
            { Key.I, ConsoleKey.I },
            { Key.J, ConsoleKey.J },
            { Key.K, ConsoleKey.K },
            { Key.L, ConsoleKey.L },
            { Key.M, ConsoleKey.M },
            { Key.N, ConsoleKey.N },
            { Key.O, ConsoleKey.O },
            { Key.P, ConsoleKey.P },
            { Key.Q, ConsoleKey.Q },
            { Key.R, ConsoleKey.R },
            { Key.S, ConsoleKey.S },
            { Key.T, ConsoleKey.T },
            { Key.U, ConsoleKey.U },
            { Key.V, ConsoleKey.V },
            { Key.W, ConsoleKey.W },
            { Key.X, ConsoleKey.X },
            { Key.Y, ConsoleKey.Y },
            { Key.Z, ConsoleKey.Z },

            { Key.Num0, ConsoleKey.D0 },
            { Key.Num1, ConsoleKey.D1 },
            { Key.Num2, ConsoleKey.D2 },
            { Key.Num3, ConsoleKey.D3 },
            { Key.Num4, ConsoleKey.D4 },
            { Key.Num5, ConsoleKey.D5 },
            { Key.Num6, ConsoleKey.D6 },
            { Key.Num7, ConsoleKey.D7 },
            { Key.Num8, ConsoleKey.D8 },
            { Key.Num9, ConsoleKey.D9 },

            // Modifiers (Note: Standalone modifier tracking is limited in raw consoles)
            { Key.Shift, ConsoleKey.NoName },
            { Key.Control, ConsoleKey.NoName },
            { Key.Alt, ConsoleKey.NoName },

            // Navigation & Editing
            { Key.Escape, ConsoleKey.Escape },
            { Key.Space, ConsoleKey.Spacebar },
            { Key.Enter, ConsoleKey.Enter },
            { Key.Tab, ConsoleKey.Tab },
            { Key.Backspace, ConsoleKey.Backspace },
            { Key.Insert, ConsoleKey.Insert },
            { Key.Delete, ConsoleKey.Delete },
            { Key.Home, ConsoleKey.Home },
            { Key.End, ConsoleKey.End },
            { Key.PageUp, ConsoleKey.PageUp },
            { Key.PageDown, ConsoleKey.PageDown },

            // Arrow Keys
            { Key.Up, ConsoleKey.UpArrow },
            { Key.Down, ConsoleKey.DownArrow },
            { Key.Left, ConsoleKey.LeftArrow },
            { Key.Right, ConsoleKey.RightArrow },

            // Function Keys
            { Key.F1, ConsoleKey.F1 },
            { Key.F2, ConsoleKey.F2 },
            { Key.F3, ConsoleKey.F3 },
            { Key.F4, ConsoleKey.F4 },
            { Key.F5, ConsoleKey.F5 },
            { Key.F6, ConsoleKey.F6 },
            { Key.F7, ConsoleKey.F7 },
            { Key.F8, ConsoleKey.F8 },
            { Key.F9, ConsoleKey.F9 },
            { Key.F10, ConsoleKey.F10 },
            { Key.F11, ConsoleKey.F11 },
            { Key.F12, ConsoleKey.F12 },

            // Punctuation & Symbols
            { Key.Grave, ConsoleKey.Oem3 },          // Usually ` / ~
            { Key.Minus, ConsoleKey.OemMinus },
            { Key.Equals, ConsoleKey.OemPlus },       // On standard keyboards, the = key is the plus key un-shifted
            { Key.LeftBracket, ConsoleKey.Oem4 },    // [
            { Key.RightBracket, ConsoleKey.Oem6 },   // ]
            { Key.Backslash, ConsoleKey.Oem5 },      // \
            { Key.Semicolon, ConsoleKey.Oem1 },      // ;
            { Key.Apostrophe, ConsoleKey.Oem7 },     // '
            { Key.Comma, ConsoleKey.OemComma },
            { Key.Period, ConsoleKey.OemPeriod },
            { Key.Slash, ConsoleKey.Oem2 },          // /

            // Lock Keys
            { Key.CapsLock, ConsoleKey.NoName },
            { Key.NumLock, ConsoleKey.NoName },

            // Numpad Specific
            { Key.Numpad0, ConsoleKey.NumPad0 },
            { Key.Numpad1, ConsoleKey.NumPad1 },
            { Key.Numpad2, ConsoleKey.NumPad2 },
            { Key.Numpad3, ConsoleKey.NumPad3 },
            { Key.Numpad4, ConsoleKey.NumPad4 },
            { Key.Numpad5, ConsoleKey.NumPad5 },
            { Key.Numpad6, ConsoleKey.NumPad6 },
            { Key.Numpad7, ConsoleKey.NumPad7 },
            { Key.Numpad8, ConsoleKey.NumPad8 },
            { Key.Numpad9, ConsoleKey.NumPad9 },
            { Key.NumpadDivide, ConsoleKey.Divide },
            { Key.NumpadMultiply, ConsoleKey.Multiply },
            { Key.NumpadSubtract, ConsoleKey.Subtract },
            { Key.NumpadAdd, ConsoleKey.Add },
            { Key.NumpadEnter, ConsoleKey.Enter },    // Console API doesn't distinguish main Enter from Numpad Enter
        };
    }
}