using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.Core
{
    public sealed class SerializedTimer
    {
        private Timer _timer;
        private TimerCallback _cb;
        private int _due;
        private int _period;

        public SerializedTimer(TimerCallback cb, int due, int period)
        {
            _cb = cb;
            _due = due;
            _period = period;
        }

        public void Start(object state)
        {
            _timer = new Timer(_cb, state, _due, _period);
        }

        public void Stop()
        {
            DisposeTimer();
        }

        private void DisposeTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}
