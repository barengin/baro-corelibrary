using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.IO;

namespace Baro.CoreLibrary.Gps.Serial
{
    internal static class NativeMethods
    {
        #region Cross-Platform Members

        /*
         * Many API calls are identical on either framework. The only thing different is
         * the dll name in the DllImport atribute, which can be a string constant. The 
         * API calls that are used by both frameworks can be moved here to lessen the 
         * code footprint for the sake of maintainability. Others, like the GPSID and 
         * Serial Comm can be kept seprarated. 
         */
#if !PocketPC
        private const string Kernel32 = "kernel32.dll";
#else
        private const string Kernel32 = "coredll.dll";
#endif

        #region Kernel32

        public static readonly IntPtr INVALID_HANDLE = new IntPtr(-1);

        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        #region IOControl

        /// <summary>
        /// Interacts with a device driver
        /// </summary>
        /// <param name="hDevice"> A handle to a device opened with the CreateFile function. </param>
        /// <param name="dwIoControlCode"> A control code specific to the device driver. </param>
        /// <param name="lpInBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="lpOutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="lpBytesReturned"> The number of bytes returned in the out buffer. </param>
        /// <param name="lpOverlapped"> A pointer to a callback for async commands. </param>
        /// <returns></returns>
        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            IntPtr lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped);

        /// <summary>
        /// Opens an IO channel with a physical device.
        /// </summary>
        /// <param name="lpFileName"> The device path </param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns> A handle to the device. </returns>
        [DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile(
            string lpFileName,							// file name
            FileAccess dwDesiredAccess,					// access mode
            FileShare dwShareMode,						// share mode
            uint lpSecurityAttributes,					// SD
            FileMode dwCreationDisposition,				// how to create
            FileAttributes dwFlagsAndAttributes,		// file attributes
            IntPtr hTemplateFile						// handle to template file
            );

        /// <summary>
        /// Performs synchronous reads on an IO channel
        /// </summary>
        /// <param name="hDevice"> A handle to a device opened with the CreateFile function. </param>
        /// <param name="lpBuffer"></param>
        /// <param name="nNumberOfBytesToRead"></param>
        /// <param name="lpNumberOfBytesRead"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool ReadFile(
            IntPtr handle,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        /// <summary>
        /// Performs synchronous writes on an IO channel
        /// </summary>
        /// <param name="hDevice"> A handle to a device opened with the CreateFile function. </param>
        /// <param name="lpBuffer"></param>
        /// <param name="nNumberOfBytesToWrite"></param>
        /// <param name="lpNumberOfBytesWritten"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool WriteFile(
            IntPtr handle,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        #endregion

        #endregion
        #endregion

        #region USB

        public const int DIGCF_DEFAULT = 0x00000001;  // only valid with DIGCF_DEVICEINTERFACE
        public const int DIGCF_PRESENT = 0x00000002;
        public const int DIGCF_ALLCLASSES = 0x00000004;
        public const int DIGCF_PROFILE = 0x00000008;
        public const int DIGCF_DEVICEINTERFACE = 0x00000010;

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public uint cbSize;
            public Guid InterfaceClassGuid;
            public uint Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiClassGuidsFromName(
            StringBuilder ClassName,
            //[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
            IntPtr ClassGuidList,
            int ClassGuidListSize,
            out int RequiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int CM_Get_Parent(
           out UInt32 pdnDevInst,
           UInt32 dnDevInst,
           int ulFlags
        );

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int CM_Get_Device_ID(
           UInt32 dnDevInst,
           IntPtr Buffer,
           int BufferLen,
           int ulFlags
        );

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr hDevInfo);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(
           ref Guid ClassGuid,
           IntPtr Enumerator,
           IntPtr hwndParent,
           int Flags
        );

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(
            IntPtr hDevInfo,
            UInt32 memberIndex,
            ref SP_DEVINFO_DATA devInfo);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(
           IntPtr hDevInfo,
           IntPtr devInfo,
           ref Guid interfaceClassGuid,
           UInt32 memberIndex,
           ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
        );

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(
           IntPtr hDevInfo,
           ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
           ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
           UInt32 deviceInterfaceDetailDataSize,
           out UInt32 requiredSize,
           ref SP_DEVINFO_DATA deviceInfoData
        );

        #endregion

        #region Mobile Device Members

        #region GPS Intermediate Driver

        // This method is used to tell the GPSID to reload its settings from the registry.
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            IntPtr lpOutBuffer,
            uint nOutBufferSize,
            uint lpBytesReturned,
            IntPtr lpOverlapped);

        public const uint IOCTL_SERVICE_REFRESH = 0x4100000C;

        #endregion

        #region Serial Port

        // http://msdn.microsoft.com/en-us/library/aa915369.aspx
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool GetCommState(IntPtr handle, ref DCB dcb);

        // http://msdn.microsoft.com/en-us/library/aa908949.aspx
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool SetCommState(IntPtr handle, [In] ref DCB dcb);

        // http://msdn.microsoft.com/en-us/library/aa363428(VS.85).aspx
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool PurgeComm(IntPtr handle, uint flags);

        // http://msdn.microsoft.com/en-us/library/aa363439(VS.85).aspx
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool SetupComm(IntPtr handle, uint dwInQueue, uint dwOutQueue);

        // http://msdn.microsoft.com/en-us/library/bb202767.aspx
        [StructLayout(LayoutKind.Sequential)]
        public struct DCB
        {
            internal uint DCBLength;
            internal uint BaudRate;
            internal BitVector32 Flags;
            internal Int16 wReserved;        // not currently used
            internal Int16 XonLim;           // transmit XON threshold
            internal Int16 XoffLim;          // transmit XOFF threshold             
            internal byte ByteSize;
            internal byte Parity;
            internal byte StopBits;
            //...and some more
            internal byte XonChar;          // Tx and Rx XON character
            internal byte XoffChar;         // Tx and Rx XOFF character
            internal byte ErrorChar;        // error replacement character
            internal byte EofChar;          // end of input character
            internal byte EvtChar;          // received event character
            internal Int16 wReserved1;       // reserved; do not use 
            internal int dwRes1, dwRes2, dwRes3, dwRes4, dwRes5, dwRes6, 
                         dwRes7, dwRes8, dwRes9, dwRes10, dwRes11;

            private static readonly int fBinary;
            private static readonly int fParity;
            private static readonly int fOutxCtsFlow;
            private static readonly int fOutxDsrFlow;
            private static readonly BitVector32.Section fDtrControl;
            private static readonly int fDsrSensitivity;
            private static readonly int fTXContinueOnXoff;
            private static readonly int fOutX;
            private static readonly int fInX;
            private static readonly int fErrorChar;
            private static readonly int fNull;
            private static readonly BitVector32.Section fRtsControl;
            private static readonly int fAbortOnError;

            static DCB()
            {
                // Create Boolean Mask
                int previousMask;
                fBinary = BitVector32.CreateMask();
                fParity = BitVector32.CreateMask(fBinary);
                fOutxCtsFlow = BitVector32.CreateMask(fParity);
                fOutxDsrFlow = BitVector32.CreateMask(fOutxCtsFlow);
                previousMask = BitVector32.CreateMask(fOutxDsrFlow);
                previousMask = BitVector32.CreateMask(previousMask);
                fDsrSensitivity = BitVector32.CreateMask(previousMask);
                fTXContinueOnXoff = BitVector32.CreateMask(fDsrSensitivity);
                fOutX = BitVector32.CreateMask(fTXContinueOnXoff);
                fInX = BitVector32.CreateMask(fOutX);
                fErrorChar = BitVector32.CreateMask(fInX);
                fNull = BitVector32.CreateMask(fErrorChar);
                previousMask = BitVector32.CreateMask(fNull);
                previousMask = BitVector32.CreateMask(previousMask);
                fAbortOnError = BitVector32.CreateMask(previousMask);

                // Create section Mask
                BitVector32.Section previousSection;
                previousSection = BitVector32.CreateSection(1);
                previousSection = BitVector32.CreateSection(1, previousSection);
                previousSection = BitVector32.CreateSection(1, previousSection);
                previousSection = BitVector32.CreateSection(1, previousSection);
                fDtrControl = BitVector32.CreateSection(2, previousSection);
                previousSection = BitVector32.CreateSection(1, fDtrControl);
                previousSection = BitVector32.CreateSection(1, previousSection);
                previousSection = BitVector32.CreateSection(1, previousSection);
                previousSection = BitVector32.CreateSection(1, previousSection);
                previousSection = BitVector32.CreateSection(1, previousSection);
                previousSection = BitVector32.CreateSection(1, previousSection);
                fRtsControl = BitVector32.CreateSection(2, previousSection);
                previousSection = BitVector32.CreateSection(1, fRtsControl);
            }

            public bool Binary
            {
                get { return Flags[fBinary]; }
                set { Flags[fBinary] = value; }
            }

            public bool CheckParity
            {
                get { return Flags[fParity]; }
                set { Flags[fParity] = value; }
            }

            public bool OutxCtsFlow
            {
                get { return Flags[fOutxCtsFlow]; }
                set { Flags[fOutxCtsFlow] = value; }
            }

            public bool OutxDsrFlow
            {
                get { return Flags[fOutxDsrFlow]; }
                set { Flags[fOutxDsrFlow] = value; }
            }

            public DtrControl DtrControl
            {
                get { return (DtrControl)Flags[fDtrControl]; }
                set { Flags[fDtrControl] = (int)value; }
            }

            public bool DsrSensitivity
            {
                get { return Flags[fDsrSensitivity]; }
                set { Flags[fDsrSensitivity] = value; }
            }

            public bool TxContinueOnXoff
            {
                get { return Flags[fTXContinueOnXoff]; }
                set { Flags[fTXContinueOnXoff] = value; }
            }

            public bool OutX
            {
                get { return Flags[fOutX]; }
                set { Flags[fOutX] = value; }
            }

            public bool InX
            {
                get { return Flags[fInX]; }
                set { Flags[fInX] = value; }
            }

            public bool ReplaceErrorChar
            {
                get { return Flags[fErrorChar]; }
                set { Flags[fErrorChar] = value; }
            }

            public bool Null
            {
                get { return Flags[fNull]; }
                set { Flags[fNull] = value; }
            }

            public RtsControl RtsControl
            {
                get { return (RtsControl)Flags[fRtsControl]; }
                set { Flags[fRtsControl] = (int)value; }
            }

            public bool AbortOnError
            {
                get { return Flags[fAbortOnError]; }
                set { Flags[fAbortOnError] = value; }
            }
        }

        // http://msdn.microsoft.com/en-us/library/aa363180(VS.85).aspx
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool ClearCommError(IntPtr handle, ref uint lpErrors, COMMSTAT lpStat);

        // http://msdn.microsoft.com/en-us/library/aa363200(VS.85).aspx
        [StructLayout(LayoutKind.Sequential)]
        public struct COMMSTAT
        {
            private BitVector32 Flags;
            internal uint dbInQue;
            internal uint dbOutQueue;

            private static readonly int fCtsHold;
            private static readonly int fDsrHold;
            private static readonly int fRlsdHold;
            private static readonly int fXoffHold;
            private static readonly int fXoffSent;
            private static readonly int fEof;
            private static readonly int fTrim;

            static COMMSTAT()
            {
                // Create Boolean Mask
                fCtsHold = BitVector32.CreateMask();
                fDsrHold = BitVector32.CreateMask(fCtsHold);
                fRlsdHold = BitVector32.CreateMask(fDsrHold);
                fXoffHold = BitVector32.CreateMask(fRlsdHold);
                fXoffSent = BitVector32.CreateMask(fXoffHold);
                fEof = BitVector32.CreateMask(fXoffSent);
                fTrim = BitVector32.CreateMask(fEof);
            }

            public bool CtsHold
            {
                get { return Flags[fCtsHold]; }
                set { Flags[fCtsHold] = value; }
            }

            public bool DsrHold
            {
                get { return Flags[fDsrHold]; }
                set { Flags[fDsrHold] = value; }
            }

            public bool RlsdHold
            {
                get { return Flags[fRlsdHold]; }
                set { Flags[fRlsdHold] = value; }
            }

            public bool XoffHold
            {
                get { return Flags[fXoffHold]; }
                set { Flags[fXoffHold] = value; }
            }

            public bool Eof
            {
                get { return Flags[fEof]; }
                set { Flags[fEof] = value; }
            }

            public bool Trim
            {
                get { return Flags[fTrim]; }
                set { Flags[fTrim] = value; }
            }
        }


        public enum DtrControl : int
        {
            Disable = 0,
            Enable = 1,
            Handshake = 2
        };

        public enum RtsControl : int
        {
            Disable = 0,
            Enable = 1,
            Handshake = 2,
            Toggle = 3
        };

        // http://msdn.microsoft.com/en-us/library/aa363190(VS.85).aspx
        [StructLayout(LayoutKind.Sequential)]
        public struct COMMTIMEOUTS
        {
            public uint ReadIntervalTimeout;
            public uint ReadTotalTimeoutMultiplier;
            public uint ReadTotalTimeoutConstant;
            public uint WriteTotalTimeoutMultiplier;
            public uint WriteTotalTimeoutConstant;
        }

        // http://msdn.microsoft.com/en-us/library/aa363261(VS.85).aspx
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool GetCommTimeouts(IntPtr handle, ref COMMTIMEOUTS timeouts);

        // http://msdn.microsoft.com/en-us/library/aa363437(VS.85).aspx
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool SetCommTimeouts(IntPtr handle, [In] ref COMMTIMEOUTS timeouts);

        #endregion

        #endregion
    }
}
