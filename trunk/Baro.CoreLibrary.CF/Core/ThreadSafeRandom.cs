using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary
{
#if PocketPC || WindowsCE
    // ThreadSafeRandom desteklenmiyor!
#else
    public static class ThreadSafeRandom
    {
        private static volatile Random s_global;

        private static Random global
        {
            get
            {
                return s_global ?? (s_global = new Random());
            }
        }

        [ThreadStatic]
        private static Random _local;

        public static void NextBytes(byte[] buffer)
        {
            Random inst = _local;

            if (inst == null)
            {
                int seed;
                lock (global) seed = global.Next();
                _local = inst = new Random(seed);
            }

            inst.NextBytes(buffer);
        }

        public static int Next()
        {
            Random inst = _local;

            if (inst == null)
            {
                int seed;
                lock (global) seed = global.Next();
                _local = inst = new Random(seed);
            }

            return inst.Next();
        }
    }
#endif
}
