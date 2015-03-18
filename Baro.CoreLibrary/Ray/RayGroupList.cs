using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public sealed class RayGroupList : RayItem<RayGroupList>, 
                                       IRayQuery<RayGroup>,
                                       IRayBagList<string, RayGroup>,
                                       IEnumerable<RayGroup>
    {
        #region Flyweight of List
        private SortedList<string, RayGroup> _flylist = null;

        private SortedList<string, RayGroup> _list
        {
            get { return _flylist ?? (_flylist = new SortedList<string, RayGroup>()); }
        }

        #endregion

        #region cTor
        internal RayGroupList()
        {
        }

        #endregion

        #region IRayBagList
        public RayGroup AddNew(string key)
        {
            RayGroup g = new RayGroup(key, this);

            WriterLock(() => _list.Add(g.Name, g));

            NotifySuccessor(IDU.Insert, ObjectHierarchy.GroupList, null, g);

            return g;
        }

        public bool Remove(string key)
        {
            RayGroup p;
            bool found = false;

            WriterLock(() =>
            {
                if (found = _list.TryGetValue(key, out p))
                {
                    p.SetSuccessor(null);
                    _list.Remove(key);
                }
            });

            if (found) NotifySuccessor(IDU.Delete, ObjectHierarchy.GroupList, null, key);
            return found;
        }

        public bool Remove(RayGroup value)
        {
            return Remove(value.Name);
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

            NotifySuccessor(IDU.Delete, ObjectHierarchy.GroupList, null, null);
        }

        public RayGroup this[string index]
        {
            get
            {
                return ReaderLock<RayGroup>(() => _list[index]);
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        public IEnumerable<RayGroup> SelectAll()
        {
            return ReaderLock<IEnumerable<RayGroup>>(() => _list.Values);
        }

        public IEnumerable<RayGroup> StartsWith(string value)
        {
            return ReaderLock<IEnumerable<RayGroup>>(() => Utils.StartsWith<RayGroup>(_list, value));
        }

        public IEnumerable<RayGroup> Like(string value)
        {
            return ReaderLock<IEnumerable<RayGroup>>(() => from kvp in _list
                                                           where kvp.Key.Contains(value)
                                                           select kvp.Value);
        }

        public override RayGroupList Clone()
        {
            RayGroupList l = new RayGroupList();

            ReaderLock(() =>
            {
                foreach (var item in _list)
                {
                    RayGroup g = item.Value.Clone();
                    g.SetSuccessor(l);

                    l._list.Add(item.Key, g);
                }
            });

            return l;
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            XmlNode n = xmlDoc.CreateElement("groups");

            ReaderLock(() =>
            {
                foreach (var item in this)
                {
                    n.AppendChild(item.CreateXmlNode(xmlDoc));
                }
            });

            return n;
        }

        public IEnumerator<RayGroup> GetEnumerator()
        {
            return ReaderLock<IEnumerator<RayGroup>>(() => _list.Values.GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ReaderLock<System.Collections.IEnumerator>(() => _list.Values.GetEnumerator());
        }

    }
}
