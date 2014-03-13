using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.Collections
{
    public class SynchList<T> : IList<T>
    {
        private List<T> m_list = new List<T>();

#if PocketPC || WindowsCE
        private object m_synch = new object();
#else
        private ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
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

        public int IndexOf(T item)
        {
            return ReadLocked<int>(() => (m_list.IndexOf(item)));
        }

        public void Insert(int index, T item)
        {
            WriteLocked(() => { m_list.Insert(index, item); });
        }

        public void RemoveAt(int index)
        {
            WriteLocked(() => { m_list.RemoveAt(index); });
        }

        public T this[int index]
        {
            get
            {
                return ReadLocked<T>(() => (m_list[index]));
            }
            set
            {
                WriteLocked(() => { m_list[index] = value; });
            }
        }

        public void Add(T item)
        {
            WriteLocked(() => { m_list.Add(item); });
        }

        public void Clear()
        {
            WriteLocked(() => { m_list.Clear(); });
        }

        public bool Contains(T item)
        {
            return ReadLocked<bool>(() => (m_list.Contains(item)));
        }

        public bool Contains(T item, IEqualityComparer<T> eq)
        {
            return ReadLocked<bool>(() => (m_list.Contains(item, eq)));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ReadLocked(() => { m_list.CopyTo(array, arrayIndex); });
        }

        public T[] ToArrayAndEmpty()
        {
            T[] array = default(T[]);

            WriteLocked(() =>
                {
                    array = m_list.ToArray();
                    m_list.Clear();
                });

            return array;
        }

        public T[] ToArray()
        {
            return ReadLocked<T[]>(() =>
            {
                return m_list.ToArray();
            });
        }

        public int Count
        {
            get { return ReadLocked<int>(() => (m_list.Count)); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return ReadLocked<bool>(() => (m_list.Remove(item)));
        }

        public IEnumerator<T> GetEnumerator()
        {
            // return m_list.GetEnumerator();
            throw new NotSupportedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // return m_list.GetEnumerator();
            throw new NotSupportedException();
        }

        public void Enqueue(T item)
        {
            WriteLocked(() => { m_list.Add(item); });
        }

#if PocketPC || WindowsCE
        public bool Dequeue(out T item)
        {
            lock (m_synch)
            {
                if (m_list.Count == 0)
                {
                    item = default(T);
                    return false;
                }

                int lastItem = m_list.Count - 1;
                item = m_list[lastItem];
                m_list.RemoveAt(lastItem);

                return true;
            }
        }
#else
        public bool Dequeue(out T item)
        {
            m_lock.EnterWriteLock();

            try
            {
                if (m_list.Count == 0)
                {
                    item = default(T);
                    return false;
                }

                int lastItem = m_list.Count - 1;
                item = m_list[lastItem];
                m_list.RemoveAt(lastItem);

                return true;
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
        }
#endif
    }
}
