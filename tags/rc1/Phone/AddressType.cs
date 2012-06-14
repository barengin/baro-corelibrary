using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Phone
{
#if PocketPC || WindowsCE
    public enum AddressType
    {
        Unknown,
        International,
        National,
        NetworkSpecific,
        Subscriber,
        Alphanumeric,
        Abbreviated
    }
#endif
}
