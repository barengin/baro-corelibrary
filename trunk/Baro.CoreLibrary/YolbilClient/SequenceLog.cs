using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed class SequenceLog
    {
        private int _sequence = 1;

        public string Log(string message)
        {
            Interlocked.Increment(ref _sequence);
            return string.Format("#{0} - {1}", _sequence, message);
        }
    }
}
