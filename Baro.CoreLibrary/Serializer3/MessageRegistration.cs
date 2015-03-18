using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    public static class MessageRegistration
    {
        private readonly static ConcurrentDictionary<Int32, Type> _registeredMessages = new ConcurrentDictionary<Int32, Type>();

        static MessageRegistration()
        {
        }

        public static void ClearAllRegisteredMessageTypes()
        {
            _registeredMessages.Clear();
        }

        public static bool UnRegisterMessageType(Type t)
        {
            Int32 id = MessageAttribute.GetMessageID(t);

            return _registeredMessages.TryRemove(id, out t);
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

        internal static Type Get(int MessageID)
        {
            return _registeredMessages[MessageID];
        }
    }
}
