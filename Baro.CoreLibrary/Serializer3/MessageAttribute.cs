using System;
using System.Collections.Generic;
using System.Linq;
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
}
