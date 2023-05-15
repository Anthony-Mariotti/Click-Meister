using System.Runtime.InteropServices;

namespace ClickMeister.Windows;

[StructLayout(LayoutKind.Sequential)]
public struct Win32Point
{
    public int X;
    public int Y;

    public Win32Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
