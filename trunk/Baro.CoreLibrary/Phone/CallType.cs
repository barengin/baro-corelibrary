using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Phone
{
#if PocketPC || WindowsCE
    public enum CallType
    {
        Missed,
        Incoming,
        Outgoing
    }
#endif
}
