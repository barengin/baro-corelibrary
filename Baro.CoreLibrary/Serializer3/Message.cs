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
        private Header _header;
        private object _object;

        public object Obj { get { return _object; } }
        public Header Header { get { return _header; } }

        private Message()
        {
        }

        public ArraySegment<byte> ToRawData()
        {
            IMessageSerializer s = (IMessageSerializer)_object;

            int totalSize = s.MessageSize + _header.MessageSize;
            byte[] data = new byte[totalSize];

            ArraySegment<byte> hArray = _header.ToRawData();
            ArraySegment<byte> oArray = s.ToRawData();

            Buffer.BlockCopy(hArray.Array, hArray.Offset, data, 0, hArray.Count);
            Buffer.BlockCopy(oArray.Array, oArray.Offset, data, hArray.Count, oArray.Count);

            return new ArraySegment<byte>(data, 0, data.Length);
        }

        // TODO: Aşağıdaki methodlar içinde ARRAY'ler cache'lenebilir.
        // Her defasında yeniden oluşturulmak zorunda kalmazlar.

        public static Message Create(ArraySegment<byte> fullArray)
        {
            Message m = new Message();

            Header h = (Header)new Header().FromRawData(fullArray);

            m._header = h;

            m._object = MessageUtils.CreateObject(h.MessageId);
            IMessageSerializer s = (IMessageSerializer)m._object;

            ArraySegment<byte> d = new ArraySegment<byte>(fullArray.Array, fullArray.Offset + h.MessageSize,
                                                                           fullArray.Count - h.MessageSize);
            s.FromRawData(d);

            return m;
        }

        public static Message Create(Header h, ArraySegment<byte> oArray)
        {
            Message m = new Message();
            m._header = h;

            m._object = MessageUtils.CreateObject(h.MessageId);
            IMessageSerializer s = (IMessageSerializer)m._object;
            s.FromRawData(oArray);

            return m;
        }

        public static Message Create(ArraySegment<byte> hArray, IMessageSerializer o)
        {
            Header h = (Header)new Header().FromRawData(hArray);

            Message m = new Message();
            m._header = h;
            m._object = o;

            return m;
        }

        public static Message Create(ArraySegment<byte> hArray, ArraySegment<byte> oArray)
        {
            Header h = (Header)new Header().FromRawData(hArray);

            Message m = new Message();
            m._header = h;

            m._object = MessageUtils.CreateObject(h.MessageId);
            IMessageSerializer s = (IMessageSerializer)m._object;
            s.FromRawData(oArray);

            return m;
        }

        public static Message Create(Header h, IMessageSerializer o)
        {
            Message m = new Message();
            m._header = h;
            m._object = o;
            return m;
        }

        public static Message Create(IMessageSerializer o, uint toInbox)
        {
            Header h = new Header();
            h.Inbox = toInbox;

            Message m = new Message();
            m._header = h;
            m._object = o;

            return m;
        }

        public static Message Create(IMessageSerializer o, uint toInbox, int ttl)
        {
            Header h = new Header();
            h.Inbox = toInbox;
            h.ExpireDate = DateTime.Now.Ticks + TimeSpan.FromHours(ttl).Ticks;

            Message m = new Message();
            m._header = h;
            m._object = o;

            return m;
        }

        public static Message Create(IMessageSerializer o, uint toInbox, int ttl, bool cacheInReceived, bool cacheInSent)
        {
            Header h = new Header();
            h.Inbox = toInbox;
            h.ExpireDate = DateTime.Now.Ticks + TimeSpan.FromHours(ttl).Ticks;
            h.CacheInReceivedMessages = cacheInReceived;
            h.CacheInSentMessages = cacheInSent;

            Message m = new Message();
            m._header = h;
            m._object = o;

            return m;
        }

        public static Header ParseHeader(ArraySegment<byte> hArray)
        {
            Header h = new Header();
            h.FromRawData(hArray);
            return h;
        }
    }
}
