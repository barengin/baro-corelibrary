using System;

namespace Baro.CoreLibrary.Cache
{
    internal sealed class Q2Algorithm<TCacheType>
    {
        // Percentage of A1 list size
        private const int PERCENTAGE_OF_A1_QUEUE = 20;

        private LRUQueue<TCacheType> m_AmQueue;
        private int m_AmSize;

        private CircularQueue<TCacheType> m_A1Queue;
        private int m_A1Size;

        /// <summary>
        /// Cache Manager sınıfı ile pageBufferSize büyüklüğünde buffer sayfalarından pageCount kadar yaratılmasını sağlar ve
        /// bunları LRUQueue sınıfı ile 2Q algoritmasını kullanarak cache'ler.
        /// </summary>
        /// <param name="pageCount"></param>
        public Q2Algorithm(int pageCount)
        {
            m_A1Size = (pageCount * PERCENTAGE_OF_A1_QUEUE) / 100;

            m_A1Queue = new CircularQueue<TCacheType>(m_A1Size);
            m_AmQueue = new LRUQueue<TCacheType>(m_AmSize = (pageCount - m_A1Size));
        }

        /// <summary>
        /// Eğer istediğimiz page elimizde varsa TRUE geri döner. Eğer yoksa FALSE geri döner.
        /// </summary>
        public bool Get(int pageCode, out CacheItem<TCacheType> cache, out CacheItem<TCacheType> removedCache)
        {
            if (m_AmQueue.Get(pageCode, out cache))
            {
                m_AmQueue.IncreaseUsedCount(cache);
                removedCache = null;
                return true;
            }
            else
                if (m_A1Queue.Get(pageCode, out cache))
                {
                    m_A1Queue.DropOut(cache);

                    if (m_AmQueue.Count == m_AmSize)
                        removedCache = m_AmQueue.RemoveMostUnnecessary();
                    else
                        removedCache = null;

                    m_AmQueue.Add(pageCode, cache);
                    return true;
                }
                else
                {
                    if (m_AmQueue.Count == m_AmSize)
                        removedCache = m_AmQueue.RemoveMostUnnecessary();
                    else
                        removedCache = null;

                    return false;
                }
        }

        public void Add(int pageCode, CacheItem<TCacheType> cache)
        {
            m_A1Queue.Add(pageCode, cache);
        }
    }
}
