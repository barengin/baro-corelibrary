using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public class RayPermission : RayItem<RayPermission>, IComparable<RayPermission>, IEquatable<RayPermission>
    {
        private string _key;
        private volatile bool _allowed;
        private volatile bool _denied;

        internal sealed class Comparer : IComparer<RayPermission>
        {
            public int Compare(RayPermission x, RayPermission y)
            {
                return x.CompareTo(y);
            }
        }

        public RayPermission(string keyName)
        {
            this._key = keyName;
        }

        public bool Inactive
        {
            get { return ((!_allowed) && (!_denied)); }
        }

        public bool Allowed
        {
            get { return _allowed; }
            set
            {
                _allowed = value;

                if (_allowed)
                {
                    Denied = false;
                }

                NotifySuccessor(IDU.Update, ObjectHierarchy.Permission, "Allowed", value);
            }
        }

        public bool Denied
        {
            get { return _denied; }
            set
            {
                _denied = value;

                if (_denied)
                {
                    Allowed = false;
                }
                
                NotifySuccessor(IDU.Update, ObjectHierarchy.Permission, "Denied", value);
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }
        }

        public override string ToString()
        {
            string value;

            if (!this.Allowed && this.Denied)
            {
                value = "denied";
            }
            else if (this.Allowed && !this.Denied)
            {
                value = "allowed";
            } if (!this.Allowed && !this.Denied)
            {
                value = "natural";
            }
            else
            {
                throw new RayInvalidPermissionSettings("Allowed and Denied can not be enabled at the same time !");
            }

            return string.Format("Permission {0} : {1}", this.Key, value);
        }

        public override bool Equals(object obj)
        {
            if (obj is RayPermission)
            {
                return this.Equals((RayPermission)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(RayPermission other)
        {
            return other.Key == this.Key && 
                   other.Allowed == this.Allowed && 
                   other.Denied == this.Denied;
        }

        public override RayPermission Clone()
        {
            return new RayPermission(_key) { Allowed = _allowed, Denied = _denied };
        }

        public static IComparer<RayPermission> CreateComparer()
        {
            return new RayPermission.Comparer();
        }

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        public int CompareTo(RayPermission other)
        {
            return string.Compare(this.Key, other.Key, true);
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            XmlNode p = xmlDoc.CreateElement("permission");

            XmlAttribute a = xmlDoc.CreateAttribute("name");
            a.Value = this.Key;
            p.Attributes.Append(a);

            a = xmlDoc.CreateAttribute("allowed");
            a.Value = this.Allowed ? "1" : "0";
            p.Attributes.Append(a);

            a = xmlDoc.CreateAttribute("denied");
            a.Value = this.Denied ? "1" : "0";
            p.Attributes.Append(a);

            return p;
        }

        protected override void Handle(IDU op, ObjectHierarchy where, string info, object value)
        {
            throw new NotSupportedException();
        }
    }
}
