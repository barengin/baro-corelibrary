using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.AAA
{
    public static class Extensions
    {
        public static UserData? GetSingle(this UserData[] userData, string key)
        {
            // Demek ki sonuç dönmemiş. Yapılan sorgu sonucu boş.
            if (userData == null)
                return null;

            foreach (var item in userData)
            {
                if (item.Key == key)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
