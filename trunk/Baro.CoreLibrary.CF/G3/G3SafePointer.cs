using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.G3
{
    public unsafe struct G3SafePointer
    {
        private readonly ushort* m_pnt;
        private readonly int m_size;

        public G3SafePointer(ushort* pointer, int size)
        {
            m_pnt = pointer;
            m_size = size;
        }

        public ushort this[int index]
        {
            get
            {
                if (index < 0 || index >= m_size)
                    throw new IndexOutOfRangeException("Index out of range exception Pointer");

                return m_pnt[index];
            }
        }
    }
}
