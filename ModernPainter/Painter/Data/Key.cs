using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernPainter.Core.Painter.Data
{
    public enum Key
    {
        Unknown = 0,

        // Alphanumerics
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        Num0, Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9,

        // Modifiers
        Shift,
        Control,
        Alt,
        Windows, // Or Command/Meta for cross-platform context

        // Navigation & Editing
        Escape,
        Space,
        Enter,
        Tab,
        Backspace,
        Insert,
        Delete,
        Home,
        End,
        PageUp,
        PageDown,

        // Arrow Keys
        Up,
        Down,
        Left,
        Right,

        // Function Keys
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,

        // Punctuation & Symbols
        Grave,        // `
        Minus,        // -
        Equals,       // =
        LeftBracket,  // [
        RightBracket, // ]
        Backslash,    // \
        Semicolon,    // ;
        Apostrophe,   // '
        Comma,        // ,
        Period,       // .
        Slash,        // /

        // Lock Keys
        CapsLock,
        NumLock,

        // Numpad Specific (Optional, if you want to differentiate from top-row numbers)
        Numpad0, Numpad1, Numpad2, Numpad3, Numpad4,
        Numpad5, Numpad6, Numpad7, Numpad8, Numpad9,
        NumpadDivide, NumpadMultiply, NumpadSubtract, NumpadAdd, NumpadEnter,

        // Mouse things
        MouseLeft, MouseRight, MouseMiddle,
        MouseScrollUp, MouseScrollDown
    }
}