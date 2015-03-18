using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Configuration
{
    public class ConfigRepositoryException: Exception
    {
        public ConfigRepositoryException()
            : base()
        {
        }

        public ConfigRepositoryException(string message)
            : base(message)
        {
        }

        public ConfigRepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
