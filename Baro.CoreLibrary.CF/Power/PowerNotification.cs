using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Baro.CoreLibrary.Power
{
#if PocketPC || WindowsCE
    public class PowerNotifications
    {
        private IntPtr ptr = IntPtr.Zero;
        private Thread t = null;
        volatile bool done = false;

        [DllImport(SystemDLL.NAME)]
        private static extern IntPtr RequestPowerNotifications(IntPtr hMsgQ, uint Flags);

        [DllImport(SystemDLL.NAME)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, int wait);

        [DllImport(SystemDLL.NAME)]
        private static extern IntPtr CreateMsgQueue(string name, ref MsgQOptions options);

        [DllImport(SystemDLL.NAME)]
        private static extern bool ReadMsgQueue(IntPtr hMsgQ, byte[] lpBuffer, uint cbBufSize, ref uint lpNumRead, int dwTimeout, ref uint pdwFlags);

        public PowerNotifications()
        {
            MsgQOptions options = new MsgQOptions();
            options.dwFlags = 0;
            options.dwMaxMessages = 20;
            options.cbMaxMessage = 10000;
            options.bReadAccess = true;
            options.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(options);

            ptr = CreateMsgQueue("CoreCfPwrMsgQ", ref options);

            RequestPowerNotifications(ptr, 0xFFFFFFFF);

            t = new Thread(new ThreadStart(DoWork));
            t.IsBackground = true;
        }

        public void Start()
        {
            t.Start();
        }

        public void Stop()
        {
            done = true;
            t.Join();
        }

        private void DoWork()
        {
            byte[] buf = new byte[10000];
            uint nRead = 0, flags = 0, res = 0;

            // Console.WriteLine("starting loop");
            while (!done)
            {
                res = WaitForSingleObject(ptr, 1000);

                if (res == 0)
                {
                    ReadMsgQueue(ptr, buf, (uint)buf.Length, ref nRead, -1, ref flags);
                    //Console.WriteLine("message: " + ConvertByteArray(buf, 0) + " flag: " + ConvertByteArray(buf, 4));

                    uint flag = BitConverter.ToUInt32(buf, 4);
                    string msg = null;

                    switch (flag)
                    {
                        case 65536:
                            msg = "Power On";
                            break;
                        case 131072:
                            msg = "Power Off";
                            break;
                        case 262144:
                            msg = "Power Critical";
                            break;
                        case 524288:
                            msg = "Power Boot";
                            break;
                        case 1048576:
                            msg = "Power Idle";
                            break;
                        case 2097152:
                            msg = "Power Suspend";
                            break;
                        case 8388608:
                            msg = "Power Reset";
                            break;
                        case 0:
                            // non power transition messages are ignored
                            break;
                        default:
                            msg = "Unknown Flag: " + flag;
                            break;
                    }

                    //if (msg != null)
                    //    Console.WriteLine(msg);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MsgQOptions
        {
            public uint dwSize;
            public uint dwFlags;
            public uint dwMaxMessages;
            public uint cbMaxMessage;
            public bool bReadAccess;
        }
    }
#endif
}
