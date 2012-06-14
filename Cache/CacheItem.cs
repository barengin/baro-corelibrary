using System;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.Collections.Generics;

namespace Baro.CoreLibrary.Cache
{
    public unsafe sealed class CacheItem<TCacheType> : IPQueueNode
    {
        internal int CacheCode { get; set; }
        internal CacheItem<TCacheType> NextCacheItem { get; set; }
        internal CacheItem<TCacheType> PrevCacheItem { get; set; }

        public TCacheType Cache { get; set; }

        #region IPQueueNode Members

        public int PQPriority
        {
            get;
            set;
        }

        public int PQPosition
        {
            get;
            set;
        }

        #endregion
    }
}
