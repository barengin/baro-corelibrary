using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.UI
{
#if PocketPC || WindowsCE
    using System.Windows.Forms;

    public static class GDI_UITools
    {
        private const int BS_MULTILINE = 0x00002000;
        private const int GWL_STYLE = -16;

        [DllImport(SystemDLL.NAME)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport(SystemDLL.NAME)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static void MakeButtonMultiline(Button b)
        {
            IntPtr hwnd = b.Handle;
            int currentStyle = GetWindowLong(hwnd, GWL_STYLE);
            int newStyle = SetWindowLong(hwnd, GWL_STYLE, currentStyle | BS_MULTILINE);
        }

        [DllImport(SystemDLL.NAME)]
        private static extern void mouse_event(MOUSEEVENTF dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [Flags()]
        private enum MOUSEEVENTF
        {
            MOVE = 0x1, /* mouse move */
            LEFTDOWN = 0x2, /* left button down */
            LEFTUP = 0x4, /*left button up */
            RIGHTDOWN = 0x8, /*right button down */
            RIGHTUP = 0x10, /*right button up */
            MIDDLEDOWN = 0x20, /*middle button down */
            MIDDLEUP = 0x40, /* middle button up */
            WHEEL = 0x800, /*wheel button rolled */
            VIRTUALDESK = 0x4000, /* map to entrire virtual desktop */
            ABSOLUTE = 0x8000, /* absolute move */
            TOUCH = 0x100000, /* absolute move */
        }

        public static void SendMouseTap(int x, int y)
        {
            mouse_event(MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.ABSOLUTE, (int)((65535 / Screen.PrimaryScreen.Bounds.Width) * x), (int)((65535 / Screen.PrimaryScreen.Bounds.Height) * y), 0, 0);
            mouse_event(MOUSEEVENTF.LEFTUP, 0, 0, 0, 0);
        }
    }
#endif
}
