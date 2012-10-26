using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed class LastActivity
    {
        private volatile int _actual;

        public LastActivity()
        {
            Reset();
        }

        public void Reset()
        {
            _actual = Environment.TickCount;
        }

        public int Peek()
        {
            return Environment.TickCount - _actual;
        }

        public static explicit operator TimeSpan(LastActivity f)
        {
            return new TimeSpan(0, 0, 0, 0, f.Peek());
        }
    }
}
