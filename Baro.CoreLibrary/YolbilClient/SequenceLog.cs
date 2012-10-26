using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed class SequenceLog
    {
        private int _sequence;
        private Action<string> _logCb;

        public SequenceLog(Action<string> logCb)
        {
            _logCb = logCb;
        }

        public void Log(string message)
        {
            Interlocked.Increment(ref _sequence);
            _logCb(string.Format("#{0} - {1}", _sequence, message));
        }
    }
}
