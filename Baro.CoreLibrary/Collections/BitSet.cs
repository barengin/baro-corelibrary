using System;
using System.Collections.Generic;

namespace Baro.CoreLibrary.Collections
{
    public sealed class BitArray
    {
        private int[] m;

        public BitArray(int length)
        {
            m = new int[(length / 32) + 1];
        }

        public bool this[int index]
        {
            get
            {
                return (m[index >> 5] & 1 << index) != 0;
            }
            set
            {
                if (value)
                {
                    m[index >> 5] |= 1 << index;
                }
                else
                {
                    m[index >> 5] &= ~(1 << index);
                }
            }
        }

        public void Clear()
        {
            int s = m.Length;
            m = new int[s];
        }
    }
}
