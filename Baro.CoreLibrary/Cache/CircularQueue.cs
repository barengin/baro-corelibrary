using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Cache
{
    public sealed class CircularQueue<TCacheType>
    {
        private readonly int m_queueSize;
        private CacheItem<TCacheType> m_root;
        private Dictionary<int, CacheItem<TCacheType>> m_dictionary;

        public CircularQueue(int capacity)
        {
            m_queueSize = capacity;
            m_root = null;
            m_dictionary = new Dictionary<int, CacheItem<TCacheType>>(capacity);
        }

        public int Count
        {
            get
            {
                return m_dictionary.Count;
            }
        }

        public bool Get(int pageCode, out CacheItem<TCacheType> cache)
        {
            return m_dictionary.TryGetValue(pageCode, out cache);
        }

        public void Add(int pageCode, CacheItem<TCacheType> cache)
        {
            cache.CacheCode = pageCode;

            if (m_root != null)
            {
                if (Count == m_queueSize)
                {
                    RemoveMostUnncessary();
                }

                // Add
                cache.PrevCacheItem = m_root;
                m_root.NextCacheItem.PrevCacheItem = cache;
                cache.NextCacheItem = m_root.NextCacheItem;
                m_root.NextCacheItem = cache;
            }
            else
            {
                cache.PrevCacheItem = cache;
                cache.NextCacheItem = cache;
                m_root = cache;
            }

            // Hızlı erişim için dictionary içine koy
            m_dictionary.Add(pageCode, cache);
        }

        public void RemoveMostUnncessary()
        {
            // Gereksiz bir taneyi çıkart
            DropOut(m_root);
            m_root = m_root.PrevCacheItem;
        }

        public void DropOut(CacheItem<TCacheType> c)
        {
            m_dictionary.Remove(c.CacheCode);

            if (Count == 0)
            {
                m_root = null;
                return;
            }

            c.PrevCacheItem.NextCacheItem = c.NextCacheItem;
            c.NextCacheItem.PrevCacheItem = c.PrevCacheItem;
        }

        public void ClearAll()
        {
            m_dictionary.Clear();
            m_root = null;
        }
    }
}
