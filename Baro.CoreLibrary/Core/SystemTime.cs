using System;

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        private ushort _year;
        private ushort _month;
        private ushort _dayOfWeek;
        private ushort _day;
        private ushort _hour;
        private ushort _minute;
        private ushort _second;
        private ushort _miliseconds;

        public ushort Year { get { return _year; } }
        public ushort Month { get { return _month; } }
        public ushort DayOfWeek { get { return _dayOfWeek; } }
        public ushort Day { get { return _day; } }
        public ushort Hour { get { return _hour; } }
        public ushort Minute { get { return _minute; } }
        public ushort Second { get { return _second; } }
        public ushort Milliseconds { get { return _miliseconds; } }

        [DllImport(SystemDLL.NAME)]
        private static extern void GetLocalTime(ref SystemTime sysTime);

        public static SystemTime GetLocalTime()
        {
            SystemTime s = new SystemTime();
            GetLocalTime(ref s);
            return s;
        }
    }
}
