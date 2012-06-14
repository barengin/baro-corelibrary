using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary
{
    internal static class NativeMethods
    {
        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool SetSystemMemoryDivision(int dwStorePages);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool GetSystemMemoryDivision(ref int lpdwStorePages, ref int lpdwRamPages, ref int lpdwPageSize);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern void MessageBeep(int type);

        [DllImport(SystemDLL.NAME, EntryPoint = "PlaySoundW", SetLastError = true)]
        internal static extern bool PlaySound(IntPtr lpszName, IntPtr hModule, SoundFlags dwFlags);

        [DllImport(SystemDLL.NAME, EntryPoint = "PlaySoundW", SetLastError = true)]
        internal static extern bool PlaySound(string lpszName, IntPtr hModule, SoundFlags dwFlags);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInAddBuffer(IntPtr hwi, IntPtr lpHdr, int cbwh);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInAddBuffer(IntPtr hwi, byte[] pwh, int cbwh);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInClose(IntPtr hDev);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInGetDevCaps(int uDeviceID, byte[] pwic, int cbwic);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInGetNumDevs();

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int waveInOpen(out IntPtr t, uint id, byte[] pwfx, IntPtr dwCallback, int dwInstance, int fdwOpen);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInPrepareHeader(IntPtr hwi, IntPtr lpHdr, int cbwh);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInPrepareHeader(IntPtr hwi, byte[] pwh, int cbwh);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInReset(IntPtr hwi);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInStart(IntPtr hwi);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInStop(IntPtr hwi);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInUnprepareHeader(IntPtr hwi, byte[] pwh, int cbwh);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveInUnprepareHeader(IntPtr hwi, IntPtr lpHdr, int cbwh);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutClose(IntPtr hwo);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutGetNumDevs();

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutGetVolume(IntPtr hwo, ref int pdwVolume);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutOpen(out IntPtr t, int id, byte[] pwfx, IntPtr dwCallback, int dwInstance, int fdwOpen);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutPause(IntPtr hwo);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutPrepareHeader(IntPtr hwo, IntPtr lpHdr, int cbwh);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutPrepareHeader(IntPtr hwo, byte[] pwh, int cbwh);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutReset(IntPtr hwo);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutRestart(IntPtr hwo);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutSetVolume(IntPtr hwo, int dwVolume);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutUnprepareHeader(IntPtr hwo, byte[] pwh, int cbwh);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutUnprepareHeader(IntPtr hwo, IntPtr lpHdr, int cbwh);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutWrite(IntPtr hwo, IntPtr lpHdr, int cbwh);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int waveOutWrite(IntPtr hwo, byte[] pwh, int cbwh);

        // Fields
        internal const int ERROR_NO_MORE_FILES = 0x12;
        internal const uint MEM_RELEASE = 0x8000;
        internal const uint MEM_RESERVE = 0x2000;
        internal const uint PAGE_NOACCESS = 1;
        internal const uint PAGE_NOCACHE = 0x200;
        internal const uint PAGE_PHYSICAL = 0x400;
        internal const uint PAGE_READWRITE = 4;

        // Methods
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
        internal static extern IntPtr VirtualAlloc(uint lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int VirtualCopy(IntPtr lpvDest, IntPtr lpvSrc, uint cbSize, uint fdwProtect);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int VirtualFree(IntPtr lpAddress, uint dwSize, uint dwFreeType);

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

        // Fields
        internal const int INFINITE = -1;
        internal const int INVALID_HANDLE_VALUE = -1;


        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern int CeRunAppAtTime(string application, SystemTime startTime);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int FileTimeToSystemTime(ref long lpFileTime, byte[] lpSystemTime);

        [DllImport(SystemDLL.NAME, EntryPoint = "FormatMessageW")]
        internal static extern int FormatMessage(FormatMessageFlags dwFlags, int lpSource, int dwMessageId, int dwLanguageId, out IntPtr lpBuffer, int nSize, int[] Arguments);

        [DllImport(SystemDLL.NAME, EntryPoint = "GetModuleHandleW", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport(SystemDLL.NAME, EntryPoint = "GetProcAddressW", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern void GetSystemInfo(out SystemInfo pSI);

        [DllImport(SystemDLL.NAME)]
        public static extern void GlobalMemoryStatus(out MemoryStatus msce);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool IsBadCodePtr(IntPtr fn);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport(SystemDLL.NAME, EntryPoint = "LoadLibraryW", SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpszLib);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern IntPtr OpenProcess(uint fdwAccess, bool fInherit, int IDProcess);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern IntPtr OpenProcess(uint fdwAccess, bool fInherit, uint IDProcess);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool PostKeybdMessage(IntPtr hwnd, uint vKey, KeyStateFlags flags, uint cCharacters, KeyStateFlags[] pShiftStateBuffer, uint[] pCharacterBuffer);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int QueryPerformanceCounter(ref long lpPerformanceCount);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int QueryPerformanceFrequency(ref long lpFrequency);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int RegCloseKey(uint hKey);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool RegCopyFile(string lpszFile);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int RegCreateKeyEx(uint hKey, string lpSubKey, int lpReserved, string lpClass, int dwOptions, int samDesired, IntPtr lpSecurityAttributes, ref uint phkResult, ref KeyDisposition lpdwDisposition);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool RegReplaceKey(uint hKey, IntPtr lpSubKey, string lpNewFile, IntPtr lpOldFile);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern bool RegRestoreFile(string lpszFile);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int RegSaveKey(uint hKey, string lpFile, IntPtr lpSecurityAttributes);

        [DllImport(SystemDLL.NAME)]
        internal static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, StringBuilder lpszPath, int nFolder, int fCreate);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int SystemTimeToFileTime(byte[] lpSystemTime, ref long lpFileTime);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern bool TerminateProcess(IntPtr hProcess, int uExitCode);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int timeKillEvent(int uTimerID);

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        internal static extern int timeSetEvent(int uDelay, int uResolution, IntPtr fptc, int dwUser, MMTimerEventType fuEvent);

        [Flags]
        internal enum SoundFlags
        {
            Alias = 0x10000,
            Async = 1,
            FileName = 0x20000,
            Loop = 8,
            Memory = 4,
            NoDefault = 2,
            NoStop = 0x10,
            NoWait = 0x2000,
            Resource = 0x40004,
            Sync = 0
        }

        // Nested Types
        [Flags]
        public enum FormatMessageFlags
        {
            AllocateBuffer = 0x100,
            ArgumentArray = 0x2000,
            FromHModule = 0x800,
            FromString = 0x400,
            FromSystem = 0x1000,
            IgnoreInserts = 0x200,
            MaxWidthMask = 0xff
        }

        internal enum KeyDisposition
        {
            CreatedNewKey = 1,
            OpenedExistingKey = 2
        }

        [Flags]
        internal enum KeyStateFlags
        {
            AnyAlt = 0x10000000,
            AnyCtrl = 0x40000000,
            AnyShift = 0x20000000,
            AsyncDown = 2,
            Capital = 0x8000000,
            Dead = 0x20000,
            Down = 0x80,
            Language1 = 0x8000,
            LeftAlt = 0x1000000,
            LeftCtrl = 0x4000000,
            LeftShift = 0x2000000,
            LeftWin = 0x800000,
            NoCharacter = 0x10000,
            NumLock = 0x1000,
            PrevDown = 0x40,
            RightAlt = 0x100000,
            RightCtrl = 0x400000,
            RightShift = 0x200000,
            RightWin = 0x80000,
            Toggled = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MemoryStatus
        {
            internal uint dwLength;
            public int MemoryLoad;
            public int TotalPhysical;
            public int AvailablePhysical;
            public int TotalPageFile;
            public int AvailablePageFile;
            public int TotalVirtual;
            public int AvailableVirtual;
        }

        [Flags]
        internal enum MMTimerEventType
        {
            Callback = 0,
            EventPulse = 0x20,
            EventSet = 0x10,
            KillSynchronous = 0x100,
            OneShot = 0,
            Periodic = 1
        }

        public enum ProcessorArchitecture : short
        {
            Alpha = 2,
            Alpha64 = 7,
            ARM = 5,
            IA64 = 6,
            Intel = 0,
            MIPS = 1,
            PPC = 3,
            SHX = 4,
            Unknown = -1
        }

        public enum ProcessorType
        {
            Alpha_21064 = 0x5248,
            ARM_7TDMI = 0x11171,
            ARM720 = 0x720,
            ARM820 = 0x820,
            ARM920 = 0x920,
            Hitachi_SH3 = 0x2713,
            Hitachi_SH3E = 0x2714,
            Hitachi_SH4 = 0x2715,
            Intel_386 = 0x182,
            Intel_486 = 0x1e6,
            Intel_IA64 = 0x898,
            Intel_Pentium = 0x24a,
            Intel_PentiumII = 0x2ae,
            MIPS_R4000 = 0xfa0,
            Motorola_821 = 0x335,
            PPC_403 = 0x193,
            PPC_601 = 0x259,
            PPC_603 = 0x25b,
            PPC_604 = 0x25c,
            PPC_620 = 620,
            SHx_SH3 = 0x67,
            SHx_SH4 = 0x68,
            StrongARM = 0xa11
        }

        internal enum RootKeys : uint
        {
            ClassesRoot = 0x80000000,
            CurrentUser = 0x80000001,
            LocalMachine = 0x80000002,
            Users = 0x80000003
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemInfo
        {
            public NativeMethods.ProcessorArchitecture ProcessorArchitecture;
            internal ushort wReserved;
            public int PageSize;
            public int MinimumApplicationAddress;
            public int MaximumApplicationAddress;
            public int ActiveProcessorMask;
            public int NumberOfProcessors;
            public NativeMethods.ProcessorType ProcessorType;
            public int AllocationGranularity;
            public short ProcessorLevel;
            public short ProcessorRevision;
        }
    }
}
