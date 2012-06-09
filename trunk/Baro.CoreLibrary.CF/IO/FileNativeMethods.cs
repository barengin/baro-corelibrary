using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Baro.CoreLibrary.IO
{
    internal static unsafe class FileNativeMethods
    {
        // Fields
        internal const int ERROR_NO_MORE_FILES = 0x12;
        internal const uint MEM_RELEASE = 0x8000;
        internal const uint MEM_RESERVE = 0x2000;
        internal const uint PAGE_NOACCESS = 1;
        internal const uint PAGE_NOCACHE = 0x200;
        internal const uint PAGE_PHYSICAL = 0x400;
        internal const uint PAGE_READWRITE = 4;

        // Methods
        [DllImport(SystemDLL.NAME)]
        extern static public void memcpy(IntPtr p1, IntPtr p2, int cb);

        [DllImport(SystemDLL.NAME)]
        unsafe extern static public void memcpy(byte* p1, byte* p2, int cb);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern IntPtr ActivateDevice(string lpszDevKey, uint dwClientInfo);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, int lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, int hTemplateFile);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool DeactivateDevice(IntPtr hDevice);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, byte[] lpInBuffer, int nInBufferSize, byte[] lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, byte[] lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, byte[] lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool GetDiskFreeSpaceEx(string directoryName, ref long freeBytesAvailable, ref long totalBytes, ref long totalFreeBytes);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern uint GetFileAttributes(string lpFileName);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool ReadFile(IntPtr hFile, byte* lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool SetFileAttributes(string lpFileName, uint dwFileAttributes);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int SetFilePointer(IntPtr hFile, int lDistanceToMove, int lpDistanceToMoveHigh, SeekOrigin dwMoveMethod);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool SetFileTime(IntPtr hFile, byte[] lpCreationTime, byte[] lpLastAccessTime, byte[] lpLastWriteTime);
        
        [DllImport("aygshell.dll", SetLastError = true)]
        internal static extern int SHChangeNotifyDeregister(IntPtr hwnd);
        
        [DllImport("aygshell.dll", SetLastError = true)]
        internal static extern void SHChangeNotifyFree(IntPtr pshcne);
        
        [DllImport("aygshell.dll", SetLastError = true)]
        internal static extern int SHChangeNotifyRegister(IntPtr hwnd, IntPtr pshcne);
        
        [DllImport("aygshell.dll", SetLastError = true)]
        internal static extern int SHChangeNotifyRegister(IntPtr hwnd, ref SHCHANGENOTIFYENTRY pshcne);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, IntPtr lpOverlapped);

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        internal struct SHCHANGENOTIFYENTRY
        {
            internal int dwEventMask;
            internal IntPtr pszWatchDir;
            internal int fRecursive;
        }

        private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

        [DllImport(SystemDLL.NAME)]
        public static extern uint FormatMessage(
            uint dwFlags, // Source and processing options
            IntPtr lpSource, // Message source
            uint dwMessageId, // Message identifier
            uint dwLanguageId, // Language identifier
            StringBuilder lpBuffer, // Message buffer
            uint nSize, // Maximum size of message buffer
            IntPtr Arguments  // Array of message inserts
            );

        public static string GetWin32ErrorMessage(uint error)
        {
            StringBuilder buff = new StringBuilder(1024);
            uint len = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM,
                IntPtr.Zero,
                error,
                0,
                buff,
                1024,
                IntPtr.Zero);
            return buff.ToString(0, (int)len);
        }
    }
}
