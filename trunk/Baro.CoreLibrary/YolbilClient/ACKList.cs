using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Baro.CoreLibrary.Collections;
using Baro.CoreLibrary.SockServer;

namespace Baro.CoreLibrary.YolbilClient
{
    internal sealed class ACKList
    {
        private SynchList<PredefinedCommands.Ack2> _list = new SynchList<PredefinedCommands.Ack2>();
        private AutoResetEvent _event = new AutoResetEvent(false);
        private AckComparer _comparer = new AckComparer();
        private bool _completed = false;

        public void UnCompleted()
        {
            _completed = false;
        }

        public void Completed()
        {
            _completed = true;
            _event.Set();
        }

        public void Add(PredefinedCommands.Ack2 ack2)
        {
            _list.Add(ack2);
            _event.Set();
        }

        public bool WaitForAck2(PredefinedCommands.Ack2 ack2, uint miliseconds)
        {

        checkAgain:
            
            if (_completed)
                return false;

            if (_list.Contains(ack2, _comparer))
            {
                _list.Remove(ack2);
                return true;
            }
            else
            {
                if (_event.WaitOne((int)miliseconds, false))
                {
                    // Ok
                    goto checkAgain;
                }
                else
                {
                    // Timeout
                    return false;
                }
            }
        }
    }
}
