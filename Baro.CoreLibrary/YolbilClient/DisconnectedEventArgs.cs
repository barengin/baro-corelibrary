using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.YolbilClient
{
    public class DisconnectedEventArgs: EventArgs
    {
        public Exception DisconnectReason { get; internal set; }
    }
}
