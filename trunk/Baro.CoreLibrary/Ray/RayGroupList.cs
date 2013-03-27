using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public class RayGroupList : RayItem<RayGroupList>, IRayQuery<RayGroup>, IEnumerable<RayGroup>
    {
        #region Flyweight of List
        private SortedList<string, RayGroup> _flylist = null;

        private SortedList<string, RayGroup> _list
        {
            get { return _flylist ?? (_flylist = new SortedList<string, RayGroup>()); }
        }

        #endregion

        public void Add(RayGroup group)
        {
            WriterLock(() => _list.Add(group.Name, group));
        }

        public void Remove(RayGroup group)
        {
            WriterLock(() => _list.Remove(group.Name));
        }

        public void Clear()
        {
            WriterLock(() => _list.Clear());
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
                    l._list.Add(item.Key, item.Value);
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
