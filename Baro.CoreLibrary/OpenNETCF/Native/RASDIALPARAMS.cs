using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    internal class RASDIALPARAMS
    {
        //        _RASDIALPARAMS 
        //        { 
        // 004     DWORD dwSize; 
        // 042     TCHAR szEntryName[RAS_MaxEntryName + 1]; 
        // 258     TCHAR szPhoneNumber[RAS_MaxPhoneNumber + 1]; 
        // 098     TCHAR szCallbackNumber[RAS_MaxCallbackNumber + 1]; 
        // 514     TCHAR szUserName[UNLEN + 1]; 
        // 514     TCHAR szPassword[PWLEN + 1]; 
        // 032     TCHAR szDomain[DNLEN + 1] ; 
        //        } 

        // note: total is 1462, but that's not DWORD aligned, so we pad out to 1464
        private const int Size = 1464;

        const int RAS_MaxDeviceName = 128; //32
        const int RAS_MaxDeviceType = 16;
        const int RAS_MaxAreaCode = 11;
        const int RAS_MaxEntryName = 20;
        const int RAS_MaxPhoneNumber = 128;
        const int RAS_MaxCallbackPhoneNumber = 48;
        const int ULEN = 256;
        const int DNLEN = 15;
        const int RASIPADDR = 4;

        const int SizeOffset = 0;
        const int EntryNameOffset = 4;
        const int PhoneNumberOffset = EntryNameOffset + (RAS_MaxEntryName + 1) * 2;
        const int CallbackNumberOffset = PhoneNumberOffset + (RAS_MaxPhoneNumber + 1) * 2;
        const int UserNameOffset = CallbackNumberOffset + (RAS_MaxCallbackPhoneNumber + 1) * 2;
        const int PasswordOffset = UserNameOffset + (ULEN + 1) * 2;
        const int DomainOffset = PasswordOffset + (ULEN + 1) * 2;

        private byte[] data;


        public byte[] Data { get { return data; } }

        public RASDIALPARAMS()
        {
            data = new byte[Size];
            dwSize = Size;

            szEntryName 
                = szPhoneNumber 
                = szCallbackNumber 
                = szUserName
                = szPassword
                = szDomain
                = string.Empty;
        }

        public int dwSize
        {
            get { return BitConverter.ToInt32(data, 0); }
            set { BitConverter.GetBytes(value).CopyTo(data, 0); }
        }

        public string szEntryName
        {
            get { return Encoding.Unicode.GetString(data, EntryNameOffset, RAS_MaxEntryName * 2).TrimEnd('\0'); }
            set { Encoding.Unicode.GetBytes(value + '\0').CopyTo(data, EntryNameOffset); }
        }

        public string szPhoneNumber
        {
            get { return Encoding.Unicode.GetString(data, PhoneNumberOffset, RAS_MaxPhoneNumber * 2).TrimEnd('\0'); }
            set { Encoding.Unicode.GetBytes(value + '\0').CopyTo(data, PhoneNumberOffset); }
        }

        public string szCallbackNumber
        {
            get { return Encoding.Unicode.GetString(data, CallbackNumberOffset, RAS_MaxEntryName * 2).TrimEnd('\0'); }
            set { Encoding.Unicode.GetBytes(value + '\0').CopyTo(data, CallbackNumberOffset); }
        }

        public string szUserName
        {
            get { return Encoding.Unicode.GetString(data, UserNameOffset, ULEN * 2).TrimEnd('\0'); }
            set { Encoding.Unicode.GetBytes(value + '\0').CopyTo(data, UserNameOffset); }
        }

        public string szPassword
        {
            get { return Encoding.Unicode.GetString(data, PasswordOffset, ULEN * 2).TrimEnd('\0'); }
            set { Encoding.Unicode.GetBytes(value + '\0').CopyTo(data, PasswordOffset); }
        }

        public string szDomain
        {
            get {return Encoding.Unicode.GetString(data, DomainOffset, DNLEN * 2).TrimEnd('\0'); }
            set { Encoding.Unicode.GetBytes(value + '\0').CopyTo(data, DomainOffset); }
        }
    }
}
