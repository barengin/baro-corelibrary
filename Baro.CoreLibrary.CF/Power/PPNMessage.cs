using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Power
{
    public enum PPNMessage : int
    {
        PPN_REEVALUATESTATE = 1,
        PPN_POWERCHANGE = 2,
        PPN_UNATTENDEDMODE = 3,
        PPN_SUSPENDKEYPRESSED = 4,
        PPN_POWERBUTTONPRESSED = 4,
        PPN_SUSPENDKEYRELEASED = 5,
        PPN_APPBUTTONPRESSED = 6,

    }
}
