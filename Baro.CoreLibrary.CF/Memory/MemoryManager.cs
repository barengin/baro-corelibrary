using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Memory
{
#if PocketPC || WindowsCE
    public static class MemoryManagement
    {
        // Fields
        private static int m_memoryPageSize;

        // Methods
        static MemoryManagement()
        {
            int lpdwStorePages = 0;
            int lpdwRamPages = 0;
            NativeMethods.GetSystemMemoryDivision(ref lpdwStorePages, ref lpdwRamPages, ref m_memoryPageSize);
        }

        // Properties
        public static int AvailablePhysicalMemory
        {
            get
            {
                NativeMethods.MemoryStatus msce = new NativeMethods.MemoryStatus();
                NativeMethods.GlobalMemoryStatus(out msce);
                return msce.AvailablePhysical;
            }
        }

        public static int AvailableVirtualMemory
        {
            get
            {
                NativeMethods.MemoryStatus msce = new NativeMethods.MemoryStatus();
                NativeMethods.GlobalMemoryStatus(out msce);
                return msce.AvailableVirtual;
            }
        }

        public static int MemoryLoad
        {
            get
            {
                NativeMethods.MemoryStatus msce = new NativeMethods.MemoryStatus();
                NativeMethods.GlobalMemoryStatus(out msce);
                return msce.MemoryLoad;
            }
        }

        public static int SystemProgramMemory
        {
            get
            {
                int lpdwStorePages = 0;
                int lpdwRamPages = 0;
                int lpdwPageSize = 0;
                NativeMethods.GetSystemMemoryDivision(ref lpdwStorePages, ref lpdwRamPages, ref lpdwPageSize);
                return (lpdwRamPages * (lpdwPageSize >> 10));
            }
            set
            {
                int lpdwStorePages = 0;
                int lpdwRamPages = 0;
                int lpdwPageSize = 0;
                NativeMethods.GetSystemMemoryDivision(ref lpdwStorePages, ref lpdwRamPages, ref lpdwPageSize);
                SystemStorageMemory = (lpdwStorePages + lpdwRamPages) - (value << (10 / lpdwPageSize));
            }
        }

        public static int SystemStorageMemory
        {
            get
            {
                int lpdwStorePages = 0;
                int lpdwRamPages = 0;
                int lpdwPageSize = 0;
                NativeMethods.GetSystemMemoryDivision(ref lpdwStorePages, ref lpdwRamPages, ref lpdwPageSize);
                return (lpdwStorePages * (lpdwPageSize >> 10));
            }
            set
            {
                NativeMethods.SetSystemMemoryDivision((value << 10) / m_memoryPageSize);
            }
        }

        public static int TotalPhysicalMemory
        {
            get
            {
                NativeMethods.MemoryStatus msce = new NativeMethods.MemoryStatus();
                NativeMethods.GlobalMemoryStatus(out msce);
                return msce.TotalPhysical;
            }
        }

        public static int TotalVirtualMemory
        {
            get
            {
                NativeMethods.MemoryStatus msce = new NativeMethods.MemoryStatus();
                NativeMethods.GlobalMemoryStatus(out msce);
                return msce.TotalVirtual;
            }
        }
    }
#endif
}
