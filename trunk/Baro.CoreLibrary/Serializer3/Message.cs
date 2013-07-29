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

        public object Msg { get { return _object; } }
        public Header Header { get { return _header; } }

        private Message()
        {
        }

        public static Message Create(ArraySegment<byte> fullArray)
        {
            return null;
        }

        public static Message Create(Header h, ArraySegment<byte> oArray)
        {
            Message m = new Message();
            m._header = h;

            m._object = MessageUtils.CreateObject(h.MessageId);
            IMessageSerializer
            return null;
        }

        public static Message Create(ArraySegment<byte> hArray, IMessageSerializer o)
        {
            return null;
        }

        public static Message Create(ArraySegment<byte> hArray, ArraySegment<byte> oArray)
        {
            return null;
        }

        public static Message Create(Header h, IMessageSerializer o)
        {
            return null;
        }

        public static Message Create(IMessageSerializer o, uint toInbox)
        {
            return null;
        }

        public static Message Create(IMessageSerializer o, uint toInbox, int ttl)
        {
            return null;
        }

        public static Message Create(IMessageSerializer o, uint toInbox, bool cacheInReceived, bool cacheInSent)
        {
            return null;
        }

        public static Header ParseHeader(ArraySegment<byte> hArray)
        {
            return null;
        }
    }
}
