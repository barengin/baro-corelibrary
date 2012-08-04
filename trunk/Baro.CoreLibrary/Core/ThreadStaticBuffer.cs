using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.Core
{
#if PocketPC || WindowsCE
    // ThreadSafeRandom CF üzerinde desteklenmiyor!
#else
    public static class ThreadStaticBuffer
    {
        [ThreadStatic]
        private static byte[] s_buffer;

        public static byte[] Get
        {
            get
            {
                return s_buffer ?? (s_buffer = new byte[2048]);
            }
        }

        public static byte[] Clone(int offset, int count)
        {
            byte[] d = new byte[count];
            Buffer.BlockCopy(Get, offset, d, 0, count);
            return d;
        }

        public static byte[] Resize(int newSize)
        {
            if (Get.Length < newSize)
            {
                byte[] tmp = Get;
                Array.Resize<byte>(ref tmp, newSize);
                s_buffer = tmp;
            }

            return Get;
        }
    }
#endif
}
