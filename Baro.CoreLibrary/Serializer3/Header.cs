using Baro.CoreLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    public sealed class Header: IMessageSerializer
    {
        public int Size { get; set; }

        public int MessageId { get; set; }

        public uint UniqueId1 { get; set; }
        public uint UniqueId2 { get; set; }
        public uint UniqueId3 { get; set; }
        public uint UniqueId4 { get; set; }

        public uint UniqueIdCRC { get; set; }

        public uint Inbox { get; set; }
        public long ExpireDate { get; set; }

        /// <summary>
        /// Sunucudan gelen bu mesaj Received listesi içinde depolansın mı?
        /// </summary>
        public bool CacheInReceivedMessages { get; set; }

        /// <summary>
        /// Sunucuya giden bu mesaj Sent listesi içinde depolansın mı?
        /// </summary>
        public bool CacheInSentMessages { get; set; }

        public Header()
        {
            CacheInReceivedMessages = MessageSettings.CacheInReceivedMessages;
            CacheInSentMessages = MessageSettings.CacheInSentMessages;
            ExpireDate = DateTime.Now.Ticks + TimeSpan.FromHours(MessageSettings.TTL).Ticks;
        }

        public UniqueID ToUniqueID()
        {
            return new UniqueID(this.UniqueId1, this.UniqueId2, this.UniqueId3, this.UniqueId4);
        }

        private void writeCache(byte field)
        {
            CacheInReceivedMessages = (field | 1) == 1;
            CacheInSentMessages = (field | 2) == 2;
        }

        private byte readCache()
        {
            int r = 0;

            r = CacheInReceivedMessages ? 1 : 0;
            r |= CacheInSentMessages ? 2 : 0;

            return (byte)r;
        }

        public object FromRawData(ArraySegment<byte> data)
        {
            // Header h = new Header();

            DataReader r = new DataReader(data);

            this.Size = r.ReadInt32();
            this.MessageId = r.ReadInt32();
            this.UniqueId1 = (uint)r.ReadInt32();
            this.UniqueId2 = (uint)r.ReadInt32();
            this.UniqueId3 = (uint)r.ReadInt32();
            this.UniqueId4 = (uint)r.ReadInt32();
            this.UniqueIdCRC = (uint)r.ReadInt32();
            this.ExpireDate = r.ReadInt64();
            this.Inbox = (uint)r.ReadInt32();
            writeCache(r.ReadByte());

            return this;
        }

        public ArraySegment<byte> ToRawData()
        {
            DataWriter w = new DataWriter();

            w.WriteInt32(this.Size);
            w.WriteInt32(this.MessageId);
            w.WriteInt32((int)this.UniqueId1);
            w.WriteInt32((int)this.UniqueId2);
            w.WriteInt32((int)this.UniqueId3);
            w.WriteInt32((int)this.UniqueId4);
            w.WriteInt32((int)this.UniqueIdCRC);
            w.WriteInt64(this.ExpireDate);
            w.WriteInt32((int)this.Inbox);
            w.WriteByte(readCache());

            return new ArraySegment<byte>(w.GetBuffer(), 0, (int)w.Length);
        }

        public int MessageSize
        {
            get { return 41; }
        }
    }
}
