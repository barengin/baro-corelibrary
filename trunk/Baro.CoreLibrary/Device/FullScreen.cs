using System;

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Device
{
#if PocketPC || WindowsCE
    public static class FullScreen
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private const int SPISETWORKAREA = 47;
        private const int SPIGETWORKAREA = 48;
        private const int SPIUPDATEINIFILE = 1;

        [DllImport(SystemDLL.NAME)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(SystemDLL.NAME)]
        private static extern int SystemParametersInfo(int uiAction, int uiParam, ref RECT pvParam, int fWinIni);

        [DllImport(SystemDLL.NAME)]
        private static extern IntPtr ShowWindow(IntPtr hwnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_RESTORE = 9;

        private static IntPtr hWndSipButton;
        private static IntPtr hWndTaskBar;

        private static RECT oldDesktop = new RECT();

        public static void Restore()
        {
            SystemParametersInfo(SPISETWORKAREA, 0, ref oldDesktop, SPIUPDATEINIFILE);

            //if (hWndInputPanel != IntPtr.Zero)
            //    ShowWindow(hWndInputPanel, SW_SHOW);

            if (hWndSipButton != IntPtr.Zero)
                ShowWindow(hWndSipButton, SW_SHOW);

            if (hWndTaskBar != IntPtr.Zero)
                ShowWindow(hWndTaskBar, SW_SHOW);
        }

        public static void InitFullScreen()
        {
            SystemParametersInfo(SPIGETWORKAREA, 0, ref oldDesktop, 0);

            IntPtr hWndInputPanel = FindWindow("SipWndClass", null);
            if (hWndInputPanel != IntPtr.Zero)
                ShowWindow(hWndInputPanel, SW_HIDE);

            hWndSipButton = FindWindow("MS_SIPBUTTON", null);
            if (hWndSipButton != IntPtr.Zero)
                ShowWindow(hWndSipButton, SW_HIDE);

            hWndTaskBar = FindWindow("HHTaskBar", null);
            if (hWndTaskBar != IntPtr.Zero)
                ShowWindow(hWndTaskBar, SW_HIDE);

            RECT rtDesktop = new RECT();
            rtDesktop.Left = 0;
            rtDesktop.Top = 0;
            rtDesktop.Bottom = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            rtDesktop.Right = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

            SystemParametersInfo(SPISETWORKAREA, 0, ref rtDesktop, SPIUPDATEINIFILE);
        }
    }
#endif
}
