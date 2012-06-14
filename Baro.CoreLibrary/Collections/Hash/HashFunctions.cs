using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Collections.Hash
{
    public static class HashFunctions
    {
        public unsafe static ushort elf_hash16(byte* key, int len)
        {
            byte* p = key;
            uint h = 0, g, i;

            for (i = 0; i < len; i++)
            {
                h = (h << 4) + p[i];
                g = h & 0xf000;

                if (g != 0)
                    h ^= g >> 8;

                h &= ~g;
            }

            return (ushort)h;
        }

        public unsafe static uint elf_hash32(byte* key, int len)
        {
            byte* p = key;
            uint h = 0, g;
            uint i;

            for (i = 0; i < len; i++)
            {
                h = (h << 4) + p[i];
                g = h & 0xf0000000;

                if (g != 0)
                    h ^= g >> 24;

                h &= ~g;
            }

            return h;
        }

        public unsafe static uint SuperFastHash(byte[] data, int len)
        {
            if (len <= 0 || data == null) return 0;

            int hash = len, tmp;
            int rem;

            fixed (byte* dt = data)
            {
                byte* dd = dt;

                rem = (int)len & 3;
                len >>= 2;

                /* Main loop */
                for (; len > 0; len--)
                {
                    hash += (dd[0] + (dd[1] << 8));
                    tmp = ((dd[2] + (dd[3] << 8)) << 11) ^ hash;
                    hash = (hash << 16) ^ tmp;
                    dd += (4); // SizeOf(Uint16) * 2
                    hash += hash >> 11;
                }

                /* Handle end cases */
                switch (rem)
                {
                    case 3:
                        hash += (dd[0] + (dd[1] << 8));
                        hash ^= hash << 16;
                        hash ^= data[2] << 18;
                        hash += hash >> 11;
                        break;
                    case 2:
                        hash += (dd[0] + (dd[1] << 8));
                        hash ^= hash << 11;
                        hash += hash >> 17;
                        break;
                    case 1:
                        hash += dd[0];
                        hash ^= hash << 10;
                        hash += hash >> 1;
                        break;
                }

                /* Force "avalanching" of final 127 bits */
                hash ^= hash << 3;
                hash += hash >> 5;
                hash ^= hash << 4;
                hash += hash >> 17;
                hash ^= hash << 25;
                hash += hash >> 6;

                return (uint)hash;
            }
        }
    }
}
