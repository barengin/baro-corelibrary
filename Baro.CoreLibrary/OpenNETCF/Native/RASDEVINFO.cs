using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    internal class RASDEVINFO
    {
        //		DWORD       dwSize;
        //		WCHAR       szDeviceType[RAS_MaxDeviceType+1];
        //		WCHAR       szDeviceName[RAS_MaxDeviceName+1];

        private byte[] data;
        const int RAS_MaxDeviceName = 32;
        const int RAS_MaxDeviceType = 16;
        const int dwSizeOff = 0;
        const int szDeviceTypeOff = 4;
        const int szDeviceNameOff = szDeviceTypeOff + (RAS_MaxDeviceType * 2) + 2;

        public static RASDEVINFO FromData(byte[] Data, int offset)
        {
            RASDEVINFO en = new RASDEVINFO();
            Buffer.BlockCopy(Data, offset, en.data, 0, en.data.Length);
            return en;
        }

        public RASDEVINFO(int AllocSize)
        {
            data = new byte[AllocSize];
            this.dwSize = GetSize();
        }

        public RASDEVINFO()
        {
            data = new byte[GetSize()];
            this.dwSize = GetSize();
        }

        public int dwSize
        {
            get { return BitConverter.ToInt32(data, 0); }
            set { BitConverter.GetBytes(value).CopyTo(data, 0); }
        }

        public string szDeviceType
        {
            get { return Encoding.Unicode.GetString(data, szDeviceTypeOff, RAS_MaxDeviceType * 2).TrimEnd('\0'); }
            set { Encoding.Unicode.GetBytes(value).CopyTo(data, szDeviceTypeOff); }
        }

        public string szDeviceName
        {
            get { return Encoding.Unicode.GetString(data, szDeviceNameOff, RAS_MaxDeviceName * 2).TrimEnd('\0'); }
            set { Encoding.Unicode.GetBytes(value).CopyTo(data, szDeviceNameOff); }
        }

        public byte[] Data { get { return data; } }

        public static int GetSize()
        {
            return 104;
        }

    }
}
