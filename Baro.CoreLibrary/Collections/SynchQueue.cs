using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.Collections
{
    public class SynchQueue<T>
    {
        private Queue<T> _queue = new Queue<T>();

#if PocketPC || WindowsCE
        private object m_synch = new object();

        public object SynchLock { get { return m_synch; } }
#else
        private ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
        
        public ReaderWriterLockSlim SynchLock { get { return m_lock; } }
#endif

        private void WriteLocked(Action f)
        {
#if PocketPC || WindowsCE
            lock (m_synch)
            {
                f();
            }
#else
            m_lock.EnterWriteLock();

            try
            {
                f();
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
#endif
        }

        private void ReadLocked(Action f)
        {
#if PocketPC || WindowsCE
            lock (m_synch)
            {
                f();
            }
#else
            m_lock.EnterReadLock();

            try
            {
                f();
            }
            finally
            {
                m_lock.ExitReadLock();
            }
#endif
        }

        private TResult ReadLocked<TResult>(Func<TResult> f)
        {
#if PocketPC || WindowsCE
            lock (m_synch)
            {
                return f();
            }
#else
            m_lock.EnterReadLock();

            try
            {
                return f();
            }
            finally
            {
                m_lock.ExitReadLock();
            }
#endif
        }

        public bool Peek(out T value)
        {
            T insideValue = default(T);

            bool r = ReadLocked<bool>(() =>
                {
                    if (_queue.Count == 0)
                        return false;

                    insideValue = _queue.Peek();
                    return true;
                });

            value = insideValue;
            return r;
        }

#if PocketPC || WindowsCE
        public bool Dequeue(out T value)
        {
            value = default(T);

            lock (m_synch)
            {
                if (_queue.Count == 0)
                    return false;

                value = _queue.Dequeue();
                return true;
            }
        }
#endif

        public T Dequeue()
        {
            T result = default(T);

            WriteLocked(() =>
            {
                result = _queue.Dequeue();
            });

            return result;
        }

        public void Enqueue(T value)
        {
            WriteLocked(() => _queue.Enqueue(value));
        }

        public void Clear()
        {
            WriteLocked(() => _queue.Clear());
        }

        public int Count
        {
            get
            {
                return ReadLocked<int>(() => _queue.Count);
            }
        }

        public T[] ToArray()
        {
            return ReadLocked<T[]>(() => _queue.ToArray());
        }
    }
}
