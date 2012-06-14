using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Device
{
#if PocketPC || WindowsCE
    public static class DeviceUniqueHash
    {
        /*
        HRESULT GetDeviceUniqueID(
          LPBYTE pbApplicationData,
          DWORD cbApplictionData,
          DWORD dwDeviceIDVersion,
          LPBYTE pbDeviceIDOutput,
          DWORD* pcbDeviceIDOutput
        );
        */
        [DllImport(SystemDLL.NAME)]
        private extern static int GetDeviceUniqueID([In, Out] byte[] appdata,
                                                    int cbApplictionData,
                                                    int dwDeviceIDVersion,
                                                    [In, Out] byte[] deviceIDOuput,
                                                    ref uint pcbDeviceIDOutput);

        /// <summary>
        /// Device için özel ID üretir. Bu method sadece Windows Mobile 5 ve üzerinde çalışır.
        /// </summary>
        /// <param name="AppString">Bu bilgi 8 karakter ve üstü olmalıdır.</param>
        /// <returns>ID</returns>
        public static byte[] GetDeviceID(string AppString)
        {
            // Call the GetDeviceUniqueID
            byte[] AppData = new byte[AppString.Length];

            for (int count = 0; count < AppString.Length; count++)
                AppData[count] = (byte)AppString[count];
            
            int appDataSize = AppData.Length;
            
            byte[] DeviceOutput = new byte[40];
            uint SizeOut = (uint)DeviceOutput.Length;
            
            GetDeviceUniqueID(AppData, appDataSize, 1, DeviceOutput, ref SizeOut);
            
            byte[] result = new byte[SizeOut];
            Buffer.BlockCopy(DeviceOutput, 0, result, 0, (int)SizeOut);

            return result;
        }
    }
#endif
}
