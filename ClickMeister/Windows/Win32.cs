using System.Runtime.InteropServices;

namespace ClickMeister.Windows
{
    public static partial class Win32
    {
        public static partial class Mouse
        {
            [LibraryImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static partial bool SetCursorPos(int x, int y);

            [LibraryImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static partial bool GetCursorPos(ref Win32Point pt);


            [LibraryImport("user32.dll",
                EntryPoint = "mouse_event",
                SetLastError = true)]
            internal static partial void Invoke(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        }
    }
}