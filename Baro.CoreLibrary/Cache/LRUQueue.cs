using System;
using System.Collections.Generic;
using Baro.CoreLibrary.Collections.Generics;

namespace Baro.CoreLibrary.Cache
{
    internal sealed class LRUQueue<TCacheType>
    {
        private Dictionary<int, CacheItem<TCacheType>> m_Dictionary;
        private PQueue<CacheItem<TCacheType>> m_PQ;

        public LRUQueue(int capacity)
        {
            m_Dictionary = new Dictionary<int, CacheItem<TCacheType>>(capacity);
            m_PQ = new PQueue<CacheItem<TCacheType>>(capacity);
        }

        public int Count
        {
            get
            {
                return m_PQ.Count;
            }
        }

        public bool Get(int cacheCode, out CacheItem<TCacheType> cache)
        {
            return m_Dictionary.TryGetValue(cacheCode, out cache);
        }

        public void Add(int cacheCode, CacheItem<TCacheType> cache)
        {
            // Hızlı erişim için dictionary içine koy
            m_Dictionary.Add(cacheCode, cache);

            cache.CacheCode = cacheCode;

            // Listenin en önünde
            cache.PQPriority = 0;
            m_PQ.Append(cache);
        }

        public void IncreaseUsedCount(CacheItem<TCacheType> c)
        {
            c.PQPriority++;
            m_PQ.IncreaseKey(c);
        }

        public CacheItem<TCacheType> RemoveMostUnnecessary()
        {
            if (m_PQ.Count == 0)
                return null;

            CacheItem<TCacheType> c = m_PQ.ExtractTop();
            m_Dictionary.Remove(c.CacheCode);
            return c;
        }

        public void DropOut(CacheItem<TCacheType> c)
        {
            m_Dictionary.Remove(c.CacheCode);
            m_PQ.Delete(c);
        }

        public void ClearAll()
        {
            m_Dictionary.Clear();
            m_PQ.ClearAll();
        }
    }
}
