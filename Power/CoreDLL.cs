using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Power
{
    public static class CoreDLL
    {
        [DllImport(SystemDLL.NAME)]
        public static extern int ReleasePowerRequirement(IntPtr hPowerReq);


        [DllImport(SystemDLL.NAME, SetLastError=true)]
        public static extern IntPtr SetPowerRequirement
        (
            string pDevice,
            CEDEVICE_POWER_STATE DeviceState,
            DevicePowerFlags DeviceFlags,
            IntPtr pSystemState,
            uint StateFlagsZero
        );

        [DllImport(SystemDLL.NAME, SetLastError = true)]
        public static extern IntPtr SetDevicePower
            (
                string pDevice,
                DevicePowerFlags DeviceFlags,
            CEDEVICE_POWER_STATE DevicePowerState
            );
        [DllImport(SystemDLL.NAME)]
        public static extern int GetDevicePower(string device, DevicePowerFlags flags, out CEDEVICE_POWER_STATE PowerState);

        [DllImport(SystemDLL.NAME)]
        public static extern int SetSystemPowerState(String stateName, PowerState powerState, DevicePowerFlags flags);


        [DllImport(SystemDLL.NAME)]
        public static extern int PowerPolicyNotify(
          PPNMessage dwMessage,
            int option
        //    DevicePowerFlags);
        );

        [DllImport(SystemDLL.NAME)]
        public static extern int GetSystemPowerStatusEx2(
             SYSTEM_POWER_STATUS_EX2 statusInfo, 
            int length,
            int getLatest
                );

        
        public static SYSTEM_POWER_STATUS_EX2 GetSystemPowerStatus()
        {
            SYSTEM_POWER_STATUS_EX2 retVal = new SYSTEM_POWER_STATUS_EX2();
           int result =  GetSystemPowerStatusEx2( retVal, Marshal.SizeOf(retVal) , 1);
            return retVal;
        }

        // System\CurrentControlSet\Control\Power\Timeouts
        [DllImport(SystemDLL.NAME)]
        public static extern int SystemParametersInfo
        (
            SPI Action,
            uint Param, 
            ref int  result, 
            int updateIni
        );

        [DllImport(SystemDLL.NAME)]
        public static extern int SystemIdleTimerReset();

        [DllImport(SystemDLL.NAME)]
        public static extern int CeRunAppAtTime(string application, SystemTime startTime);
        
        [DllImport(SystemDLL.NAME)]
        public static extern int CeRunAppAtEvent(string application, int EventID);

        [DllImport(SystemDLL.NAME)]
        public static extern int FileTimeToSystemTime(ref long lpFileTime, SystemTime lpSystemTime);
        
        [DllImport(SystemDLL.NAME)]
        public static extern int FileTimeToLocalFileTime(ref long lpFileTime, ref long lpLocalFileTime);

        // For named events
        //[DllImport(SystemDLL.NAME, SetLastError = true)]
        //internal static extern bool EventModify(IntPtr hEvent, EVENT ef);

        //[DllImport(SystemDLL.NAME, SetLastError = true)]
        //internal static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        //[DllImport(SystemDLL.NAME, SetLastError = true)]
        //internal static extern bool CloseHandle(IntPtr hObject);

        //[DllImport(SystemDLL.NAME, SetLastError = true)]
        //internal static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        //[DllImport(SystemDLL.NAME, SetLastError = true)]
        //internal static extern int WaitForMultipleObjects(int nCount, IntPtr[] lpHandles, bool fWaitAll, int dwMilliseconds);
    }
}
