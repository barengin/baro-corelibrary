using System;

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Baro.CoreLibrary.IO
{
#if PocketPC || WindowsCE
    public enum DataProtectionScope
    {
        CurrentUser,
        LocalMachine
    }

    /// <summary>
    /// This class is using TrustedAPIs. You must have a valid certificate to use this class.
    /// </summary>
    public static class ProtectedData
    {
        // Fields
        private const int CRYPTPROTECT_LOCAL_MACHINE = 4;

        // Methods
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        private static extern bool CryptProtectData(ref CRYPTOAPI_BLOB pDataIn, IntPtr szDataDescr, ref CRYPTOAPI_BLOB pOptionalEntropy, IntPtr pvReserved, IntPtr pPromptStruct, uint dwFlags, ref CRYPTOAPI_BLOB pDataOut);
        
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        private static extern bool CryptUnprotectData(ref CRYPTOAPI_BLOB pDataIn, IntPtr ppszDataDescr, ref CRYPTOAPI_BLOB Entropy, IntPtr pvReserved, IntPtr pPromptStruct, uint dwFlags, ref CRYPTOAPI_BLOB pDataOut);
        
        [DllImport(SystemDLL.NAME)]
        private static extern void memset(IntPtr pbData, uint offset, uint cbData);

        public static byte[] Protect(byte[] userData, byte[] optionalEntropy, DataProtectionScope scope)
        {
            byte[] destination = null;
            CRYPTOAPI_BLOB pDataOut = new CRYPTOAPI_BLOB();
            GCHandle handle = new GCHandle();
            GCHandle handle2 = new GCHandle();
            if (userData == null)
            {
                throw new ArgumentNullException("userData");
            }
            try
            {
                handle = GCHandle.Alloc(userData, GCHandleType.Pinned);
                CRYPTOAPI_BLOB pDataIn = new CRYPTOAPI_BLOB();
                pDataIn.cbData = (uint)userData.Length;
                pDataIn.pbData = new IntPtr((int)handle.AddrOfPinnedObject());
                CRYPTOAPI_BLOB pOptionalEntropy = new CRYPTOAPI_BLOB();
                if (optionalEntropy != null)
                {
                    handle2 = GCHandle.Alloc(optionalEntropy, GCHandleType.Pinned);
                    pOptionalEntropy.cbData = (uint)optionalEntropy.Length;
                    pOptionalEntropy.pbData = handle2.AddrOfPinnedObject();
                }
                uint dwFlags = 1;
                if (scope == DataProtectionScope.LocalMachine)
                {
                    dwFlags |= 4;
                }
                if (!CryptProtectData(ref pDataIn, IntPtr.Zero, ref pOptionalEntropy, IntPtr.Zero, IntPtr.Zero, dwFlags, ref pDataOut))
                {
                    throw new CryptographicException(Marshal.GetLastWin32Error());
                }
                if (pDataOut.pbData == IntPtr.Zero)
                {
                    throw new OutOfMemoryException();
                }
                destination = new byte[pDataOut.cbData];
                Marshal.Copy(pDataOut.pbData, destination, 0, destination.Length);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
                if (handle2.IsAllocated)
                {
                    handle2.Free();
                }
                if (pDataOut.pbData != IntPtr.Zero)
                {
                    ZeroMemory(pDataOut.pbData, pDataOut.cbData);
                    Marshal.FreeHGlobal(pDataOut.pbData);
                }
            }
            return destination;
        }

        public static byte[] Unprotect(byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope)
        {
            byte[] destination = null;
            CRYPTOAPI_BLOB pDataOut = new CRYPTOAPI_BLOB();
            GCHandle handle = new GCHandle();
            GCHandle handle2 = new GCHandle();
            try
            {
                handle = GCHandle.Alloc(encryptedData, GCHandleType.Pinned);
                CRYPTOAPI_BLOB pDataIn = new CRYPTOAPI_BLOB();
                pDataIn.cbData = (uint)encryptedData.Length;
                pDataIn.pbData = new IntPtr((int)handle.AddrOfPinnedObject());
                CRYPTOAPI_BLOB entropy = new CRYPTOAPI_BLOB();
                if (optionalEntropy != null)
                {
                    handle2 = GCHandle.Alloc(optionalEntropy, GCHandleType.Pinned);
                    entropy.cbData = (uint)optionalEntropy.Length;
                    entropy.pbData = handle2.AddrOfPinnedObject();
                }
                uint dwFlags = 1;
                if (scope == DataProtectionScope.LocalMachine)
                {
                    dwFlags |= 4;
                }
                if (!CryptUnprotectData(ref pDataIn, IntPtr.Zero, ref entropy, IntPtr.Zero, IntPtr.Zero, dwFlags, ref pDataOut))
                {
                    throw new CryptographicException(Marshal.GetLastWin32Error());
                }
                if (pDataOut.pbData == IntPtr.Zero)
                {
                    throw new OutOfMemoryException();
                }
                destination = new byte[pDataOut.cbData];
                Marshal.Copy(pDataOut.pbData, destination, 0, destination.Length);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
                if (handle2.IsAllocated)
                {
                    handle2.Free();
                }
                if (pDataOut.pbData != IntPtr.Zero)
                {
                    ZeroMemory(pDataOut.pbData, pDataOut.cbData);
                    Marshal.FreeHGlobal(pDataOut.pbData);
                }
            }
            return destination;
        }

        private static void ZeroMemory(IntPtr pbData, uint cbData)
        {
            memset(pbData, 0, cbData);
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        private struct CRYPTOAPI_BLOB
        {
            public uint cbData;
            public IntPtr pbData;
        }
    }
#endif
}
