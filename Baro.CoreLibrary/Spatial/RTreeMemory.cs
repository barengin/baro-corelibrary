using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Spatial
{
    /// <summary>
    /// Cosmetic katman üzerinde sorgulama yapýldýðýnda bu delege kullanýlýr.
    /// </summary>
    public delegate void RTreeMemoryCallBack(object tag);

    /// <summary>
    /// Cosmetic katman için RTree implementasyonu. Cosmetic katman ile ilgili iþler bu class tarafýndan
    /// yapýlýr.
    /// </summary>
    public sealed class RTreeMemory
    {
        private const int MAXOBJ = 30;
        private const int MINOBJ = MAXOBJ / 2;

        private Stack<Node> stack = new Stack<Node>(500);

        private Node RootNode = new Node(NodeType.Container);

        private Node FindAContainer(Mbr M)
        {
            Node N = RootNode;

            while (N.Type == NodeType.Directory)
            {
                object sel = N.Items[0].Item;
                long MinArea = N.Items[0].Mbr.Union(M).Area() - M.Area() - N.Items[0].Mbr.Area();

                for (int x = 1; x < N.Items.Count; x++)
                {
                    long min = N.Items[x].Mbr.Union(M).Area() - M.Area() - N.Items[x].Mbr.Area();
                    if (min < MinArea)
                    {
                        MinArea = min;
                        sel = N.Items[x].Item;
                    }
                }

                N = (Node)sel;
            }

            return N;
        }

        private void GetFarObjects(Node N, out int s1, out int s2)
        {
            s1 = 0; s2 = 1;
            long worst = 0;

            for (int x = 0; x < N.Items.Count; x++)
            {
                for (int y = 0; y < N.Items.Count; y++)
                {
                    Mbr J = N.Items[x].Mbr.Union(N.Items[y].Mbr);
                    long d1 = J.Area() - N.Items[x].Mbr.Area() - N.Items[y].Mbr.Area();
                    if (d1 > worst)
                    {
                        worst = d1;
                        s1 = x;
                        s2 = y;
                    }
                }
            }
        }

        private Node QSplit(Node N)
        {
            int s1, s2;
            GetFarObjects(N, out s1, out s2);

            Node G1 = new Node(N.Type);
            Node G2 = new Node(N.Type);

            //G1.Parent = N.Parent;
            //G2.Parent = N.Parent;

            G1.Add(N.Items[s1].Mbr, N.Items[s1].Item);
            G2.Add(N.Items[s2].Mbr, N.Items[s2].Item);

            if (s2 > s1)
            {
                N.Items.RemoveAt(s2);
                N.Items.RemoveAt(s1);
            }
            else
            {
                N.Items.RemoveAt(s1);
                N.Items.RemoveAt(s2);
            }

            ////////////////////////////////////
            while (N.Items.Count != 0)
            {
                long minarea = long.MaxValue;
                int bestentry = -1;
                int bestgroup = -1;

                for (int x = 0; x < N.Items.Count; x++)
                {
                    long d1 = G1.Mbr.Union(N.Items[x].Mbr).Area() - G1.Mbr.Area();
                    long d2 = G2.Mbr.Union(N.Items[x].Mbr).Area() - G2.Mbr.Area();

                    if (Math.Min(d1, d2) < minarea)
                    {
                        bestentry = x;
                        if (d1 > d2)
                        {
                            bestgroup = 2;
                            minarea = d2;
                        }
                        else
                        {
                            bestgroup = 1;
                            minarea = d1;
                        }
                    }
                }

                /////////////////
                //Debug.Assert(bestentry != -1, "dikkat");
                //Debug.Assert(bestgroup != -1, "dikkat");
                /////////////////

                if (bestgroup == 1)
                {
                    G1.Add(N.Items[bestentry].Mbr, N.Items[bestentry].Item);
                }
                else
                {
                    G2.Add(N.Items[bestentry].Mbr, N.Items[bestentry].Item);
                }

                N.Items.RemoveAt(bestentry);
                ///////////////////

                if ((G1.Items.Count + N.Items.Count) == MINOBJ)
                {
                    for (int x = 0; x < N.Items.Count; x++)
                    {
                        G1.Add(N.Items[x].Mbr, N.Items[x].Item);
                    }

                    N.Items.Clear();
                }

                if ((G2.Items.Count + N.Items.Count) == MINOBJ)
                {
                    for (int x = 0; x < N.Items.Count; x++)
                    {
                        G2.Add(N.Items[x].Mbr, N.Items[x].Item);
                    }

                    N.Items.Clear();
                }
                ////////////////////////
            }

            N.Items.Clear();
            for (int k = 0; k < G1.Items.Count; k++)
            {
                N.Add(G1.Items[k].Mbr, G1.Items[k].Item);
            }

            return G2;
        }

        private void SplitAndAdjust(Node n)
        {
            Node newnode = QSplit(n);

            // Alttaki kutudakilerin parent'larýný update etmek lazým.
            if (newnode.Type == NodeType.Directory)
            {
                for (int k = 0; k < newnode.Items.Count; k++)
                {
                    ((Node)newnode.Items[k].Item).Parent = newnode;
                }
            }
            //

            if (n.Parent == null)
            {
                CreateNewRoot(newnode, n);
            }
            else
            {
                Node p = n.Parent;
                p.Add(newnode.Mbr, newnode);
                newnode.Parent = p;

                AdjustEntry(p, n);

                if (p.Items.Count > MAXOBJ)
                {
                    SplitAndAdjust(p);
                }
                else
                {
                    AdjustPath(p);
                }
            }
        }

        private void CreateNewRoot(Node newnode1, Node newnode2)
        {
            RootNode = new Node(NodeType.Directory);

            RootNode.Add(newnode1.Mbr, newnode1);
            RootNode.Add(newnode2.Mbr, newnode2);

            newnode1.Parent = RootNode;
            newnode2.Parent = RootNode;
        }

        private bool AdjustEntry(Node parent, Node node)
        {
            for (int k = 0; k < parent.Items.Count; k++)
            {
                NodeItem i = parent.Items[k];

                if (node == i.Item)
                {
                    if (i.Mbr.IsSame(node.Mbr))
                    {
                        return false;
                    }
                    else
                    {
                        parent.Items.RemoveAt(k);
                        parent.Add(node.Mbr, node);
                        return true;
                    }
                }
            }

            System.Diagnostics.Debug.Fail("Adjust Path hatasý");
            return false;
        }

        private void AdjustPath(Node newnode)
        {
            Node p = newnode.Parent;

            if (p == null)
            {
                return;
            }

            if (AdjustEntry(p, newnode))
            {
                AdjustPath(p);
            }
        }

        /// <summary>
        /// Cosmetic katman içindeki herþeyi siler.
        /// </summary>
        public void Clear()
        {
            RootNode = new Node(NodeType.Container);
        }

        /// <summary>
        /// Cosmetic katmanýn içinde Rectangular Query yapar ve sonuçlarý Callback olarak bildirir.
        /// </summary>
        /// <param name="minx">Rectangle MinX</param>
        /// <param name="miny">Rectangle MinY</param>
        /// <param name="maxx">Rectangle MaxX</param>
        /// <param name="maxy">Rectangle MaxY</param>
        /// <param name="cb">Callback yapýlacak delege</param>
        public void QueryRect(int minx, int miny, int maxx, int maxy, RTreeMemoryCallBack cb)
        {
            Mbr mbr = new Mbr(minx, miny, maxx, maxy);

            stack.Clear();
            stack.Push(RootNode);

            while (stack.Count != 0)
            {
                Node n = stack.Pop();

                if (n.Mbr.IntersectsMbr(mbr))
                {
                    for (int i = 0; i < n.Items.Count; i++)
                    {
                        if (n.Items[i].Mbr.IntersectsMbr(mbr))
                        {
                            if (n.Type == NodeType.Directory)
                            {
                                stack.Push((Node)n.Items[i].Item);
                            }
                            else
                            {
                                cb(n.Items[i].Item);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cosmetic katmanýn içinde Rectangular Query yapar ve sonuçlarý Liste olarak döndürür.
        /// </summary>
        /// <param name="minx">Rectangle MinX</param>
        /// <param name="miny">Rectangle MinY</param>
        /// <param name="maxx">Rectangle MaxX</param>
        /// <param name="maxy">Rectangle MaxY</param>
        public List<object> QueryRect(int minx, int miny, int maxx, int maxy)
        {
            List<object> list = new List<object>();
            Mbr mbr = new Mbr(minx, miny, maxx, maxy);

            stack.Clear();
            stack.Push(RootNode);

            while (stack.Count != 0)
            {
                Node n = stack.Pop();

                if (n.Mbr.IntersectsMbr(mbr))
                {
                    for (int i = 0; i < n.Items.Count; i++)
                    {
                        if (n.Items[i].Mbr.IntersectsMbr(mbr))
                        {
                            if (n.Type == NodeType.Directory)
                            {
                                stack.Push((Node)n.Items[i].Item);
                            }
                            else
                            {
                                list.Add(n.Items[i].Item);
                            }
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Cosmetic katmanda çizilecek olan objenin MBR'ý ve RouteIndex + LineID karýþýmý
        /// </summary>
        /// <param name="M">MBR of object</param>
        /// <param name="id">sol 32 bit RouteIndex, sað 32 bit LineID</param>
        public void Insert(Mbr M, object tag)
        {
            Node N = FindAContainer(M);
            N.Add(M, tag);

            if (N.Items.Count > MAXOBJ)
            {
                SplitAndAdjust(N);
            }
            else
            {
                AdjustPath(N);
            }
        }
    }
}
