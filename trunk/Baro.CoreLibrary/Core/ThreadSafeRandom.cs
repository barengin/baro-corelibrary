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
        private static volatile Random s_global;

        private static Random global
        {
            get
            {
                return s_global ?? (s_global = new Random());
            }
        }

        public static int NextWithNeg()
        {
            lock (global) 
            {
                return global.Next(int.MinValue, int.MaxValue);
            }
        }

        public static int Next()
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
