using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Text
{
    public abstract unsafe class Encoding : IComparer<string>
    {
        public abstract char this[byte index]
        {
            get;
        }

        public abstract byte this[char index]
        {
            get;
        }

        public abstract string GetString(byte[] t);

        public abstract string GetString(byte[] t, int size);

        public abstract string GetString(byte* t, int size);

        public abstract byte[] GetBytes(string t);

        public abstract void GetBytesInto(string t, byte* buffer, int bufferSize);

        public abstract void GetBytesInto(string t, byte[] buffer, int bufferSize);

        public abstract int Compare(string x, string y);

        public abstract int Compare(byte* x, int sizex, byte* y, int sizey);

        public abstract int CodePage { get; }
    }
}
