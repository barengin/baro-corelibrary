using System;

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Baro.CoreLibrary.Crc;
using System.IO;

namespace Baro.CoreLibrary.IO
{
#if PocketPC || WindowsCE
    public unsafe struct StorageID
    {
        private const int MANUFACTUREID_INVALID = 0x01;
        private const int SERIALNUM_INVALID = 0x02;

        private int dwSize;
        private int dwFlags;
        private int dwManufactureIDOffset;
        private int dwSerialNumOffset;

        public string Serial;
        public string Manufacturer;

        public uint GetSDCardId()
        {
            if (Serial != null)
            {
                byte[] S = System.Text.Encoding.ASCII.GetBytes(Serial);
                CRC32 crc = new CRC32();
                return crc.GetCrc32(S);
            }
            else
                return 0;
        }

        public StorageID(byte[] buffer)
        {
            dwSize = BitConverter.ToInt32(buffer, 0);
            dwFlags = BitConverter.ToInt32(buffer, 4);
            dwManufactureIDOffset = BitConverter.ToInt32(buffer, 8);
            dwSerialNumOffset = BitConverter.ToInt32(buffer, 12);

            if ((dwFlags & MANUFACTUREID_INVALID) == MANUFACTUREID_INVALID)
            {
                // ignore
            }

            if ((dwFlags & SERIALNUM_INVALID) == SERIALNUM_INVALID)
            {
                throw new Exception("SD Serial ID is invalid: Try another SD Card. Make sure this is a sd card.");
            }

            fixed (byte* B = buffer)
            {
                sbyte* S = (sbyte*)B;

                if (dwManufactureIDOffset == 0)
                {
                    Manufacturer = string.Empty;
                }
                else
                {
                    Manufacturer = new string(S + dwManufactureIDOffset);
                }

                if (dwSerialNumOffset == 0)
                {
                    Serial = string.Empty;
                }
                else
                {
                    Serial = new string(S + dwSerialNumOffset);
                }
            }

            int k = 0;
            string buf = string.Empty;
            bool rok = true;

            while (k < Serial.Length)
            {
                if (rok && Serial[k] == '0')
                {
                    //
                }
                else
                {
                    rok = false;
                    buf += Serial[k];
                }

                k++;
            }

            Serial = buf;
        }
    }

    public static class SDCardID
    {
        #region " consts "
        private const int ERROR_INSUFFICIENT_BUFFER = 122;
        private const int ERROR_INVALID_NAME = 123;
        private const uint GenericRead = 0x80000000;
        private const int GenericWrite = 0x40000000;
        private const int FileAttributeNormal = 0x80;
        private const int FileShareRead = 1;
        private const int FileShareWrite = 2;
        private const uint IOCTL_DISK_GET_STORAGEID = 0x71C24;
        private const int OpenExisting = 3;
        #endregion

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DEVMGR_DEVICE_INFORMATION
        {
            public Int32 dwSize;
            public IntPtr hDevice;
            public IntPtr hParentDevice;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string szLegacyName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDeviceKey;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szBusName;
        }

        private enum DeviceSearchType : int
        {
            DeviceSearchByLegacyName = 0,
            DeviceSearchByDeviceName = 1,
            DeviceSearchByBusName = 2,
            DeviceSearchByGuid = 3,
            DeviceSearchByParent = 4
        }

        #region " P/Invoke "
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        private static extern bool GetDeviceInformationByFileHandle(IntPtr handle, ref DEVMGR_DEVICE_INFORMATION pdi);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        private static extern bool DeviceIoControl(IntPtr deviceHandle, uint controlCode, byte[] inBuffer, int inBufferSize,
        [In][Out] byte[] outBuffer, int outBufferSize, ref int bytesReturned, ref IntPtr overlapped);

        [DllImport(SystemDLL.NAME)]
        private static extern IntPtr CreateFile(string FileName, uint DesiredAccess, int ShareMode, int SecurityAttributes,
            int CreationDisposition, int FlagsAndAttributes, int hTemplateFile);

        [DllImport(SystemDLL.NAME)]
        private static extern int CloseHandle(IntPtr hObject);
        #endregion

        private static int RequestStorageID(IntPtr handle, ref byte[] buffer)
        {
            int bufferSize = buffer.Length;
            int bytesReturned = 0;

            //DEVMGR_DEVICE_INFORMATION p = new DEVMGR_DEVICE_INFORMATION();
            //p.dwSize = Marshal.SizeOf(p);

            //GetDeviceInformationByFileHandle(handle, ref p);

            IntPtr overlap = IntPtr.Zero;
            bool result = DeviceIoControl(handle, IOCTL_DISK_GET_STORAGEID, null, 0, buffer, bufferSize, ref bytesReturned, ref overlap);

            if (!result)
                return 0;

            return bytesReturned;
        }

        public static StorageID GetSDID(string volume)
        {
            byte[] buffer = new byte[1024];

            int size = 0;
            IntPtr h = IntPtr.Zero;

            try
            {
                h = CreateFile(volume, GenericRead | GenericWrite, 0, 0, OpenExisting, 0, 0);
                
                if (h == IntPtr.Zero || h == (IntPtr)(-1))
                    throw new IOException("Error opening SDCard Volume. Check your SD card path and it's still in the drive.");

                size = RequestStorageID(h, ref buffer);
                
                if (size == 0)
                    throw new IOException("Error SDCard ID. Win32Error: " + Marshal.GetLastWin32Error().ToString());
            }
            finally
            {
                CloseHandle(h);
            }

            return new StorageID(buffer);
        }
    }
#endif
}
