using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.Ray
{
    public class RayInvalidPermissionSettings : Exception
    {
        public RayInvalidPermissionSettings()
            : base()
        {
        }

        public RayInvalidPermissionSettings(string message)
            : base(message)
        {
        }
    }
}
