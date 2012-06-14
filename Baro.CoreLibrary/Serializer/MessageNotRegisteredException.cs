using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.Serializer2
{
    public class MessageNotRegisteredException : Exception
    {
        public MessageNotRegisteredException(string message)
            : base(message)
        {
        }
    }
}
