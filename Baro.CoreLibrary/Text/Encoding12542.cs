using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Text
{
    /// <summary>
    /// Eski Yolbil 3 serisinin kullandığı encoding kütüphanesi
    /// </summary>
    internal sealed unsafe class Encoding12542 : Encoding
    {
        struct Char1254
        {
            public char Char;
            public byte Order;
        }

        private Char1254[] CharTable = new Char1254[256];

        #region Build char table
        private void UpdateOrder(char c, byte order)
        {
            for (int k = 0; k < 256; k++)
                if (CharTable[k].Char == c)
                {
                    CharTable[k].Order = order;
                    return;
                }
        }

        private void MakeOrder1254()
        {
            UpdateOrder('\u0020', 20);

            UpdateOrder('0', 30);
            UpdateOrder('1', 31);
            UpdateOrder('2', 32);
            UpdateOrder('3', 33);
            UpdateOrder('4', 34);
            UpdateOrder('5', 35);
            UpdateOrder('6', 36);
            UpdateOrder('7', 37);
            UpdateOrder('8', 38);
            UpdateOrder('9', 39);

            UpdateOrder('(', 40);
            UpdateOrder(')', 41);

            UpdateOrder('-', 42);
            UpdateOrder('/', 43);
            UpdateOrder('\\', 44);

            UpdateOrder('a', 53);
            UpdateOrder('b', 54);
            UpdateOrder('c', 55);
            UpdateOrder('ç', 56);
            UpdateOrder('d', 57);
            UpdateOrder('e', 58);
            UpdateOrder('f', 59);
            UpdateOrder('g', 60);
            UpdateOrder('ğ', 61);
            UpdateOrder('h', 62);
            UpdateOrder('ı', 63);
            UpdateOrder('i', 64);
            UpdateOrder('j', 65);
            UpdateOrder('k', 66);
            UpdateOrder('l', 67);
            UpdateOrder('m', 68);
            UpdateOrder('n', 69);
            UpdateOrder('o', 70);
            UpdateOrder('ö', 71);
            UpdateOrder('p', 72);
            UpdateOrder('q', 73);
            UpdateOrder('r', 74);
            UpdateOrder('s', 75);
            UpdateOrder('ş', 76);
            UpdateOrder('t', 77);
            UpdateOrder('u', 78);
            UpdateOrder('ü', 79);
            UpdateOrder('v', 80);
            UpdateOrder('w', 81);
            UpdateOrder('x', 82);
            UpdateOrder('y', 83);
            UpdateOrder('z', 84);

            UpdateOrder('A', 53);
            UpdateOrder('B', 54);
            UpdateOrder('C', 55);
            UpdateOrder('Ç', 56);
            UpdateOrder('D', 57);
            UpdateOrder('E', 58);
            UpdateOrder('F', 59);
            UpdateOrder('G', 60);
            UpdateOrder('Ğ', 61);
            UpdateOrder('H', 62);
            UpdateOrder('I', 63);
            UpdateOrder('İ', 64);
            UpdateOrder('J', 65);
            UpdateOrder('K', 66);
            UpdateOrder('L', 67);
            UpdateOrder('M', 68);
            UpdateOrder('N', 69);
            UpdateOrder('O', 70);
            UpdateOrder('Ö', 71);
            UpdateOrder('P', 72);
            UpdateOrder('Q', 73);
            UpdateOrder('R', 74);
            UpdateOrder('S', 75);
            UpdateOrder('Ş', 76);
            UpdateOrder('T', 77);
            UpdateOrder('U', 78);
            UpdateOrder('Ü', 79);
            UpdateOrder('V', 80);
            UpdateOrder('W', 81);
            UpdateOrder('X', 82);
            UpdateOrder('Y', 83);
            UpdateOrder('Z', 84);
        }

        private void Make1254Table()
        {
            for (int k = 0; k < 256; k++)
            {
                CharTable[k].Char = (char)k;
                CharTable[k].Order = 255;
            }

            CharTable[0xD0].Char = 'Ğ';
            CharTable[0xDD].Char = 'İ';
            CharTable[0xDE].Char = 'Ş';

            CharTable[0xF0].Char = 'ğ';
            CharTable[0xFD].Char = 'ı';
            CharTable[0xFE].Char = 'ş';

            CharTable[0xFF].Char = '\u25cf'; // Black circle

            MakeOrder1254();
        }

        #endregion

        public Encoding12542()
        {
            Make1254Table();
        }

        public override char this[byte index]
        {
            get { throw new NotImplementedException(); }
        }

        private byte GetByteOrder1254(char c)
        {
            return CharTable[this[c]].Order;
        }

        public override byte this[char index]
        {
            get
            {
                int t = (int)index;

                if (t > 0x100 && t < 0x25D0)
                {
                    if (t == 0x11E)
                        return 0xD0;
                    else if (t == 0x11F)
                        return 0xF0;
                    else if (t == 0x130)
                        return 0xDD;
                    else if (t == 0x131)
                        return 0xFD;
                    else if (t == 0x15E)
                        return 0xDE;
                    else if (t == 0x15F)
                        return 0xFE;
                    else if (t == 0x25CF)
                        return 0xFF;
                }

                return (byte)t;
            }
        }

        public override string GetString(byte* t, int size)
        {
            StringBuilder s = new StringBuilder();

            while (size-- != 0)
                s.Append(CharTable[*t++].Char);

            return s.ToString();
        }

        public override string GetString(byte[] t)
        {
            if (t == null) return null;
            
            StringBuilder s = new StringBuilder();

            for (int k = 0; k < t.Length; k++)
                s.Append(CharTable[t[k]].Char);

            return s.ToString();
        }

        public override string GetString(byte[] t, int size)
        {
            throw new NotImplementedException();
        }

        public override byte[] GetBytes(string s)
        {
            if (s == null) return null;

            int l = s.Length;

            byte[] b = new byte[l];

            for (int k = 0; k < l; k++)
            {
                int t = (int)s[k];

                if (t > 0x100 && t < 0x25D0)
                {
                    if (t == 0x11E)
                        b[k] = 0xD0;
                    else if (t == 0x11F)
                        b[k] = 0xF0;
                    else if (t == 0x130)
                        b[k] = 0xDD;
                    else if (t == 0x131)
                        b[k] = 0xFD;
                    else if (t == 0x15E)
                        b[k] = 0xDE;
                    else if (t == 0x15F)
                        b[k] = 0xFE;
                    else if (t == 0x25cf)
                        b[k] = 0xFF;
                }
                else
                {
                    b[k] = (byte)t;
                }
            }

            return b;
        }

        public override void GetBytesInto(string t, byte* buffer, int bufferSize)
        {
            throw new NotImplementedException();
        }

        public override void GetBytesInto(string t, byte[] buffer, int bufferSize)
        {
            throw new NotImplementedException();
        }

        public override int Compare(string x, string y)
        {
            try
            {
                int minlen = (x.Length < y.Length) ? x.Length : y.Length;

                for (int k = 0; k < minlen; k++)
                {
                    int xa = GetByteOrder1254(x[k]) - GetByteOrder1254(y[k]);

                    if (xa < 0)
                    {
                        return -1;
                    }
                    else if (xa > 0)
                    {
                        return 1;
                    }
                }

                if (x.Length == y.Length)
                {
                    return 0;
                }
                else
                {
                    if (x.Length < y.Length)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            catch
            {
                throw new Exception("Hatalı karakter: '" + x + "' === '" + y + "'");
            }
        }

        public override int Compare(byte* x, int sizex, byte* y, int sizey)
        {
            try
            {
                int minlen = (sizex < sizey) ? sizex : sizey;

                for (int k = 0; k < minlen; k++)
                {
                    int xa = CharTable[x[k]].Order - CharTable[y[k]].Order;

                    if (xa < 0)
                    {
                        return -1;
                    }
                    else if (xa > 0)
                    {
                        return 1;
                    }
                }

                if (sizex == sizey)
                {
                    return 0;
                }
                else
                {
                    if (sizex < sizey)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            catch
            {
                throw new Exception("Hatalı karakter");
            }
        }

        public override int CodePage
        {
            get { return 12542; }
        }
    }
}
