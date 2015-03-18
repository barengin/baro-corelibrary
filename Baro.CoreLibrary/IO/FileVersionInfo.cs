using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.IO
{
    public sealed class FileVersionInfo
    {
        // Fields
        private string m_filename;
        private byte[] m_fixedversioninfo;

        // Methods
        private FileVersionInfo(string fileName)
        {
            this.m_filename = Path.GetFileName(fileName);
            int handle = 0;
            int cb = 0;
            cb = GetFileVersionInfoSize(fileName, ref handle);
            if (cb > 0)
            {
                IntPtr buffer = Marshal.AllocHGlobal(cb);
                
                try
                {
                    if (!GetFileVersionInfo(fileName, handle, cb, buffer))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "Error retrieving FileVersionInformation");
                    }
                    IntPtr zero = IntPtr.Zero;
                    int len = 0;
                    if (!VerQueryValue(buffer, @"\", ref zero, ref len))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "Error retrieving language independant version");
                    }
                    this.m_fixedversioninfo = new byte[len];
                    Marshal.Copy(zero, this.m_fixedversioninfo, 0, len);
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        private static extern bool GetFileVersionInfo(string filename, int handle, int len, IntPtr buffer);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        private static extern int GetFileVersionInfoSize(string filename, ref int handle);

        public static FileVersionInfo GetVersionInfo(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("The specified file was not found");
            }
            return new FileVersionInfo(fileName);
        }

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        private static extern bool VerQueryValue(IntPtr buffer, string subblock, ref IntPtr blockbuffer, ref int len);

        // Properties
        public int FileBuildPart
        {
            get
            {
                return Convert.ToInt32(BitConverter.ToInt16(this.m_fixedversioninfo, 14));
            }
        }

        public int FileMajorPart
        {
            get
            {
                return Convert.ToInt32(BitConverter.ToInt16(this.m_fixedversioninfo, 10));
            }
        }

        public int FileMinorPart
        {
            get
            {
                return Convert.ToInt32(BitConverter.ToInt16(this.m_fixedversioninfo, 8));
            }
        }

        public string FileName
        {
            get
            {
                return this.m_filename;
            }
        }

        public int FilePrivatePart
        {
            get
            {
                return Convert.ToInt32(BitConverter.ToInt16(this.m_fixedversioninfo, 12));
            }
        }

        public int ProductBuildPart
        {
            get
            {
                return Convert.ToInt32(BitConverter.ToInt16(this.m_fixedversioninfo, 0x16));
            }
        }

        public int ProductMajorPart
        {
            get
            {
                return Convert.ToInt32(BitConverter.ToInt16(this.m_fixedversioninfo, 0x12));
            }
        }

        public int ProductMinorPart
        {
            get
            {
                return Convert.ToInt32(BitConverter.ToInt16(this.m_fixedversioninfo, 0x10));
            }
        }

        public int ProductPrivatePart
        {
            get
            {
                return Convert.ToInt32(BitConverter.ToInt16(this.m_fixedversioninfo, 20));
            }
        }
    }
}
