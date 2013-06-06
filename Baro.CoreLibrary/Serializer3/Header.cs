using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    public sealed class Header: IMessageContent, IMessageContent<Header>
    {
        public int Size { get; set; }

        public int CommandId { get; set; }

        public int UniqueId1 { get; set; }
        public int UniqueId2 { get; set; }
        public int UniqueId3 { get; set; }
        public int UniqueId4 { get; set; }

        public int UniqueIdCRC { get; set; }

        public int Inbox { get; set; }

        public long ExpireDate { get; set; }

        public Header FromRawData(RawData data)
        {
            // Header h = new Header();

            DataReader r = new DataReader(data);

            this.Size = r.ReadInt32();
            this.CommandId = r.ReadInt32();
            this.UniqueId1 = r.ReadInt32();
            this.UniqueId2 = r.ReadInt32();
            this.UniqueId3 = r.ReadInt32();
            this.UniqueId4 = r.ReadInt32();
            this.UniqueIdCRC = r.ReadInt32();
            this.ExpireDate = r.ReadInt64();
            this.Inbox = r.ReadInt32();

            return this;
        }

        public RawData ToRawData()
        {
            DataWriter w = new DataWriter(40);

            w.WriteInt32(this.Size);
            w.WriteInt32(this.CommandId);
            w.WriteInt32(this.UniqueId1);
            w.WriteInt32(this.UniqueId2);
            w.WriteInt32(this.UniqueId3);
            w.WriteInt32(this.UniqueId4);
            w.WriteInt32(this.UniqueIdCRC);
            w.WriteInt64(this.ExpireDate);
            w.WriteInt32(this.Inbox);

            RawData d = new RawData() { Index = 0 };
            d.Data = w.GetBuffer();
            d.Count = (int)w.Length;

            return d;
        }

        public int MessageSize
        {
            get { return 40; }
        }
    }
}
