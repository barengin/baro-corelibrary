﻿using System;

namespace Baro.CoreLibrary.WindowsInput.Native
{
#pragma warning disable 649
    /// <summary>
    /// The HARDWAREINPUT structure contains information about a simulated message generated by an input device other than a keyboard or mouse.  (see: http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    public struct HARDWAREINPUT
    {
        /// <summary>
        /// Value specifying the message generated by the input hardware. 
        /// </summary>
        public UInt32 Msg;

        /// <summary>
        /// Specifies the low-order word of the lParam parameter for uMsg. 
        /// </summary>
        public UInt16 ParamL;

        /// <summary>
        /// Specifies the high-order word of the lParam parameter for uMsg. 
        /// </summary>
        public UInt16 ParamH;
    }
#pragma warning restore 649
}
