using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    internal class RASENTRY
    {
        const int RAS_MaxDeviceName = 128;//32;
        const int RAS_MaxDeviceType = 16;
        const int RAS_MaxAreaCode = 11;
        const int RAS_MaxPhoneNumber = 128 + 1;
        const int RASIPADDR = 4;

        private byte[] data;
        const int dwSizeOff = 0;
        const int dwOptionsOff = 4;
        const int dwCountryIDOff = 8;
        const int dwCountryCodeOff = 12;
        const int szAreaCodeOff = 16;
        const int szLocalPhoneNumberOff = 38;
        const int ipaddrOff = 300;
        const int ipaddrDnsOff = 304;
        const int ipaddrDnsAltOff = 308;
        const int ipaddrWinsOff = 312;
        const int ipaddrWinsAltOff = 316;

        //		DWORD dwSize;
        //		DWORD dwfOptions;
        //		DWORD dwCountryID;
        //		DWORD dwCountryCode;
        //		TCHAR szAreaCode[ RAS_MaxAreaCode + 1 ];
        //		TCHAR szLocalPhoneNumber[ RAS_MaxPhoneNumber + 1 ];
        //		DWORD dwAlternatesOffset;
        //		RASIPADDR ipaddr;
        //		RASIPADDR ipaddrDns;
        //		RASIPADDR ipaddrDnsAlt;
        //		RASIPADDR ipaddrWins;
        //		RASIPADDR ipaddrWinsAlt;






        public byte[] Data { get { return data; } }

        public RASENTRY(int size)
        {
            data = new byte[size];
            dwSize = size;
        }

        public RASENTRY(byte[] entryData)
        {
            data = entryData;
            dwSize = entryData.Length;
        }

        public int dwSize
        {
            get { return BitConverter.ToInt32(data, 0); }
            set { BitConverter.GetBytes(value).CopyTo(data, 0); }
        }
        public string szDeviceType
        {
            get { return Encoding.Unicode.GetString(data, 1892, RAS_MaxDeviceType * 2).TrimEnd('\0'); }
            set { Encoding.Unicode.GetBytes(value).CopyTo(data, 1892); }
        }

        public string szDeviceName
        {
            get
            {
                //06/11/2004
                string device = Encoding.Unicode.GetString(data, 1926, RAS_MaxDeviceName * 2);
                return device.Substring(0, device.IndexOf('\0'));
            }
            set { Encoding.Unicode.GetBytes(value).CopyTo(data, 1926); }
        }

        public int dwOptions
        {
            get { return GetInt(data, dwOptionsOff); }
            set { SetInt(data, dwOptionsOff, value); }
        }

        public int dwCountryCode
        {
            get { return GetInt(data, dwCountryCodeOff); }
            set { SetInt(data, dwCountryCodeOff, value); }
        }

        public string szAreaCode
        {
            get { return GetString(data, szAreaCodeOff, RAS_MaxAreaCode); }
            set { SetString(data, szAreaCodeOff, value); }
        }

        public string szLocalPhoneNumber
        {
            get
            {
                return GetString(data, szLocalPhoneNumberOff, RAS_MaxPhoneNumber);
            }
            set
            {
                //this.dwOptions |= ConnectionOptions.
                SetString(data, szLocalPhoneNumberOff, value);
            }
        }

        public string IPAddress
        {
            get
            {
                return GetIPAddress(data, ipaddrOff);
            }
            set
            {
                if (value != "")
                {
                    SetIPAddress(data, ipaddrOff, value);
                }
            }
        }

        public string IPAddressDns
        {
            get
            {
                return GetIPAddress(data, ipaddrDnsOff);
            }
            set
            {
                if (value != "")
                {

                    SetIPAddress(data, ipaddrDnsOff, value);
                }
            }
        }

        public string IPAddressDnsAlt
        {
            get
            {
                return GetIPAddress(data, ipaddrDnsAltOff);
            }
            set
            {
                if (value != null && value != "")
                {
                    SetIPAddress(data, ipaddrDnsAltOff, value);
                }
            }
        }

        public string IPAddressWins
        {
            get
            {
                return GetIPAddress(data, ipaddrWinsOff);
            }
            set
            {
                if (value != null && value != "")
                {
                    SetIPAddress(data, ipaddrWinsOff, value);
                }
            }
        }

        public string IPAddressWinsAlt
        {
            get
            {
                return GetIPAddress(data, ipaddrWinsAltOff);
            }
            set
            {
                if (value != null && value != "")
                {
                    SetIPAddress(data, ipaddrWinsAltOff, value);
                }
            }
        }

        private static string GetIPAddress(byte[] aData, int Offset)
        {
            string ipaddress = "";
            byte[] ipaddr = new byte[4];
            Buffer.BlockCopy(aData, Offset, ipaddr, 0, 4);
            for (int i = 3; i > -1; i--)
            {
                ipaddress += Convert.ToDecimal(ipaddr[i]).ToString() + ".";
            }
            return ipaddress.TrimEnd('.');
        }

        private static void SetIPAddress(byte[] aData, int Offset, string value)
        {
            string[] addrArr = value.Split('.');
            byte[] ipaddr = new byte[4];
            int n = 0;
            for (int i = 3; i > -1; i--, n++)
            {
                ipaddr[n] = Convert.ToByte(addrArr[i]);
            }
            Buffer.BlockCopy(ipaddr, 0, aData, Offset, 4);


        }

        // utility:  get a uint from the byte array
        private static int GetInt(byte[] aData, int Offset)
        {
            return BitConverter.ToInt32(aData, Offset);
        }

        // utility:  set a uint int the byte array
        private static void SetInt(byte[] aData, int Offset, int Value)
        {
            byte[] buint = BitConverter.GetBytes(Value);
            Buffer.BlockCopy(buint, 0, aData, Offset, buint.Length);
        }

        // utility:  get a unicode string from the byte array
        private static string GetString(byte[] aData, int Offset, int Length)
        {
            //String sReturn =  Encoding.Unicode.GetString(aData, Offset, Length);
            String sReturn = Encoding.Unicode.GetString(aData, Offset, Length * 2).TrimEnd('\0');
            return sReturn;
        }

        // utility:  set a unicode string in the byte array
        private static void SetString(byte[] aData, int Offset, string Value)
        {
            byte[] arr = Encoding.Unicode.GetBytes(Value);
            Buffer.BlockCopy(arr, 0, aData, Offset, arr.Length);
        }

    }
}
