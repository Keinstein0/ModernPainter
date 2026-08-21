using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ModernPainter.Console.Reader.MousePositioning
{
    public class WindowsMouseDriver : IMouseDriver
{
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X; public int Y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct COORD { public short X; public short Y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct SMALL_RECT { public short Left; public short Top; public short Right; public short Bottom; }

    [StructLayout(LayoutKind.Sequential)]
    private struct CONSOLE_SCREEN_BUFFER_INFOEX
    {
        public uint cbSize;
        public COORD dwSize;
        public COORD dwCursorPosition;
        public ushort wAttributes;
        public SMALL_RECT srWindow;
        public COORD dwMaximumWindowSize;
        public ushort wPopupAttributes;
        public bool bFullscreenSupported;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public uint[] ColorTable;
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFOEX csbi);

    [DllImport("kernel32.dll")]
    private static extern COORD GetConsoleFontSize(IntPtr hConsoleOutput, int nFont);

    private const int STD_OUTPUT_HANDLE = -11;
    private const int VK_LBUTTON = 0x01;

    private (int charX, int virtualY, bool isPressed)? _currentState;

    public void Initialize() { }
    public void Shutdown() { }

    public void Update()
    {
        IntPtr hwnd = GetConsoleWindow();
        IntPtr hOutput = GetStdHandle(STD_OUTPUT_HANDLE);
        if (hwnd == IntPtr.Zero || hOutput == (IntPtr)(-1))
        {
            _currentState = null;
            return;
        }

        if (!GetCursorPos(out POINT mouse))
        {
            _currentState = null;
            return;
        }

        // ScreenToClient automatically strips title bar and outer window margins
        ScreenToClient(hwnd, ref mouse);

        CONSOLE_SCREEN_BUFFER_INFOEX csbi = new CONSOLE_SCREEN_BUFFER_INFOEX();
        csbi.cbSize = (uint)Marshal.SizeOf(csbi);
        if (!GetConsoleScreenBufferInfoEx(hOutput, ref csbi))
        {
            _currentState = null;
            return;
        }

        COORD fontSize = GetConsoleFontSize(hOutput, 0);
        if (fontSize.X == 0 || fontSize.Y == 0)
        {
            _currentState = null;
            return;
        }

        int clientX = mouse.X - (csbi.srWindow.Left * fontSize.X);
        int clientY = mouse.Y - (csbi.srWindow.Top * fontSize.Y);

        if (clientX < 0 || clientY < 0)
        {
            _currentState = null;
            return;
        }

        int charX = clientX / fontSize.X;
        int halfCellHeight = fontSize.Y / 2;
        int virtualY = clientY / halfCellHeight;

        // Check left mouse button state asynchronously
        bool isPressed = (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0;

        _currentState = (charX, virtualY, isPressed);
    }

        public (int charX, int virtualY, bool isPressed)? GetState() => _currentState;
    }
}
