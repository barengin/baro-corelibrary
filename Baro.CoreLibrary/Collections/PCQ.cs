using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.Collections
{
    public class PCQ<T> where T : class
    {
        volatile bool _completed = false;

        EventWaitHandle _wh = new AutoResetEvent(false);
        SynchList<T> _list = new SynchList<T>();

        public void Completed()
        {
            _completed = true;
            _list.Enqueue(null);
            _wh.Set();
        }

        public void Enqueue(T value)
        {
            if (_completed)
                return;

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            _list.Enqueue(value);
            _wh.Set();
        }

        public bool Dequeue(out T value)
        {
            bool ok;

            do
            {
                if (_completed)
                {
                    value = null;
                    return false;
                }

                ok = _list.Dequeue(out value);

                if (ok)
                {
                    return true;
                }
                else
                {
                    _wh.WaitOne();
                }
            } while (true);
        }
    }
}
