using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public class RayKeyNotFoundException: Exception
    {
        public RayKeyNotFoundException(string message)
            : base(message)
        {
        }

        public RayKeyNotFoundException()
            : base()
        {
        }
    }
}
