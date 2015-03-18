using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public class RayDuplicateUserException: Exception
    {
        public RayDuplicateUserException()
            : base()
        {
        }

        public RayDuplicateUserException(string message)
            : base(message)
        {
        }
    }
}
