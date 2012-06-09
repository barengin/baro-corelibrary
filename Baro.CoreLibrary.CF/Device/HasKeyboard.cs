using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Baro.CoreLibrary.Device
{
#if PocketPC || WindowsCE
    public static class HasKeyboard
    {
        const string _shellSubkey = @"Software\Microsoft\Shell";
        const string _hasKeyboardRegistryValueName = "HasKeyboard";
        public static bool DeviceHasKeyboard()
        {
            bool hasKeyboard = false;
            RegistryKey shellSubKey = null;
            try
            {
                shellSubKey = Registry.CurrentUser.OpenSubKey(_shellSubkey);
                if (shellSubKey != null)
                {
                    int hasKeyboardRegistryValue =
                        (int)shellSubKey.GetValue(_hasKeyboardRegistryValueName, 0);
                    hasKeyboard = hasKeyboardRegistryValue == 1;
                }
            }
            finally
            {
                if (shellSubKey != null)
                    shellSubKey.Close();
            }

            return hasKeyboard;

        }
    }
#endif
}
