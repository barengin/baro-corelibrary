using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public class RayGroup: RayItem<RayGroup>
    {
        private string _name;
        private RayPermissionList _permissions;

        public string Name { get { return _name; } }
        public RayPermissionList Permissions { get { return _permissions; } }

        private RayGroup()
        {
            // DİKKAT !!! _permission nesnesini yaratma !!!
            // 
            // _permissions = new RayPermissionList();
        }

        public RayGroup(string groupName)
        {
            _name = groupName;
            _permissions = new RayPermissionList();
        }

        public override RayGroup Clone()
        {
            RayGroup g = new RayGroup();

            g._name = this.Name;
            g._permissions = this.Permissions.Clone();

            return g;
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            XmlNode n = xmlDoc.CreateElement("group");
            
            XmlAttribute a = xmlDoc.CreateAttribute("name");
            a.Value = this.Name;
            n.Attributes.Append(a);

            n.AppendChild(this.Permissions.CreateXmlNode(xmlDoc));

            return n;
        }
    }
}
