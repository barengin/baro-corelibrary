using System;

using System.Collections.Generic;
using System.Text;


namespace Baro.CoreLibrary.GdiPlus
{
    internal class GdiPlusStatusException:Exception
    {
        GdiPlusStatusException(GpStatus status, string msg)
            : base(msg)
        {
            Status = status;
        }

        public GpStatus Status { get; private set; }

        public static Exception Exception(GpStatus status)
        {
            return new GdiPlusStatusException(status,status.ToString());
        }
    }

}
