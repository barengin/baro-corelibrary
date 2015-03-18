using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    public static class MessageSettings
    {
        /// <summary>
        /// Gönderilen mesajı sunucu üzerinde kaç saat saklanacağını belirtir. Header içerisinde ise
        /// tam olarak tarih ve saat yazılabilir. Parametresiz yaratılan Header nesnesinin içindeki
        /// ExpireDate bu kolona göre güncellenir. Orjinal değer: 48 saat
        /// </summary>
        public static int TTL { get; set; }

        /// <summary>
        /// Sunucudan gelen bu mesaj Received listesi içinde depolansın mı?
        /// Orjinal değer: true
        /// </summary>
        public static bool CacheInReceivedMessages { get; set; }

        /// <summary>
        /// Sunucuya giden bu mesaj Sent listesi içinde depolansın mı?
        /// Orjinal değer: true
        /// </summary>
        public static bool CacheInSentMessages { get; set; }

        /// <summary>
        /// Bütün ayarları orjinal değerlere dönüştürür.
        /// </summary>
        public static void Reset()
        {
            TTL = 48;
            CacheInReceivedMessages = true;
            CacheInSentMessages = true;
        }

        static MessageSettings()
        {
            Reset();
        }
    }
}
