using Baro.CoreLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    public sealed class Message
    {
        private readonly IMessageContent _message;
        private readonly object _objectCache;

        public Header Header { get; private set; }

        public int MessageSize
        {
            get
            {
                return Header.MessageSize + _message.MessageSize;
            }
        }

        public Message(MessageInbox info, object message)
        {
            this._message = (IMessageContent)message;

            Header h = new Header();

            h.MessageId = MessageAttribute.GetMessageID(message);
            h.ExpireDate = info.ExpireDate.Ticks;
            h.Inbox = info.Inbox;
            h.Size = h.MessageSize + _message.MessageSize;

            UniqueID ui = UniqueID.CreateNew();

            h.UniqueId1 = ui.Data1;
            h.UniqueId2 = ui.Data2;
            h.UniqueId3 = ui.Data3;
            h.UniqueId4 = ui.Data4;
            h.UniqueIdCRC = ui.Crc;

            this.Header = h;

            _objectCache = message;
        }

        public Message(ArraySegment<byte> fullContent)
        {
            this.Header = new Header().FromRawData(fullContent);

            ArraySegment<byte> n = new ArraySegment<byte>(fullContent.Array,
                                                          fullContent.Offset + this.Header.MessageSize,
                                                          fullContent.Count - this.Header.MessageSize);

            object o = MessageRegistration.CreateObject(this.Header.MessageId);
            _message = o as IMessageContent;

            _objectCache = o.GetType().GetMethod("FromRawData").Invoke(o, new object[] { n });
        }

        public ArraySegment<byte> ToRawData()
        {
            ArraySegment<byte> h = this.Header.ToRawData();
            ArraySegment<byte> o = this._message.ToRawData();

            int size = Header.MessageSize + _message.MessageSize;

            byte[] b = new byte[size];

            Buffer.BlockCopy(h.Array, h.Offset, b, 0, h.Count);
            Buffer.BlockCopy(o.Array, o.Offset, b, h.Count, o.Count);

            return new ArraySegment<byte>(b, 0, size);
        }

        public Object ToObject()
        {
            return _objectCache;
        }
    }
}
