using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.Collections
{
    [Obsolete]
    public class SynchDictionary<Key, Value>
    {
        private Dictionary<Key, Value> m_dict;

#if PocketPC || WindowsCE
        private object m_synch = new object();
#else
        private ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
#endif

        public Dictionary<Key, Value> AsynchDictionary { get { return m_dict; } }

        public SynchDictionary()
        {
            m_dict = new Dictionary<Key, Value>();
        }
        
        public SynchDictionary(int capacity)
        {
            m_dict = new Dictionary<Key, Value>(capacity);
        }

        public SynchDictionary(int capacity, IEqualityComparer<Key> eqComparer)
        {
            m_dict = new Dictionary<Key, Value>(capacity, eqComparer);
        }

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

        public KeyValuePair<Key, Value>[] ToArray()
        {
            return ReadLocked<KeyValuePair<Key, Value>[]>(() => m_dict.ToArray<KeyValuePair<Key, Value>>());
        }

        public void Clear()
        {
            WriteLocked(() => m_dict.Clear());
        }
        
        public bool Contains(Key key)
        {
            return ReadLocked<bool>(() => m_dict.ContainsKey(key));
        }

        public void Remove(Key key)
        {
            WriteLocked(() => m_dict.Remove(key));
        }

        public void Add(Key key, Value value)
        {
            WriteLocked(() => m_dict.Add(key, value));
        }

        public Value this[Key token]
        {
            get { return ReadLocked<Value>(() => m_dict[token]); }
        }

        public bool TryGetValue(Key k, out Value v)
        {
            Value val = default(Value);
            bool r = ReadLocked<bool>(() => { return m_dict.TryGetValue(k, out val); });
            v = val;
            return r;
        }
    }
}
