using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.Serializer2;
using Baro.CoreLibrary.Collections;
using System.IO;
using System.Threading;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed class SendQueue
    {
        private readonly SynchQueue<Message> _q = new SynchQueue<Message>();
        private readonly string _folder;

        public EventHandler OnEnqueue;
        public EventHandler OnDequeue;

        public int Count { get { return _q.Count; } }

        public SendQueue(string folder)
        {
            _folder = folder;
            Load();
        }

        public bool Peek(out Message m)
        {
            return _q.Peek(out m);
        }

        public bool Dequeue(out Message m)
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

        public void Enqueue(Message m, bool saveToDisk)
        {
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
            _q.SynchLock.EnterWriteLock();
            
            try
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
            finally
            {
                _q.SynchLock.ExitWriteLock();
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
    }
}
