using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] Clone(this byte[] data, int offset, int count)
        {
            byte[] d = new byte[count];
            Buffer.BlockCopy(data, 0, d, 0, count);
            return d;
        }
    }
}
