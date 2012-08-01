using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    internal class RASCONN
    {
        //		DWORD dwSize; 
        //		HRASCONN hrasconn; 
        //		TCHAR szEntryName[RAS_MaxEntryName + 1]; 
        const int RAS_MaxEntryName = 20;

        const int dwSizeOff = 0;
        const int hRasConnOff = 4;
        const int szEntryNameOff = 8;

        private byte[] data;

        public byte[] Data { get { return data; } }

        public static int GetSize()
        {
            return 8 + (RAS_MaxEntryName + 1) * 2 + 2;
        }

        public static RASCONN FromData(byte[] Data, int offset)
        {
            RASCONN en = new RASCONN();
            Buffer.BlockCopy(Data, offset, en.data, 0, en.data.Length);
            return en;
        }

        public int dwSize
        {
            get { return BitConverter.ToInt32(data, 0); }
            set { BitConverter.GetBytes(value).CopyTo(data, 0); }
        }

        public RASCONN()
        {
            data = new byte[GetSize()];
            this.dwSize = GetSize();
        }

        public RASCONN(int AllocSize)
        {
            data = new byte[AllocSize];
            this.dwSize = GetSize();
        }

        public int hRasConn
        {
            get { return BitConverter.ToInt32(data, hRasConnOff); }
            set { BitConverter.GetBytes(value).CopyTo(data, hRasConnOff); }
        }

        public string szEntryName
        {
            get { return Encoding.Unicode.GetString(data, szEntryNameOff, RAS_MaxEntryName * 2).TrimEnd('\0'); }
            set { Encoding.Unicode.GetBytes(value).CopyTo(data, szEntryNameOff); }
        }
    }
}
