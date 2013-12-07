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

        public ushort Year { get { return _year; } set { _year = value; } }
        public ushort Month { get { return _month; } set { _month = value; } }
        public ushort DayOfWeek { get { return _dayOfWeek; } }
        public ushort Day { get { return _day; } set { _day = value; } }
        public ushort Hour { get { return _hour; } set { _hour = value; } }
        public ushort Minute { get { return _minute; } set { _minute = value; } }
        public ushort Second { get { return _second; } set { _second = value; } }
        public ushort Milliseconds { get { return _miliseconds; } set { _miliseconds = value; } }

        [DllImport(SystemDLL.NAME)]
        private static extern void GetLocalTime(ref SystemTime sysTime);

        [DllImport(SystemDLL.NAME)]
        private static extern void SetLocalTime(ref SystemTime sysTime);

        [DllImport(SystemDLL.NAME)]
        private static extern void GetSystemTime(ref SystemTime sysTime);

        [DllImport(SystemDLL.NAME)]
        private static extern void SetSystemTime(ref SystemTime sysTime);

        public DateTime ToDateTimeUTC()
        {
            return new DateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second, DateTimeKind.Utc);
        }

        public static void SetSystemTime(SystemTime sysTime)
        {
            SetSystemTime(ref sysTime);
        }

        public static SystemTime GetSystemTime()
        {
            SystemTime s = new SystemTime();
            GetSystemTime(ref s);
            return s;
        }

        public static void SetLocalTime(SystemTime sysTime)
        {
            SetLocalTime(ref sysTime);
        }

        public static SystemTime GetLocalTime()
        {
            SystemTime s = new SystemTime();
            GetLocalTime(ref s);
            return s;
        }
    }
}
