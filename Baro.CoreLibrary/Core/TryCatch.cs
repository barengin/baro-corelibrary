using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Core
{
    public static class Try
    {
        public static void Catch(Action _try, Action<Exception> _catch)
        {
            try
            {
                _try();
            }
            catch (Exception exception)
            {
                _catch(exception);
            }
        }
    }
}
