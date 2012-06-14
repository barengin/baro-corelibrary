using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Serializer2
{
    public sealed class Message
    {
        public readonly static int MESSAGE_INTERNAL_HEADER_SIZE = Marshal.SizeOf(typeof(MessageInternalHeader));

        private const int UNCOMPRESS_BUFFER_POOL_SIZE = 32;
        private const int UNCOMPRESS_BUFFER_SIZE = 1024 * 24;

        internal const int CRC_MAGIC_NUMBER = 0x4A8A4AAA;

        private readonly static Type[] s_registeredMessages = new Type[65536];
        private readonly static MessageSerializer s_serializer = new MessageSerializer();
        private readonly static MessageDeserializer s_deserializer = new MessageDeserializer();
        
        // Support for ZLib
        // private readonly static UncompressBufferPool s_uncompressBufferPool = new UncompressBufferPool(UNCOMPRESS_BUFFER_POOL_SIZE, UNCOMPRESS_BUFFER_SIZE);

        // Support for ZLib
        // internal static UncompressBufferPool UncompressBufferPool { get { return s_uncompressBufferPool; } }

        private readonly MessageInternalHeader m_header;
        private readonly byte[] m_data;
        private readonly int m_size;

        public int Size { get { return m_size; } }
        public byte[] Data { get { return m_data; } }
        public MessageInfo GetMessageInfo() { return m_header.CreateMessageInfo(); }
        public MessageInternalHeader GetMessageHeader() { return m_header; }

        internal Message(MessageInternalHeader header, byte[] data, int size)
        {
            m_data = data;
            m_header = header;
            m_size = size;
        }

        public static void ClearAllRegisteredMessageTypes()
        {
            lock (s_registeredMessages)
            {
                for (int k = 0; k < 65536; k++)
                    s_registeredMessages[(ushort)k] = null;
            }
        }

        public static void UnRegisterMessageType(Type t)
        {
            UInt16 id = MessageAttribute.GetMessageID(t);

            lock (s_registeredMessages)
            {
                s_registeredMessages[id] = null;
            }
        }

        public static void RegisterMessageType(Type t)
        {
            UInt16 id = MessageAttribute.GetMessageID(t);

            lock (s_registeredMessages)
            {
                if (s_registeredMessages[id] != null)
                    throw new InvalidOperationException("Bu id ile bir komut zaten kaydedilmiş");

                s_registeredMessages[id] = t;
            }
        }

        internal static Type GetTypeFromID(UInt16 id)
        {
            Type tmp;

            lock(s_registeredMessages)
            {
                tmp = s_registeredMessages[id];
            }

            if (tmp == null)
            {
                throw new MessageNotRegisteredException("Bu ID ile bir mesaj sisteme kaydedilmemiş. " + id.ToString());
            }

            return tmp;
        }

#if PocketPC || WindowsCE
        public static Message Create(MessageInfo info, object obj, bool compress, MessageKey key)
#else
        public static Message Create(MessageInfo info, object obj, bool compress, MessageKey key = null)
#endif
        {
            return s_serializer.Serialize(info, obj, compress, key);
        }

#if PocketPC || WindowsCE
        public static object Parse(byte[] data, out MessageInternalHeader header, MessageKey key)
#else
        public static object Parse(byte[] data, out MessageInternalHeader header, MessageKey key = null)
#endif
        {
            return s_deserializer.Deserialize(data, out header, key);
        }

#if PocketPC || WindowsCE
        public static object Parse(byte[] data, MessageInternalHeader header, MessageKey key)
#else
        public static object Parse(byte[] data, MessageInternalHeader header, MessageKey key = null)
#endif
        {
            return s_deserializer.Deserialize(header, data, key);
        }

        public static MessageInternalHeader GetInternalHeader(byte[] data)
        {
            return s_deserializer.GetHeader(data);
        }
    }
}
