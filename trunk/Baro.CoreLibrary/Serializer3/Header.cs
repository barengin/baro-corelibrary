using Baro.CoreLibrary.Core;
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

        public int MessageId { get; set; }

        public uint UniqueId1 { get; set; }
        public uint UniqueId2 { get; set; }
        public uint UniqueId3 { get; set; }
        public uint UniqueId4 { get; set; }

        public uint UniqueIdCRC { get; set; }

        public uint Inbox { get; set; }

        public long ExpireDate { get; set; }

        public UniqueID ToUniqueID()
        {
            return new UniqueID(this.UniqueId1, this.UniqueId2, this.UniqueId3, this.UniqueId4);
        }

        public Header FromRawData(ArraySegment<byte> data)
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

            return this;
        }

        public ArraySegment<byte> ToRawData()
        {
            DataWriter w = new DataWriter(40);

            w.WriteInt32(this.Size);
            w.WriteInt32(this.MessageId);
            w.WriteInt32((int)this.UniqueId1);
            w.WriteInt32((int)this.UniqueId2);
            w.WriteInt32((int)this.UniqueId3);
            w.WriteInt32((int)this.UniqueId4);
            w.WriteInt32((int)this.UniqueIdCRC);
            w.WriteInt64(this.ExpireDate);
            w.WriteInt32((int)this.Inbox);

            return new ArraySegment<byte>(w.GetBuffer(), 0, (int)w.Length);
        }

        public int MessageSize
        {
            get { return 40; }
        }
    }
}
