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

        public TimeSpan Peek()
        {
            return new TimeSpan(0, 0, 0, 0, Environment.TickCount - _actual);
        }
    }
}
