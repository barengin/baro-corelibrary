using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    public static class MessageUtils
    {
        public static object CreateObject(Int32 MessageID)
        {
            Type t = MessageRegistration.Get(MessageID);
            return CreateObject(t);
        }

        private static object CreateObject(Type t)
        {
            return Activator.CreateInstance(t);
        }
    }
}
