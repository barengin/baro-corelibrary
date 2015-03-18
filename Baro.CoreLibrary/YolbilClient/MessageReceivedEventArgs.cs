using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.Serializer2;

namespace Baro.CoreLibrary.YolbilClient
{
    public class MessageReceivedEventArgs: EventArgs
    {
        public MessageHeader Header { get; internal set; }
        public object Message { get; internal set; }
    }
}
