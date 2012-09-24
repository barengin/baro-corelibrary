﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Baro.CoreLibrary.AAA2
{
    sealed class Keys
    {
        //                          <KeyName, Key>
        private ConcurrentDictionary<string, Key> _keys = new ConcurrentDictionary<string, Key>(Environment.ProcessorCount, 20);

        public bool TryGetKey(string key, out Key k)
        {
            return _keys.TryGetValue(key, out k);
        }

        //public Key this[string key]
        //{
        //    get
        //    {
        //        return _keys.GetOrAdd(key, (keyToCreate) => { return new Key(); });
        //    }
        //}
    }
}
