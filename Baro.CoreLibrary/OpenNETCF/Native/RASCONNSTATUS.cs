using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    internal class RASCONNSTATUS
    {
        //		DWORD        dwSize;
        //		RASCONNSTATE rasconnstate;
        //		DWORD        dwError;
        //		TCHAR        szDeviceType[ RAS_MaxDeviceType + 1 ];
        //		TCHAR        szDeviceName[ RAS_MaxDeviceName + 1 ];
        const int RAS_MaxDeviceName = 128;// 32;
        const int RAS_MaxDeviceType = 16;
        private byte[] data;

        const int dwSizeOff = 0;
        const int RasConnStateOff = 4;
        const int dwErrorOff = 8;
        const int szDeviceTypeOff = 12;
        const int szDeviceNameOff = szDeviceTypeOff + (RAS_MaxDeviceType + 1) * 2;
        const int size = szDeviceNameOff + (RAS_MaxDeviceName + 1) * 2;

        public RASCONNSTATUS()
        {
            data = new byte[size];
            dwSize = size;
        }
        public int dwSize
        {
            get { return BitConverter.ToInt32(data, 0); }
            set { BitConverter.GetBytes(value).CopyTo(data, 0); }
        }

        public int dwRasConnState
        {
            get { return BitConverter.ToInt32(data, RasConnStateOff); }
            set { BitConverter.GetBytes(value).CopyTo(data, RasConnStateOff); }
        }

        public int dwError
        {
            get { return BitConverter.ToInt32(data, dwErrorOff); }
            set { BitConverter.GetBytes(value).CopyTo(data, dwErrorOff); }
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

    }
}
