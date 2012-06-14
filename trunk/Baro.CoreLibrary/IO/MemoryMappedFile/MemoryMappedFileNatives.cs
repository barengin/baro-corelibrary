using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Baro.CoreLibrary.IO.MemoryMappedFile
{
#if PocketPC || WindowsCE
    internal unsafe static class MemoryMappedFileNatives
    {
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hFile);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool CloseHandle(int hFile);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int CreateFileMapping(
            IntPtr hFile, // Handle to file
            int lpAttributes, // Security
            int flProtect, // protection
            uint dwMaximumSizeHigh, // High-order DWORD of size
            uint dwMaximumSizeLow, // Low-order DWORD of size
            IntPtr lpName // Object name
            );

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            int dwShareMode,
            int lpSecurityAttributes,
            int dwCreationDisposition, // How to create
            int dwFlagsAndAttributes, // File attributes
            int hTemplateFile // Handle to template file
            );

        // Mobile 5 ve 6 üzerinde artık CreateFile komutunu kullanıyoruz.

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int CreateFileForMapping(
            string lpFileName,
            uint dwDesiredAccess,
            int dwShareMode,
            int lpSecurityAttributes,
            int dwCreationDisposition, // How to create
            int dwFlagsAndAttributes, // File attributes
            int hTemplateFile // Handle to template file
            );

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern byte* MapViewOfFile(
            int hFileMappingObject, // handle to file-mapping object
            uint dwDesiredAccess, // Access mode
            uint dwFileOffsetHigh, // High-order DWORD of offset
            uint dwFileOffsetLow, // Low-order DWORD of offset
            uint dwNumberOfBytesToMap // Number of bytes to map
            );

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool UnmapViewOfFile(
            byte* lpBaseAddress // Starting address
            );

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
#endif
}
