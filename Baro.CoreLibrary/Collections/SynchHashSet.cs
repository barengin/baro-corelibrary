using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.Collections
{
    public class SynchHashSet<T>
    {
        private HashSet<T> m_hash;
        private ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();

        public SynchHashSet()
        {
            m_hash = new HashSet<T>();
        }

        public SynchHashSet(IEqualityComparer<T> eqComparer)
        {
            m_hash = new HashSet<T>(eqComparer);
        }

        #region Locking
        private void WriteLocked(Action f)
        {
            m_lock.EnterWriteLock();

            try
            {
                f();
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
        }

        private TResult WriteLocked<TResult>(Func<TResult> f)
        {
            m_lock.EnterWriteLock();

            try
            {
                return f();
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
        }

        private void ReadLocked(Action f)
        {
            m_lock.EnterReadLock();

            try
            {
                f();
            }
            finally
            {
                m_lock.ExitReadLock();
            }
        }

        private TResult ReadLocked<TResult>(Func<TResult> f)
        {
            m_lock.EnterReadLock();

            try
            {
                return f();
            }
            finally
            {
                m_lock.ExitReadLock();
            }
        }

        #endregion

        public void Clear()
        {
            WriteLocked(() => m_hash.Clear());
        }

        public bool Add(T value)
        {
            return WriteLocked<bool>(() => m_hash.Add(value));
        }

        public bool Contains(T value)
        {
            return ReadLocked<bool>(() => m_hash.Contains(value));
        }

        public bool Remove(T value)
        {
            return WriteLocked<bool>(() => m_hash.Remove(value));
        }
    }
}
