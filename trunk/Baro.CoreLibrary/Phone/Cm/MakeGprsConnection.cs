using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Phone.Gprs
{
#if PocketPC || WindowsCE
    public static class GprsConnection
    {
    #region -- DLL - Imports --
        [DllImport("cellcore.dll")]
        private static extern int ConnMgrMapURL(string pwszURL, ref Guid pguid, IntPtr pdwIndex);

        [DllImport("cellcore.dll")]
        private static extern int ConnMgrEstablishConnectionSync(ref ConnectionInfo ci, ref IntPtr phConnection, uint dwTimeout, ref uint pdwStatus);

        [DllImport("cellcore.dll")]
        private static extern IntPtr ConnMgrApiReadyEvent();

        [DllImport("cellcore.dll")]
        private static extern int ConnMgrReleaseConnection(IntPtr hConnection, int bCache);

        [DllImport(SystemDLL.NAME)]
        private static extern int CloseHandle(IntPtr hObject);
        #endregion

    #region Consts
        const int S_OK = 0;
        const uint CONNMGR_PARAM_GUIDDESTNET = 0x1;
        const uint CONNMGR_FLAG_PROXY_HTTP = 0x1;
        const uint CONNMGR_PRIORITY_USERINTERACTIVE = 0x08000;
        const uint INFINITE = 0xffffffff;
        const uint CONNMGR_STATUS_CONNECTED = 0x10;
        #endregion

        private static void WaitForConnMgr()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            mre.Handle = ConnMgrApiReadyEvent();
            mre.WaitOne();
            CloseHandle(mre.Handle);
        }

        public static bool Setup(Uri url)
        {
            return Setup(url.ToString());
        }

        public static bool Setup(string urlStr)
        {
            WaitForConnMgr();

            ConnectionInfo ci = new ConnectionInfo();
            IntPtr phConnection = IntPtr.Zero;
            uint status = 0;

            if (ConnMgrMapURL(urlStr, ref ci.guidDestNet, IntPtr.Zero) != S_OK)
                return false;

            ci.cbSize = (uint)Marshal.SizeOf(ci);
            ci.dwParams = CONNMGR_PARAM_GUIDDESTNET;
            ci.dwFlags = CONNMGR_FLAG_PROXY_HTTP;
            ci.dwPriority = CONNMGR_PRIORITY_USERINTERACTIVE;
            ci.bExclusive = 0;
            ci.bDisabled = 0;
            ci.hWnd = IntPtr.Zero;
            ci.uMsg = 0;
            ci.lParam = 0;

            if (ConnMgrEstablishConnectionSync(ref ci, ref phConnection, INFINITE, ref status) != S_OK &&
                status != CONNMGR_STATUS_CONNECTED)
                return false;

            return true;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ConnectionInfo
        {
            public uint cbSize;
            public uint dwParams;
            public uint dwFlags;
            public uint dwPriority;
            public int bExclusive;
            public int bDisabled;
            public Guid guidDestNet;
            public IntPtr hWnd;
            public uint uMsg;
            public uint lParam;
            public uint ulMaxCost;
            public uint ulMinRcvBw;
            public uint ulMaxConnLatency;
        }
    }
#endif
}
