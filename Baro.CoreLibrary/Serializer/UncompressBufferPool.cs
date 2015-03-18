using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.Serializer2
{
    /* === ZLib Yerine yeni bir kütüphane konulana kadar sistem Compression desteklemiyor. ===
        using System.Collections.Concurrent;

        internal class UncompressBufferPool        
        {
            private readonly ConcurrentStack<byte[]> m_pool = new ConcurrentStack<byte[]>();
            private readonly int m_bufferSize;

            public UncompressBufferPool(int poolSize, int bufferSize)
            {
                m_bufferSize = bufferSize;

                System.Threading.Tasks.Parallel.For(0, poolSize, (i) =>
                {
                    byte[] buf = new byte[bufferSize];
                    m_pool.Push(buf);
                });
            }

            public byte[] Pop()
            {
                byte[] buf;
                bool exists = m_pool.TryPop(out buf);

                return exists ? buf : new byte[m_bufferSize];
            }

            public void Push(byte[] buffer)
            {
                m_pool.Push(buffer);
            }
        }
     **/
}
