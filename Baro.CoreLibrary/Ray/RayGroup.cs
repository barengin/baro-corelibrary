using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public sealed class RayGroup: RayItem<RayGroup>
    {
        private string _name;
        private RayPermissionList _permissions;

        public string Name { get { return _name; } }
        public RayPermissionList Permissions { get { return _permissions; } }

        #region cTors
        private RayGroup()
        {
            throw new NotSupportedException();

            // DİKKAT !!! _permission nesnesini yaratma !!!
            //
            // 
            // _permissions = new RayPermissionList();
        }

        internal RayGroup(string groupName, RayHandler successor)
        {
            _name = groupName;
            _permissions = new RayPermissionList();
            SetSuccessor(successor);
        }

        #endregion

        public override RayGroup Clone()
        {
            throw new NotSupportedException();

            //RayGroup g = new RayGroup();

            //g._name = this.Name;
            //g._permissions = this.Permissions.Clone();

            //return g;
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

        protected override void Handle(IDU op, ObjectHierarchy where, string info, object value)
        {
            switch (where)
            {
                case ObjectHierarchy.Permission:
                    NotifySuccessor(IDU.Update, ObjectHierarchy.Group, null, this);
                    break;
                
                case ObjectHierarchy.PermissionList:
                    NotifySuccessor(IDU.Update, ObjectHierarchy.Group, null, this);
                    break;

                default:
                    break;
            }
        }
    }
}
