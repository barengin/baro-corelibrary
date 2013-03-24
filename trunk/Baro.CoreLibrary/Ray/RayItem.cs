using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public abstract class RayItem<T>: IRayItem<T>
    {
        public abstract T Clone();

        public abstract XmlNode CreateXmlNode(XmlDocument xmlDoc);

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
