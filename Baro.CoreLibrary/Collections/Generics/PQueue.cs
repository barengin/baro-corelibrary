using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Collections.Generics
{
    public class PQueue<TNode> where TNode: IPQueueNode
    {
        private TNode[] list;
        private int queueCount = 0;

        private void Grow()
        {
            TNode[] list2 = new TNode[(int)(list.Length * 1.25f)];
            list.CopyTo(list2, 0);
            list = list2;
        }

        public PQueue(int capacity)
        {
            list = new TNode[capacity + 1];
        }

        public int Count
        {
            get
            {
                return queueCount;
            }
        }

        public void Append(TNode item)
        {
            int r = ++queueCount;

            if (queueCount >= list.Length)
                Grow();

            TNode parent;

            while ((r > 1) && ((parent = list[r >> 1]).PQPriority > item.PQPriority))
            {
                parent.PQPosition = r;
                list[r] = parent;
                r = r >> 1;
            }

            item.PQPosition = r;
            list[r] = item;
        }

        public void DecreaseKey(TNode item)
        {
            int r = item.PQPosition;

            TNode parent;

            while ((r > 1) && ((parent = list[r >> 1]).PQPriority > item.PQPriority))
            {
                parent.PQPosition = r;
                list[r] = parent;
                r = r >> 1;
            }

            item.PQPosition = r;
            list[r] = item;
        }

        public void IncreaseKey(TNode item)
        {
            int NodeNo = item.PQPosition;
            int v = item.PQPriority;

            while (true)
            {
                int Smaller = NodeNo << 1;       // Left child
                int r = Smaller | 1;             // Right child

                if (r <= queueCount)
                {
                    int dt_lf = list[Smaller].PQPriority;
                    int dt_rh = list[r].PQPriority;

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
                        if (v <= list[Smaller].PQPriority) { break; }
                    }
                    else
                    {
                        break;
                    }
                }

                TNode t = list[Smaller];
                t.PQPosition = NodeNo;
                list[NodeNo] = t;
                
                NodeNo = Smaller;
            }

            item.PQPosition = NodeNo;
            list[NodeNo] = item;
        }

        public void Delete(TNode item)
        {
            int NodeNo = item.PQPosition;

            TNode k = list[NodeNo] = list[queueCount--];
            int v = k.PQPriority;

            while (true)
            {
                int Smaller = NodeNo << 1;       // Left child
                int r = Smaller | 1;             // Right child

                if (r <= queueCount)
                {
                    int dt_lf = list[Smaller].PQPriority;
                    int dt_rh = list[r].PQPriority;

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
                        if (v <= list[Smaller].PQPriority) { break; }
                    }
                    else
                    {
                        break;
                    }
                }

                TNode t = list[Smaller];
                t.PQPosition = NodeNo;
                list[NodeNo] = t;

                NodeNo = Smaller;
            }

            item.PQPosition = NodeNo;
            list[NodeNo] = item;
        }

        public TNode ExtractTop()
        {
            TNode result = list[1]; // Sonuç
            // result.PQPosition = -1;

            TNode k = list[1] = list[queueCount--];
            int v = k.PQPriority;

            int NodeNo = 1;

            while (true)
            {
                int Smaller = NodeNo << 1;       // Left child
                int r = Smaller | 1;             // Right child

                if (r <= queueCount)
                {
                    int dt_lf = list[Smaller].PQPriority;
                    int dt_rh = list[r].PQPriority;

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
                        if (v <= list[Smaller].PQPriority) { break; }
                    }
                    else
                    {
                        break;
                    }
                }

                TNode t = list[Smaller];
                t.PQPosition = NodeNo;
                list[NodeNo] = t;

                NodeNo = Smaller;
            }

            k.PQPosition = NodeNo;
            list[NodeNo] = k;

            return result;
        }

        public void ClearAll()
        {
            queueCount = 0;
            list = new TNode[list.Length];
        }
    }
}
