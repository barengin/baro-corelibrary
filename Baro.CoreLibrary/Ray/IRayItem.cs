using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    interface IRayItem<T>: ICloneable
    {
        new T Clone();
    }
}
