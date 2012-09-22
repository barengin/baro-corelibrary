using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.Core
{
    /// <summary>
    /// A modern random number generator based on G. Marsaglia: 
    /// Seeds for Random Number Generators, Communications of the
    /// ACM 46, 5 (May 2003) 90-93; and a posting by Marsaglia to 
    /// comp.lang.c on 2003-04-03.
    /// </summary>
    public class RandomC5
    {
        private uint[] Q = new uint[16];
        private uint c = 362436, i = 15;

        private uint Cmwc()
        {
            ulong t, a = 487198574UL;
            uint x, r = 0xfffffffe;

            i = (i + 1) & 15;
            t = a * Q[i] + c;
            c = (uint)(t >> 32);
            x = (uint)(t + c);
            if (x < c)
            {
                x++;
                c++;
            }

            return Q[i] = r - x;
        }


        /// <summary>
        /// Get a new random System.Int32 value
        /// </summary>
        /// <returns>The random int</returns>
        public int Next()
        {
            return (int)Cmwc();
        }

        /// <summary>
        /// Fill a array of byte with random bytes
        /// </summary>
        /// <param name="buffer">The array to fill</param>
        public void NextBytes(byte[] buffer)
        {
            for (int i = 0, length = buffer.Length; i < length; i++)
                buffer[i] = (byte)Cmwc();
        }


        /// <summary>
        /// Create a random number generator seed by system time.
        /// </summary>
        public RandomC5()
            : this(DateTime.Now.Ticks)
        {
        }


        /// <summary>
        /// Create a random number generator with a given seed
        /// </summary>
        /// <exception cref="ArgumentException">If seed is zero</exception>
        /// <param name="seed">The seed</param>
        public RandomC5(long seed)
        {
            if (seed == 0)
                throw new ArgumentException("Seed must be non-zero");

            uint j = (uint)(seed & 0xFFFFFFFF);

            for (int i = 0; i < 16; i++)
            {
                j ^= j << 13;
                j ^= j >> 17;
                j ^= j << 5;
                Q[i] = j;
            }

            Q[15] = (uint)(seed ^ (seed >> 32));
        }
    }
}
