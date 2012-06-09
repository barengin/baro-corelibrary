using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Power
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VibrateNote
    {
        public short wDuration;
        public byte bAmplitude;
        public byte bFrequency;
    }
}
