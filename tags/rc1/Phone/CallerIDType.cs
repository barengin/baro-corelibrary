using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Phone
{
#if PocketPC || WindowsCE
    public enum CallerIDType
    {
        Unavailable,
        Blocked,
        Available
    }
#endif
}
