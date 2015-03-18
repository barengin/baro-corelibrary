using Baro.CoreLibrary.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public sealed class RayPermissionList : RayItem<RayPermissionList>, 
                                            IRayQuery<RayPermission>, 
                                            IRayBagList<string, RayPermission>, 
                                            IEnumerable<RayPermission>
    {
        #region Flyweight of List
        private SortedList<string, RayPermission> _flylist = null;

        private SortedList<string, RayPermission> _list
        {
            get { return _flylist ?? (_flylist = new SortedList<string, RayPermission>()); }
        }

        #endregion

        #region cTors
        internal RayPermissionList()
        {
        }

        #endregion

        #region IRayBagList
        public RayPermission AddNew(string permission)
        {
            RayPermission p = new RayPermission(permission);
            p.SetSuccessor(this);

            WriterLock(() => _list.Add(p.Key, p));
            NotifySuccessor(IDU.Insert, ObjectHierarchy.PermissionList, null, p);

            return p;
        }

        public bool Remove(string permission)
        {
            RayPermission p;
            bool found = false;

            WriterLock(() =>
            {
                if (found = _list.TryGetValue(permission, out p))
                {
                    p.SetSuccessor(null);
                    _list.Remove(permission);
                }
            });

            if (found) NotifySuccessor(IDU.Delete, ObjectHierarchy.PermissionList, null, permission);
            return found;
        }

        public bool Remove(RayPermission permission)
        {
            return Remove(permission.Key);
        }

        public void Clear()
        {
            WriterLock(() =>
            {
                foreach (var item in this)
                {
                    item.SetSuccessor(null);
                }

                _list.Clear();
            });

            NotifySuccessor(IDU.Delete, ObjectHierarchy.PermissionList, null, null);
        }

        public RayPermission this[string permissionName]
        {
            get
            {
                try
                {
                    return ReaderLock<RayPermission>(() => _list[permissionName]);
                }
                catch (KeyNotFoundException)
                {
                    return null;
                }
            }
            set
            {
                throw new NotSupportedException("This method is read-only. You can use Add(Permission) method instead");
            }
        }

        #endregion

        #region IRayQuery
        public IEnumerable<RayPermission> SelectAll()
        {
            return ReaderLock<IEnumerable<RayPermission>>(() => _list.Values);
        }

        public IEnumerable<RayPermission> StartsWith(string value)
        {
            return ReaderLock<IEnumerable<RayPermission>>(() => Utils.StartsWith<RayPermission>(_list, value));
        }

        public IEnumerable<RayPermission> Like(string value)
        {
            return ReaderLock<IEnumerable<RayPermission>>(() => from kvp in _list
                                                                where kvp.Key.Contains(value)
                                                                select kvp.Value);
        }

        #endregion

        #region IEnumerable
        public IEnumerator<RayPermission> GetEnumerator()
        {
            return ReaderLock<IEnumerator<RayPermission>>(() => _list.Values.GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ReaderLock<IEnumerator<RayPermission>>(() => _list.Values.GetEnumerator());
        }

        #endregion

        public override RayPermissionList Clone()
        {
            RayPermissionList l = new RayPermissionList();

            ReaderLock(() =>
            {
                foreach (var item in this._list)
                {
                    RayPermission p = item.Value.Clone();
                    p.SetSuccessor(l);

                    l._list.Add(p.Key, p);
                }
            });

            return l;
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            XmlNode n = xmlDoc.CreateElement("permissions");

            ReaderLock(() =>
                {
                    foreach (var item in this)
                    {
                        n.AppendChild(item.CreateXmlNode(xmlDoc));
                    }
                });

            return n;
        }
    }
}
