using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.Serializer2
{
    public class MessageCrcException: Exception
    {
        public MessageCrcException(string message)
            : base(message)
        {
        }
    }
}
