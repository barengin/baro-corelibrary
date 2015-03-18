using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed class ReusableThread
    {
        private Thread _t;
        private AutoResetEvent _jobEvent = new AutoResetEvent(false);
        private Action _jobAction;

        private volatile bool _working = false;

        public ReusableThread()
        {
            _t = new Thread(new ThreadStart(_job));
        }

        public void Start(Action job)
        {
            _jobAction = job;
            _working = true;
            
            _jobEvent.Set();
        }

        private void _job()
        {
            WaitForAJob();

            try
            {
                _jobAction();
            }
            finally
            {
                _working = false;
            }
        }

        private void WaitForAJob()
        {
            _jobEvent.WaitOne();
        }
    }
}
