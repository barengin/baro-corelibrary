using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public class RayPermission : RayItem<RayPermission>, IComparable<RayPermission>
    {
        private string _key;
        private bool _allowed;
        private bool _denied;

        public sealed class Comparer : IComparer<RayPermission>
        {
            public int Compare(RayPermission x, RayPermission y)
            {
                return x.CompareTo(y);
            }
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
                    _denied = false;
                }
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
                    _allowed = false;
                }
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = Utils.ValidateKey(value);
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
            return other.Key == this.Key && other.Allowed == this.Allowed && other.Denied == this.Denied;
        }

        public override RayPermission Clone()
        {
            return new RayPermission() { Allowed = _allowed, Denied = _denied, Key = _key };
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
    }
}
