using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MessageAttribute : Attribute
    {
        public Int32 ID { get; set; }

        public static MessageAttribute GetMessageAttribute(Type t)
        {
            object[] attr = t.GetCustomAttributes(typeof(MessageAttribute), true);

            if (attr == null || attr.Length == 0)
            {
                throw new Exception("Bu nesne MessageAttribute ile işaretlenmemiş!");
            }

            return (attr[0] as MessageAttribute);
        }

        public static MessageAttribute GetMessageAttribute(object o)
        {
            return GetMessageAttribute(o.GetType());
        }

        public static Int32 GetMessageID(Type t)
        {
            return GetMessageAttribute(t).ID;
        }

        public static Int32 GetMessageID(object o)
        {
            return GetMessageAttribute(o).ID;
        }
    }

    public static class MessageRegistration
    {
        private readonly static ConcurrentDictionary<Int32, Type> _registeredMessages = new ConcurrentDictionary<Int32, Type>();

        public static void ClearAllRegisteredMessageTypes()
        {
            _registeredMessages.Clear();
        }

        public static void UnRegisterMessageType(Type t)
        {
            Int32 id = MessageAttribute.GetMessageID(t);

            _registeredMessages.TryRemove(id, out t);
        }

        public static void RegisterMessageType(Type t)
        {
            Int32 id = MessageAttribute.GetMessageID(t);

            if (!_registeredMessages.TryAdd(id, t))
            {
                if (_registeredMessages[id] != null)
                    throw new InvalidOperationException("Bu id ile bir komut zaten kaydedilmiş");
            }
        }

        public static object CreateObject(Int32 MessageID)
        {
            Type t = _registeredMessages[MessageID];
            return CreateObject(t);
        }

        public static object CreateObject(Type t)
        {
            Int32 id = MessageAttribute.GetMessageID(t);
            return Activator.CreateInstance(t);
        }
    }
}
