using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Collections
{
    public sealed unsafe class PQueueInt : IDisposable
    {
        int Capacity = 64;
        int queueCount = 0;

#if PocketPC || WindowsCE
        int* keys = null;
        int* vals = null;
#else
        int[] keys = null;
        int[] vals = null;
#endif

        private void SetCapacity(int size)
        {
            if (size == 0)
            {
#if DEBUG
                maxItem = 0;
#endif

#if PocketPC || WindowsCE
                Memory.HeapMemoryManager.instance.Free((byte*)keys);
                Memory.HeapMemoryManager.instance.Free((byte*)vals);
#else
                keys = null;
                vals = null;
#endif

            }
            else
            {
                Capacity = size;

#if PocketPC || WindowsCE
                keys = (int*)Memory.HeapMemoryManager.instance.ReAlloc((byte*)keys, size * 4);
                vals = (int*)Memory.HeapMemoryManager.instance.ReAlloc((byte*)vals, size * 4);
#else
                Array.Resize<int>(ref keys, size);
                Array.Resize<int>(ref vals, size);
#endif

            }
        }

        public PQueueInt(int Capacity)
        {
#if PocketPC || WindowsCE
            keys = (int*)Memory.HeapMemoryManager.instance.Alloc(Capacity * 4);
            vals = (int*)Memory.HeapMemoryManager.instance.Alloc(Capacity * 4);
#else
            keys = new int[Capacity];
            vals = new int[Capacity];
#endif

            this.Capacity = Capacity;
        }

        public int QueueCount
        {
            get
            {
                return queueCount;
            }
        }

        private void Grow()
        {
            Capacity += Capacity / 2;

            SetCapacity(Capacity);
        }

#if DEBUG
        int maxItem = 0;

        public int Max { get { return maxItem; } }
#endif

        /// <summary>
        /// Verilen Node'un Data dizisi içindeki durumuna göre pozisyonu update edilir.
        /// Data dizisindeki değeri sadece azalabilir!!!
        /// </summary>
        /// <param name="NodeId">Node ID</param>
        //public void DecreaseKey(int NodeId)
        //{
        //    int P = NodeId2Pos[NodeId];		// Pos in heap
        //    int NewValue = Data[NodeId];	// Value

        //    while ((P > 1) && (Data[Key[P / 2]] > NewValue))
        //    {
        //        Key[P] = Key[P / 2];
        //        NodeId2Pos[Key[P]] = P;
        //        P = P / 2;
        //    }

        //    Key[P] = NodeId;
        //    NodeId2Pos[NodeId] = P;
        //}

        public void Append(int Key, int Value)
        {
            int r = ++queueCount;

#if DEBUG
            if (queueCount >= maxItem)
                maxItem = queueCount;
#endif

            if (queueCount >= Capacity)
                Grow();

            while ((r > 1) && (vals[r >> 1] > Value))
            {
                keys[r] = keys[r >> 1];
                vals[r] = vals[r >> 1];
                r = r >> 1;
            }

            keys[r] = Key;
            vals[r] = Value;
        }

        public unsafe int ExtractMin()
        {
            int result = keys[1];			     // Seçim

            int k = keys[1] = keys[queueCount];
            int v = vals[1] = vals[queueCount--];

            int NodeNo = 1;

            while (true)
            {
                int Smaller = NodeNo << 1;       // Left child
                int r = Smaller | 1;             // Right child

                if (r <= queueCount)
                {
                    int dt_lf = vals[Smaller];
                    int dt_rh = vals[r];

                    if (dt_lf > dt_rh)
                    {
                        Smaller = r;
                        if (v <= dt_rh) { break; }
                    }
                    else
                    {
                        if (v <= dt_lf) { break; }
                    }
                }
                else
                {
                    if (Smaller <= queueCount)
                    {
                        if (v <= vals[Smaller]) { break; }
                    }
                    else
                    {
                        break;
                    }
                }

                keys[NodeNo] = keys[Smaller];
                vals[NodeNo] = vals[Smaller];
                NodeNo = Smaller;
            }

            keys[NodeNo] = k;
            vals[NodeNo] = v;
            return result;
        }

        #region IDisposable Members

        ~PQueueInt()
        {
            Dispose();
        }

        public void Dispose()
        {
            SetCapacity(0);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
