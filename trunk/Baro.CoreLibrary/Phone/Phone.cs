using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Phone
{
#if PocketPC || WindowsCE
    public class Phone
    {
        // Methods
        private Phone()
        {
        }

        public static bool MakeCall(string destination)
        {
            return MakeCall(destination, false, null);
        }

        public static bool MakeCall(string destination, bool prompt)
        {
            return MakeCall(destination, prompt, null);
        }

        private static IntPtr StringToHGlobalUni(string s)
        {
            if (s == null)
            {
                return IntPtr.Zero;
            }

            int cb = (s.Length + 1) * 2;
            IntPtr destination = Marshal.AllocHGlobal(cb);
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(s);
            Marshal.Copy(bytes, 0, destination, bytes.Length);
            return destination;
        }

        public static bool MakeCall(string destination, bool prompt, string calledParty)
        {
            MakeCallInfo ppmci = new MakeCallInfo();
            ppmci.cbSize = 0x18;
            ppmci.pszDestAddress = StringToHGlobalUni(destination);
            if (calledParty != null)
            {
                ppmci.pszCalledParty = StringToHGlobalUni(calledParty);
            }
            if (prompt)
            {
                ppmci.dwFlags = CallFlags.PromptBeforeCalling;
            }
            else
            {
                ppmci.dwFlags = CallFlags.Default;
            }
            int num = PhoneMakeCall(ref ppmci);
            if (ppmci.pszDestAddress != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ppmci.pszDestAddress);
            }
            if (ppmci.pszCalledParty != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ppmci.pszCalledParty);
            }
            return (num == 0);
        }

        [DllImport("phone.dll", SetLastError = true)]
        private static extern int PhoneMakeCall(ref MakeCallInfo ppmci);

        // Nested Types
        private enum CallFlags
        {
            Default = 1,
            PromptBeforeCalling = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MakeCallInfo
        {
            public int cbSize;
            public CallFlags dwFlags;
            public IntPtr pszDestAddress;
            private IntPtr pszAppName;
            public IntPtr pszCalledParty;
            private IntPtr pszComment;
        }
    }
#endif
}
