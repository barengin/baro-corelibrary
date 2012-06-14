using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Phone
{
#if PocketPC || WindowsCE
    public enum CallLogSeek
    {
        Beginning = 2,
        End = 4
    }
#endif
}
