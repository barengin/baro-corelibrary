using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Baro.CoreLibrary.Core;

namespace Baro.CoreLibrary
{
#if PocketPC || WindowsCE
    public static class ThreadSafeRandom
    {
        private static volatile RandomC5 s_global;

        private static RandomC5 global
        {
            get { return s_global ?? (s_global = new RandomC5()); }
        }

        public static uint Next()
        {
            lock (global) 
            {
                return global.Next();
            }
        }
    }
#else
    public static class ThreadSafeRandom
    {
        private static volatile RandomC5 s_global;

        private static RandomC5 global
        {
            get { return s_global ?? (s_global = new RandomC5()); }
        }

        [ThreadStatic]
        private static RandomC5 _local;

        public static uint Next()
        {
            RandomC5 inst = _local;

            if (inst == null)
            {
                uint seed;
                lock (global) seed = global.Next();
                _local = inst = new RandomC5(seed);
            }

            return inst.Next();
        }
    }
#endif
}
