using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Baro.CoreLibrary.Collections;
using Baro.CoreLibrary.Serializer2;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed class SendQueue: IEnumerable<Message>
    {
        private const int MAXITEMS = 2000;

        private readonly SynchQueue<Message> _q = new SynchQueue<Message>();
        private readonly string _folder;

        public EventHandler OnEnqueue;
        public EventHandler OnDequeue;

        private volatile bool _completed = false;
        private EventWaitHandle _wh = new AutoResetEvent(false);

        public int Count { get { return _q.Count; } }

        public void Completed()
        {
            _completed = true;
            _wh.Set();
        }

        public void MakeUnCompleted()
        {
            _completed = false;
            _wh.Set();
        }

        public bool Dequeue(out Message value)
        {
            bool ok;

            do
            {
                if (_completed)
                {
                    value = null;
                    return false;
                }

                ok = DequeueInternal(out value);

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

        public void Enqueue(Message value, bool saveToDisk)
        {
            //if (_completed)
            //    throw new InvalidOperationException("SendQueue.Completed edilmiş");

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            EnqueueInternal(value, saveToDisk);
            _wh.Set();
        }

        public SendQueue(string folder)
        {
            _folder = folder;
            Load();
        }

        public bool Peek(out Message value)
        {
            bool ok;

            do
            {
                if (_completed)
                {
                    value = null;
                    return false;
                }

                ok = _q.Peek(out value);

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

        private bool DequeueInternal(out Message m)
        {
            bool r = _q.Dequeue(out m);

            if (r)
            {
                string f = Path.Combine(_folder, m.GetMessageHeader().GetMsgID().ToString() + ".msg");

                if (File.Exists(f))
                    File.Delete(f);

                Save();

                if (OnDequeue != null)
                    OnDequeue(this, EventArgs.Empty);
            }

            return r;
        }

        private void EnqueueInternal(Message m, bool saveToDisk)
        {
            if (_q.Count == MAXITEMS)
            {
                Message dummy;
                DequeueInternal(out dummy);
            }

            _q.Enqueue(m);

            if (saveToDisk)
            {
                Message.SaveToFile(m, Path.Combine(_folder, m.GetMessageHeader().GetMsgID().ToString() + ".msg"));
                Save();
            }

            if (OnEnqueue != null)
                OnEnqueue(this, EventArgs.Empty);
        }

#if PocketPC || WindowsCE
        public void Load()
        {
            lock (_q.SynchLock)
            {
                if (File.Exists(Path.Combine(_folder, "queue.txt")))
                {
                    using (StreamReader sr = new StreamReader(Path.Combine(_folder, "queue.txt")))
                    {
                        while (!sr.EndOfStream)
                        {
                            string r = sr.ReadLine();
                            Message m;

                            try
                            {
                                m = Message.LoadFromFile(Path.Combine(_folder, r + ".msg"));
                            }
                            catch
                            {
                                continue;
                            }

                            _q.Enqueue(m);
                        }
                    }
                }
            }
        }

        public void Save()
        {
            lock (_q.SynchLock)
            {
                Message[] m = _q.ToArray();

                using (StreamWriter sw = new StreamWriter(Path.Combine(_folder, "queue.txt")))
                {
                    for (int i = 0; i < m.Length; i++)
                    {
                        string s = m[i].GetMessageHeader().GetMsgID().ToString();
                        sw.WriteLine(s);
                    }
                }
            }
        }
#else
        public void Load()
        {
            if (File.Exists(Path.Combine(_folder, "queue.txt")))
            {
                using (StreamReader sr = new StreamReader(Path.Combine(_folder, "queue.txt")))
                {
                    while (!sr.EndOfStream)
                    {
                        string r = sr.ReadLine();
                        Message m;

                        try
                        {
                            string filename = Path.Combine(_folder, r + ".msg");

                            if (File.Exists(filename))
                            {
                                m = Message.LoadFromFile(filename);
                                _q.Enqueue(m);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
        }

        public void Save()
        {
            Message[] m = _q.ToArray();

            _q.SynchLock.EnterWriteLock();

            try
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(_folder, "queue.txt")))
                {
                    for (int i = 0; i < m.Length; i++)
                    {
                        string s = m[i].GetMessageHeader().GetMsgID().ToString();
                        sw.WriteLine(s);
                    }
                }
            }
            finally
            {
                _q.SynchLock.ExitWriteLock();
            }
        }
#endif

        #region IEnumerable<Message> Members

        public IEnumerator<Message> GetEnumerator()
        {
            return _q.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _q.GetEnumerator();
        }

        #endregion

        public bool isCompleted { get { return _completed; } }
    }
}
