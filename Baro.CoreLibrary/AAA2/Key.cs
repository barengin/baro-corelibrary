using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Baro.CoreLibrary.AAA2
{
    sealed class Key
    {
        private ConcurrentDictionary<string, User2> _keys = new ConcurrentDictionary<string, User2>(Environment.ProcessorCount, 20000);

        public bool Add(string value, User2 user)
        {
            return _keys.TryAdd(value, user);
        }

        public bool Get(string value, out User2 user)
        {
            return _keys.TryGetValue(value, out user);
        }
    }
}
