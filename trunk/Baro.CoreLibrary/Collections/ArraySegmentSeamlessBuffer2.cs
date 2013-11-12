using System;
using System.Collections.Generic;
// using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Collections
{
    public sealed class ArraySegmentSeamlessBuffer2<T>
    {
        private int m_bufferSize = 0;
        private LinkedList<ArraySegment<T>> list = new LinkedList<ArraySegment<T>>();

        public int BufferSize
        {
            get
            {
                return m_bufferSize;
            }
        }

        public void Clear()
        {
            m_bufferSize = 0;
            list.Clear();
        }

        public void Add(ArraySegment<T> data)
        {
            if (data.Count != 0)
            {
                list.AddLast(data);
                m_bufferSize += data.Count;
            }
        }

        public List<ArraySegment<T>> RemoveFromStart(int count)
        {
            if (count < 0 || count > m_bufferSize)
                throw new ArgumentOutOfRangeException("count");

            List<ArraySegment<T>> result = new List<ArraySegment<T>>();

            while (count > 0 && list.Count > 0)
            {
                ArraySegment<T> first = list.First.Value;

                // Remove first data from pos
                if (count >= first.Count)
                {
                    result.Add(first);
                    list.RemoveFirst();
                    count -= first.Count;
                    m_bufferSize -= first.Count;
                }
                else
                // Remove partial
                {
                    result.Add(new ArraySegment<T>(first.Array, first.Offset, count));

                    ArraySegment<T> d = new ArraySegment<T>(first.Array, first.Offset + count, first.Count - count);

                    list.RemoveFirst();
                    m_bufferSize -= first.Count;

                    list.AddFirst(d);
                    m_bufferSize += d.Count;

                    break;
                }
            }

            return result;
        }

    }
}
