using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Baro.CoreLibrary.Spatial
{
    internal enum NodeType
    {
        Directory,
        Container
    };

    /// <summary>
    /// MBR deklarasyonu (Minimum Boundry Rectangle)
    /// </summary>
    public struct Mbr
    {
        /// <summary>
        /// MBR Deðerleri
        /// </summary>
        public int minx, miny, maxx, maxy;

        /// <summary>
        /// Geniþlik
        /// </summary>
        public int Width { get { return maxx - minx; } }

        /// <summary>
        /// Yükseklik
        /// </summary>
        public int Height { get { return maxy - miny; } }

        /// <summary>
        /// Orta nokta
        /// </summary>
        public Point GetCenter { get { return new Point((minx + maxx) / 2, (miny + maxy) / 2); } }

        /// <summary>
        /// 
        /// </summary>
        public Rectangle GetRectangle { get { return new Rectangle(minx, miny, maxx - minx, maxy - miny); } }

        /// <summary>
        /// cTor
        /// </summary>
        /// <param name="minx"></param>
        /// <param name="miny"></param>
        /// <param name="maxx"></param>
        /// <param name="maxy"></param>
        public Mbr(int minx, int miny, int maxx, int maxy)
        {
            this.maxx = maxx;
            this.maxy = maxy;
            this.minx = minx;
            this.miny = miny;
        }

        /// <summary>
        /// MBR Intersects
        /// </summary>
        /// <param name="r1minx"></param>
        /// <param name="r1maxx"></param>
        /// <param name="r1miny"></param>
        /// <param name="r1maxy"></param>
        /// <param name="r2minx"></param>
        /// <param name="r2maxx"></param>
        /// <param name="r2miny"></param>
        /// <param name="r2maxy"></param>
        /// <returns></returns>
        public static bool Intersects(int r1minx, int r1maxx, int r1miny, int r1maxy,
            int r2minx, int r2maxx, int r2miny, int r2maxy)
        {
            if (r1minx > r2minx) r2minx = r1minx;
            if (r1miny > r2miny) r2miny = r1miny;
            if (r1maxx < r2maxx) r2maxx = r1maxx;
            if (r1maxy < r2maxy) r2maxy = r1maxy;

            return (r2maxx > r2minx) && (r2maxy > r2miny);
        }

        /// <summary>
        /// MBR alanýný verir.
        /// </summary>
        /// <returns>Metre kare cinsinden.</returns>
        public long Area()
        {
            return Math.Abs((maxx - minx) * (maxy - miny));
        }

        /// <summary>
        /// Ýki MBR'ýn birleþimini verir.
        /// </summary>
        /// <param name="m">Birleþtirilecek ikinci MBR</param>
        /// <returns>Sonuç yeni bir MBR'dýr.</returns>
        public Mbr Union(Mbr m)
        {
            Mbr r = this;

            if (m.minx < minx) r.minx = m.minx;
            if (m.miny < miny) r.miny = m.miny;
            if (m.maxx > maxx) r.maxx = m.maxx;
            if (m.maxy > maxy) r.maxy = m.maxy;

            return r;
        }

        /// <summary>
        /// Ýki MBR'ýn ayný olup olmadýðýný kontrol eder.
        /// </summary>
        /// <param name="m">Kontrol edilecek ikinci MBR</param>
        /// <returns>Ayný ise TRUE döner.</returns>
        public bool IsSame(Mbr m)
        {
            if (minx != m.minx) { return false; }
            if (maxx != m.maxx) { return false; }
            if (miny != m.miny) { return false; }
            if (maxy != m.maxy) { return false; }

            return true;
        }

        /// <summary>
        /// User defined hash code
        /// </summary>
        /// <returns>hash code int32</returns>
        public override int GetHashCode()
        {
            return minx ^ miny ^ maxx ^ maxy;
        }

        /// <summary>
        /// Eþitlik
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static bool operator ==(Mbr m1, Mbr m2)
        {
            return m1.Equals(m2);
        }

        /// <summary>
        /// Eþit deðil
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static bool operator !=(Mbr m1, Mbr m2)
        {
            return !m1.Equals(m2);
        }

        /// <summary>
        /// Eþitlik
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return IsSame((Mbr)obj);
        }

        /// <summary>
        /// Bir noktanýn MBR içinde olup olmadýðýný kontrol eder.
        /// </summary>
        /// <param name="X">X</param>
        /// <param name="Y">Y</param>
        /// <returns>Ýçinde ise TRUE döner</returns>
        public bool PointInMbr(int X, int Y)
        {
            return (X >= minx) && (X < maxx) && (Y >= miny) && (Y < maxy);
        }

        /// <summary>
        /// Ýki MBR'in kesiþip kesiþmediðini kontrol eder.
        /// </summary>
        /// <param name="m">Ýkinci MBR</param>
        /// <returns>Kesiþiyor ise TRUE döner</returns>
        public bool IntersectsMbr(Mbr m)
        {
            int Rminx = minx, Rmaxx = maxx, Rmaxy = maxy, Rminy = miny;

            if (m.minx > Rminx) Rminx = m.minx;
            if (m.miny > Rminy) Rminy = m.miny;
            if (m.maxx < Rmaxx) Rmaxx = m.maxx;
            if (m.maxy < Rmaxy) Rmaxy = m.maxy;

            return (Rmaxx > Rminx) && (Rmaxy > Rminy);
        }
    }

    internal struct NodeItem
    {
        public object Item;
        public Mbr Mbr;

        public NodeItem(Mbr m, object l)
        {
            this.Mbr = m;
            this.Item = l;
        }
    }

    internal sealed class Node
    {
        public Node Parent;
        public NodeType Type;
        public Mbr Mbr;

        public List<NodeItem> Items = new List<NodeItem>();

        public void Add(Mbr M, object Link)
        {
            Items.Add(new NodeItem(M, Link));

            if (Items.Count == 1)
            {
                Mbr = M;
            }
            else
            {
                Mbr = Mbr.Union(M);
            }
        }

        public Node(NodeType type)
        {
            Type = type;
        }
    }
}
