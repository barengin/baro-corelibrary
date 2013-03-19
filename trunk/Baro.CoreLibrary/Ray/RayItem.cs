using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public abstract class RayItem<T>: IRayItem<T>
    {
        public abstract T Clone();

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
