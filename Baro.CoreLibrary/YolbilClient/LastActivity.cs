using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed class LastActivity
    {
        private object _synch = new object();
        private int _actual;

        public LastActivity()
        {
            Reset();
        }

        public void Reset()
        {
            lock (_synch)
            {
                _actual = Environment.TickCount;
            }
        }

        public TimeSpan Peek()
        {
            lock (_synch)
            {
                return new TimeSpan(0, 0, 0, 0, Environment.TickCount - _actual);
            }
        }
    }
}
