using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.IO.NativeFile
{
    public unsafe class NativeFileReader : IDisposable
    {
        private IntPtr hFileHandle;
        private int m_FileSize;
        private const uint GENERIC_READ = 0x80000000;
        private const int FILE_ATTRIBUTE_NORMAL = 0x00000080;
        private const int FILE_FLAG_RANDOM_ACCESS = 0x10000000;
        private const int FILE_FLAG_NO_BUFFERING = 0x20000000;

        public NativeFileReader(string file)
        {
            // FILE_SHARE_READ = 1
            hFileHandle = FileNativeMethods.CreateFile(file, GENERIC_READ, 1, 0, (uint)FileMode.Open, FILE_FLAG_NO_BUFFERING, 0);

            // Invalid handle value
            if (hFileHandle == (IntPtr)(-1))
            {
                IOException eb = new IOException("File can not open", null);
                throw eb;
            }

            m_FileSize = FileNativeMethods.SetFilePointer(hFileHandle, 0, 0, SeekOrigin.End);
            Position = 0;
        }

        internal IntPtr FileHandle
        {
            get
            {
                return hFileHandle;
            }
        }

        public int Size
        {
            get
            {
                return m_FileSize;
            }
        }

        public int Position
        {
            get
            {
                return FileNativeMethods.SetFilePointer(hFileHandle, 0, 0, SeekOrigin.Current);
            }
            set
            {
                FileNativeMethods.SetFilePointer(hFileHandle, value, 0, SeekOrigin.Begin);
            }
        }

        public int Read(byte* buffer, int bytesToRead)
        {
            int readed = 0;
            FileNativeMethods.ReadFile(hFileHandle, buffer, bytesToRead, ref readed, IntPtr.Zero);
            return readed;
        }

        public int Read(byte[] buffer, int bytesToRead)
        {
            int readed = 0;
            FileNativeMethods.ReadFile(hFileHandle, buffer, bytesToRead, ref readed, IntPtr.Zero);
            return readed;
        }

        public void Close()
        {
            FileNativeMethods.CloseHandle(hFileHandle);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        ~NativeFileReader()
        {
            Dispose();
        }

        #endregion
    }
}
