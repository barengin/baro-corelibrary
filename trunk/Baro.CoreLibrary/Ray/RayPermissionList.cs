using Baro.CoreLibrary.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public sealed class RayPermissionList : RayItem<RayPermissionList>, IRayQuery<RayPermission>, IEnumerable<RayPermission>
    {
        #region Flyweight of List
        private SortedList<string, RayPermission> _flylist = null;

        private SortedList<string, RayPermission> _list
        {
            get { return _flylist ?? (_flylist = new SortedList<string, RayPermission>()); }
        }

        #endregion

        public void Add(RayPermission p)
        {
            WriterLock(() => _list.Add(p.Key, p));
            p.SetSuccessor(this);
            NotifySuccessor(IDU.Insert, ObjectHierarchy.PermissionList, null, p);
        }

        public void Remove(string key)
        {
            RayPermission p;

            if (_list.TryGetValue(key, out p))
            {
                p.SetSuccessor(null);
                WriterLock(() => _list.Remove(key));
                NotifySuccessor(IDU.Delete, ObjectHierarchy.PermissionList, null, key);
            }
        }

        public void Clear()
        {
            WriterLock(() => _list.Clear());

            ReaderLock(() =>
            {
                foreach (var item in this)
                {
                    item.SetSuccessor(null);
                }
            });
            
            NotifySuccessor(IDU.Delete, ObjectHierarchy.PermissionList, null, null);
        }

        public RayPermission GetPermission(string index)
        {
            return ReaderLock<RayPermission>(() =>
            {
                try
                {
                    return _list[index];
                }
                catch(KeyNotFoundException)
                {
                    return null;
                }
            });
        }

        public RayPermission this[string index]
        {
            get
            {
                try
                {
                    return ReaderLock<RayPermission>(() => _list[index]);
                }
                catch (KeyNotFoundException)
                {
                    throw new RayKeyNotFoundException("Key not found: " + index);
                }
            }
            set
            {
                throw new NotSupportedException("This method is read-only. You can use Add(Permission) method instead");
            }
        }

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

        public IEnumerator<RayPermission> GetEnumerator()
        {
            return ReaderLock<IEnumerator<RayPermission>>(() => _list.Values.GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ReaderLock<IEnumerator<RayPermission>>(() => _list.Values.GetEnumerator());
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

        protected override void Handle(IDU op, ObjectHierarchy where, string info, object value)
        {
            // TODO: Burada aşağıdan gelen bilgiyi yukarı gönderiyoruz. Ancak filtreleme gerekebilir???
            NotifySuccessor(op, where, info, value);
        }
    }
}
