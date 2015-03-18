using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    internal class RasEntryName : IDisposable
    {
        const int RAS_MaxEntryName = 20;
        private byte[] data;
        
        public static int GetSize()
        {
            return 4 + ((RAS_MaxEntryName + 1) * 2);
        }

        public RasEntryName()
        {
            data = new byte[GetSize()];
        }

        public static RasEntryName FromData(byte[] Data, int offset)
        {
            RasEntryName en = new RasEntryName();
            Buffer.BlockCopy(Data, offset, en.data, 0, en.data.Length);
            return en;
        }
        
        public int dwSize
        {
            get { return BitConverter.ToInt32(data, 0); }
            set { BitConverter.GetBytes(value).CopyTo(data, 0); }
        }
        
        public string szEntryName
        {
            get { return Encoding.Unicode.GetString(data, 4, RAS_MaxEntryName * 2).TrimEnd('\0'); }
            set { Encoding.Unicode.GetBytes(value).CopyTo(data, 4); }
        }

        public override string ToString() { return szEntryName; }
        
        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
