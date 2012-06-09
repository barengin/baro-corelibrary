using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.Collections
{
    public sealed class ArraySegmentSeamlessBuffer<T>
    {
        private int m_bufferSize = 0;
        private object m_synch = new object();
        private List<ArraySegment<T>> l_data = null;

        private List<ArraySegment<T>> lazyData
        {
            get
            {
#if PocketPC || WindowsCE
                if (l_data == null)
                {
                    lock (m_synch)
                    {
                        if (l_data == null)
                        {
                            l_data = new List<ArraySegment<T>>();
                        }
                    }
                }

                return l_data;
#else
                return LazyInitializer.EnsureInitialized<List<ArraySegment<T>>>(ref l_data, () => (new List<ArraySegment<T>>()));
#endif
            }
        }

        public int BufferSize
        {
            get
            {
                lock (m_synch)
                {
                    return m_bufferSize;
                }
            }
        }

        /// <summary>
        /// DEBUG ONLY !!! Really slow !!!
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                lock (m_synch)
                {
                    if (index < 0 || index >= m_bufferSize)
                        throw new ArgumentOutOfRangeException("index");

                    for (int k = 0; k < lazyData.Count; k++)
                    {
                        int size = lazyData[k].Count;

                        if (index < size)
                        {
                            ArraySegment<T> aseg = lazyData[k];
                            return aseg.Array[aseg.Offset + index];
                        }
                        else
                        {
                            index -= size;
                        }
                    }

                    throw new InvalidOperationException("you shouldn't see this message");
                }
            }
        }

        public void RemoveFromStart(int count)
        {
            if (count < 0 || count > m_bufferSize)
                throw new ArgumentOutOfRangeException("count");

            lock (m_synch)
            {
                while (count > 0 && lazyData.Count > 0)
                {
                    ArraySegment<T> source = lazyData[0];

                    // Remove first data from pos
                    if (count >= source.Count)
                    {
                        lazyData.RemoveAt(0);
                        count -= source.Count;
                        m_bufferSize -= source.Count;
                        continue;
                    }
                    else
                    // Remove partial
                    {
                        ArraySegment<T> d = new ArraySegment<T>(source.Array, source.Offset + count, source.Count - count);

                        lazyData.RemoveAt(0);
                        m_bufferSize -= source.Count;

                        lazyData.Insert(0, d);
                        m_bufferSize += d.Count;

                        return;
                    }
                }
            }
        }

        public void CopyTo(T[] destination, int destinationOffset, int sourceIndex, int sourceCount)
        {
            if (sourceIndex < 0 || sourceIndex >= m_bufferSize)
                throw new ArgumentOutOfRangeException("sourceIndex");

            if (sourceIndex + sourceCount > m_bufferSize)
                throw new ArgumentOutOfRangeException("sourceIndex & sourceCount", "İki değerin toplamı BufferSize() aşamaz");

            lock (m_synch)
            {
                int pos = -1;

                for (int k = 0; k < lazyData.Count; k++)
                {
                    int size = lazyData[k].Count;

                    if (sourceIndex < size)
                    {
                        pos = k;
                        break;
                    }
                    else
                    {
                        sourceIndex -= size;
                    }
                }

                while (sourceCount > 0)
                {
                    ArraySegment<T> source = lazyData[pos];

                    int newSize = Math.Min(sourceCount, source.Count - sourceIndex);
                    sourceCount -= newSize;

                    Buffer.BlockCopy(source.Array, source.Offset + sourceIndex, destination, destinationOffset, newSize);
                    destinationOffset += newSize;

                    pos++;
                    sourceIndex = 0;
                }
            }
        }

        public void Clear()
        {
            lock (m_synch)
            {
                m_bufferSize = 0;
                lazyData.Clear();
            }
        }

        public void Add(ArraySegment<T> data)
        {
            if (data.Count != 0)
            {
                lock (m_synch)
                {
                    lazyData.Add(data);
                    m_bufferSize += data.Count;
                }
            }
        }
    }
}
