using System;

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Memory
{
#if PocketPC || WindowsCE
    public sealed unsafe class HeapMemoryManager : IDisposable
    {
        #region Fields
        [DllImport(SystemDLL.NAME)]
        private static extern byte* HeapAlloc(int handle, int flag, int size);

        [DllImport(SystemDLL.NAME)]
        private static extern bool HeapFree(int handle, int flag, byte* adr);

        [DllImport(SystemDLL.NAME)]
        private static extern byte* HeapReAlloc(int handle, int flag, byte* adr, int newsize);

        [DllImport(SystemDLL.NAME)]
        private static extern int HeapCreate(int options, int initialsize, int maxsize);

        [DllImport(SystemDLL.NAME)]
        private static extern bool HeapDestroy(int handle);

        private int HeapHandle;

        #endregion

        #region Constructor
        public static readonly HeapMemoryManager instance = new HeapMemoryManager();

        private HeapMemoryManager()
        {
            HeapHandle = HeapCreate(0, 0, 0);
        }
        #endregion

        #region IDisposable Members
        ~HeapMemoryManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            HeapDestroy(HeapHandle);
            GC.SuppressFinalize(this);
        }
        #endregion

        public byte* Alloc(int size)
        {
            return HeapAlloc(HeapHandle, 0, size);
        }

        public byte* ReAlloc(byte* adr, int newsize)
        {
            return HeapReAlloc(HeapHandle, 0, adr, newsize);
        }

        public void Free(byte* adr)
        {
            HeapFree(HeapHandle, 0, adr);
        }
    }
#endif
}
