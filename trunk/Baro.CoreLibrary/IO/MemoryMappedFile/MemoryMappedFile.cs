using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Baro.CoreLibrary.IO.NativeFile;
using System.IO;

namespace Baro.CoreLibrary.IO.MemoryMappedFile
{
#if PocketPC || WindowsCE
    public unsafe class MemoryMappedFile : IDisposable
    {
        #region Consts

        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const int OPEN_EXISTING = 3;
        private const int FILE_ATTRIBUTE_NORMAL = 0x00000080;
        private const int FILE_FLAG_RANDOM_ACCESS = 0x10000000;
        private const int PAGE_READONLY = 0x02;
        private const int PAGE_READWRITE = 0x04;
        private const int SECTION_MAP_READ = 0x0004;
        private const int SEC_COMMIT = 0x8000000;
        public const uint MAPPING_SIZE = 1024 * 1024 * 32;

        #endregion

        private NativeFileReader m_file;
        private int m_mappingHandle = 0;
        private byte* m_mem;

        public byte* Mem
        {
            get
            {
                return m_mem;
            }
        }

        public MemoryMappedFile(NativeFileReader file)
        {
            m_file = file;
            OpenMap();
        }

        private void OpenMap()
        {
            // Create file mapping
            m_mappingHandle = MemoryMappedFileNatives.CreateFileMapping(m_file.FileHandle, 0, PAGE_READONLY | SEC_COMMIT, 0, 0, IntPtr.Zero);

            if (m_mappingHandle == 0)
            {
                IOException eb = new IOException("Can't create mapping object");
                throw eb;
            }

            m_mem = MemoryMappedFileNatives.MapViewOfFile(m_mappingHandle, SECTION_MAP_READ, 0, 0, 0);
            
            if (m_mem == null)
            {
                IOException eb = new IOException("Can't create mapping object");
                throw eb;
            }
        }

        private void CloseMap()
        {
            // Unmap
            if (m_mem != null)
            {
                MemoryMappedFileNatives.UnmapViewOfFile(m_mem);
                m_mem = null;
            }

            if (m_mappingHandle != 0)
            {
                MemoryMappedFileNatives.CloseHandle(m_mappingHandle);
                m_mappingHandle = 0;
            }

            m_file.Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            CloseMap();
            GC.SuppressFinalize(this);
        }

        ~MemoryMappedFile()
        {
            Dispose();
        }

        #endregion
    }
#endif
}
