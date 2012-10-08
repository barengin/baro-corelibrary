using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.YolbilClient
{
    internal sealed class ClientState
    {
        public byte[] receiveBuffer = new byte[2048];
    }
}
