using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.YolbilClient
{
    /// <summary>
    /// Kullanım şekli gereği bu sınıf asla Thread-Safe olamaz. Çünkü aynı anda sadece tek bir thread tarafından kullanılabilir.
    /// </summary>
    public sealed class LastActivity
    {
        private int _actual;

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
