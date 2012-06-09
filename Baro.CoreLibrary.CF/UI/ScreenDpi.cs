using System;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.UI
{
    public struct ScreenDpiXY
    {
        public int DpiX, DpiY;
    }

    public static class ScreenDpi
    {
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        private static extern Int32 GetDeviceCaps(IntPtr hdc, Int32 capindex);

        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;

        public static ScreenDpiXY GetScreenDpiXY()
        {
            ScreenDpiXY sxy = new ScreenDpiXY();
            IntPtr scrDC = GetDC(IntPtr.Zero);
            
            sxy.DpiX = GetDeviceCaps(scrDC, LOGPIXELSX);
            sxy.DpiY = GetDeviceCaps(scrDC, LOGPIXELSY);

            ReleaseDC(IntPtr.Zero, scrDC);
            
            return sxy;
        }
    }
}
