using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary
{
    public static class App2
    {
#if PocketPC || WindowsCE
        private static readonly string appPath = string.Concat(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "\\");
#else
        private static readonly string appPath = string.Concat(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "\\");
#endif

        #region Show Window API
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        [DllImport(SystemDLL.NAME)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(SystemDLL.NAME)]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport(SystemDLL.NAME)]
        private static extern bool EnableWindow(IntPtr hwnd, bool enabled);

        [DllImport(SystemDLL.NAME)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion APIs

        [DllImport(SystemDLL.NAME, CharSet = CharSet.Unicode, SetLastError = false)]
        private static extern IntPtr CreateEvent(IntPtr eventAttributes, bool isManualReset, bool initialState, string eventName);

        [DllImport(SystemDLL.NAME, CharSet = CharSet.Unicode, SetLastError = false)]
        private static extern bool CloseHandle(IntPtr handle);

        public static string AppPath
        {
            get { return appPath; }
        }

        public static bool RunSingleInstance(string windowsTitle, Func<Form> createForm)
        {
            IntPtr h = FindWindow(null, windowsTitle);

            // Uygulama zaten çalışıyosa çalışanı öne getirir ve kendisi kapanır.
            if (h != IntPtr.Zero)
            {
                ShowWindow(h, SW_SHOW);
                EnableWindow(h, true);
                SetForegroundWindow(h);
                return false;
            }
            else
            {
                Form f = createForm();
                f.Text = windowsTitle;
                
                Application.Run(f);
                return true;
            }
        }
    }
}
