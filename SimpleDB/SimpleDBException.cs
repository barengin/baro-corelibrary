using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.SimpleDB
{
    public class SimpleDBException: Exception
    {
        public SimpleDBException(string message)
            : base(message)
        {
        }
    }
}
