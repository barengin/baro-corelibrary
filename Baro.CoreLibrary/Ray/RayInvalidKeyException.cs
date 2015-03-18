using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public class RayInvalidKeyException: Exception
    {
        public RayInvalidKeyException()
            : base()
        {
        }

        public RayInvalidKeyException(string message)
            : base(message)
        {
        }
    }
}
