using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.Collections
{
    public class SynchBiDictionary<Key, Value>
    {
        private Dictionary<Key, Value> m_keyDict;
        private Dictionary<Value, Key> m_valueDict;

#if PocketPC || WindowsCE
        private object m_synch = new object();
#else
        private ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
#endif

        public SynchBiDictionary()
        {
            m_keyDict = new Dictionary<Key, Value>();
            m_valueDict = new Dictionary<Value, Key>();
        }

        public SynchBiDictionary(int capacity)
        {
            m_keyDict = new Dictionary<Key, Value>(capacity);
            m_valueDict = new Dictionary<Value, Key>(capacity);
        }

        public SynchBiDictionary(int capacity, IEqualityComparer<Key> keyEqualityComparer, IEqualityComparer<Value> valueEqualityComparer)
        {
            m_keyDict = new Dictionary<Key, Value>(capacity, keyEqualityComparer);
            m_valueDict = new Dictionary<Value, Key>(capacity, valueEqualityComparer);
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
            return ReadLocked<KeyValuePair<Key, Value>[]>(() => m_keyDict.ToArray<KeyValuePair<Key, Value>>());
        }

        public void Clear()
        {
            WriteLocked(() => { m_keyDict.Clear(); m_valueDict.Clear(); });
        }

        public bool ContainsKey(Key key)
        {
            return ReadLocked<bool>(() => m_keyDict.ContainsKey(key));
        }

        public bool ContainsValue(Value value)
        {
            return ReadLocked<bool>(() => m_valueDict.ContainsKey(value));
        }

        public void RemoveKey(Key key)
        {
            WriteLocked(() =>
            {
                Value v;
                
                if (m_keyDict.TryGetValue(key, out v))
                {
                    m_keyDict.Remove(key);
                    m_valueDict.Remove(v);
                }
            });
        }

        public void RemoveValue(Value value)
        {
            WriteLocked(() =>
            {
                Key k;

                if (m_valueDict.TryGetValue(value, out k))
                {
                    m_keyDict.Remove(k);
                    m_valueDict.Remove(value);
                }
            });
        }

        public void Add(Key key, Value value)
        {
            WriteLocked(() => 
            { 
                m_keyDict.Add(key, value);
                m_valueDict.Add(value, key);
            });
        }

        public Value this[Key key]
        {
            get { return ReadLocked<Value>(() => m_keyDict[key]); }
        }

        public Key this[Value value]
        {
            get { return ReadLocked<Key>(() => m_valueDict[value]); }
        }

        public bool TryGetValue(Key k, out Value v)
        {
            Value val = default(Value);
            bool r = ReadLocked<bool>(() => { return m_keyDict.TryGetValue(k, out val); });
            v = val;
            return r;
        }

        public bool TryGetKey(Value v, out Key k)
        {
            Key key = default(Key);
            bool r = ReadLocked<bool>(() => { return m_valueDict.TryGetValue(v, out key); });
            k = key;
            return r;
        }
    }
}
