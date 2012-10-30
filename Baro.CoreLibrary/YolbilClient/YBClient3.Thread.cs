using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.YolbilClient
{
    partial class YBClient3
    {
        private Thread _thread;
        private volatile bool _threadRunning;

        private bool ThreadIsRunning
        {
            get { return (_thread != null) && _threadRunning; }
        }

        private void threadLoop()
        {
        }

        private void StartThread()
        {
            // Çalışıyorsa durdur
            StopThread();

            // Yenisini başlat
            _threadRunning = true;
            _thread = new Thread(new ThreadStart(threadLoop));
            _thread.Name = "YBClient Thread";
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void StopThread()
        {
            // Açıksa durdur.
            if (_thread != null)
            {
                _threadRunning = false;
                _thread.Join();
            }
        }
    }
}
