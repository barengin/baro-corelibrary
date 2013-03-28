using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    interface IRayBagList<TKey, TType>
    {
        TType AddNew(TKey key);
        
        bool Remove(TKey key);
        bool Remove(TType value);

        void Clear();

        TType this[TKey index] { get; set; }
    }
}
