using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Baro.CoreLibrary.Extensions
{
#if PocketPC || WindowsCE
    public struct ScreenDpiXY
    {
        public int DpiX, DpiY;
    }

    /// <summary>
    /// GDI Tools2
    /// </summary>
    public static class GDITools
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

        /// <summary>
        /// Sets the location of the SIP
        /// </summary>
        /// <param name="x">x coordinate of the SIP</param>
        /// <param name="y">y coordinate of the SIP</param>
        public static void SetSIPLocation(int x, int y)
        {
            try
            {
                SIPINFO info = new SIPINFO();
                info.cbSize = Marshal.SizeOf(info);

                if (SipGetInfo(out info))
                {
                    info.rcSipRect.right = (info.rcSipRect.right -
                        info.rcSipRect.left) + x;
                    info.rcSipRect.left = x;

                    info.rcSipRect.bottom = (info.rcSipRect.bottom -
                        info.rcSipRect.top) + y;
                    info.rcSipRect.top = y;

                    SipSetInfo(ref info);
                }
            }
            catch (DllNotFoundException)
            {
            }
            catch (MissingMethodException)
            {
            }
        }

        /// <summary>
        /// This structure contains information about the current state of the input panel, such as the input panel size, screen location, docked status, and visibility status.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct SIPINFO
        {
            /// <summary>
            /// Size, in bytes, of the SIPINFO structure. This member must be filled in by the application with the size of operator. Because the system can check the size of the structure to determine the operating system version number, this member allows for future enhancements to the SIPINFO structure while maintaining backward compatibility.
            /// </summary>
            public int cbSize;

            /// <summary>
            /// Specifies flags representing state information of the input panel. It is any combination of the following bit flags:
            /// Value               Description
            /// SIPF_DOCKED         The input panel is docked, or not floating.
            /// SIPF_LOCKED         The input panel is locked, meaning that the user cannot change its visible status.
            /// SIPF_OFF            The input panel is off, or not visible.
            /// SIPF_ON             The input panel is on, or visible.
            /// </summary>
            public uint fdwFlags;

            /// <summary>
            /// Rectangle, in screen coordinates, that represents the area of the desktop not obscured by the input panel. If the input panel is floating, this rectangle is equivalent to the working area. Full-screen applications that respond to input panel size changes can set their window rectangle to this rectangle. If the input panel is docked but does not occupy an entire edge, then this rectangle represents the largest rectangle not obscured by the input panel. If an application wants to use the screen space around the input panel, it needs to reference rcSipRect.
            /// </summary>
            public RECT rcVisibleDesktop;

            /// <summary>
            /// Rectangle, in screen coordinates of the window rectangle and not the client area, the represents the size and location of the input panel. An application does not generally use this information unless it needs to wrap around a floating or a docked input panel that does not occupy an entire edge.
            /// </summary>
            public RECT rcSipRect;

            /// <summary>
            /// Specifies the size of the data pointed to by the pvImData member.
            /// </summary>
            public uint dwImDataSize;

            /// <summary>
            /// Void pointer to input method (IM)-defined data. The IM calls the IInputMethod::GetImData and IInputMethod::SetImData methods to send and receive information from this structure. 
            /// </summary>
            public IntPtr pvImData;
        }

        /// <summary>
        /// This structure defines the coordinates of the upper-left and lower-right corners of a rectangle. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            /// <summary>
            /// Specifies the x-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int left;

            /// <summary>
            /// Specifies the y-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int top;

            /// <summary>
            /// Specifies the x-coordinate of the lower-right corner of the rectangle. 
            /// </summary>
            public int right;

            /// <summary>
            /// Specifies the y-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int bottom;
        }

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SipSetInfo(ref SIPINFO sipInfo);

        [DllImport(SystemDLL.NAME)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SipGetInfo(out SIPINFO info);
    }
#endif
}
