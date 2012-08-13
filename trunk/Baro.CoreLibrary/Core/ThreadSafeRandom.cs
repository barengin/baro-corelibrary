using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Aynı thread içerisinde kullanılabilir.
        /// Kesinlikle diğer methodlara parametre geçilmemeli.
        /// </summary>
        /// <returns></returns>
        public static Random ThreadSafeRandomObj()
        {
            Random inst = _local;

            if (inst == null)
            {
                int seed;
                lock (global) seed = global.Next();
                _local = inst = new Random(seed);
            }

            return inst;
        }

        public static int NextWithNeg()
        {
            Random inst = _local;

            if (inst == null)
            {
                int seed;
                lock (global) seed = global.Next();
                _local = inst = new Random(seed);
            }

            return inst.Next(int.MinValue, int.MaxValue);
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
