using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public class RayGroupList: RayItem<RayGroupList>, IRayQuery<RayGroup>, IEnumerable<RayGroup>
    {
        private SortedList<string, RayGroup> _list = new SortedList<string, RayGroup>();

        public void Add(RayGroup group)
        {
            _list.Add(group.Name, group);
        }

        public void Remove(RayGroup group)
        {
            _list.Remove(group.Name);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public RayGroup this[string index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public IEnumerable<RayGroup> SelectAll()
        {
            return _list.Values;
        }

        public IEnumerable<RayGroup> StartsWith(string value)
        {
            return Utils.StartsWith<RayGroup>(_list, value);
        }

        public IEnumerable<RayGroup> Like(string value)
        {
            return from kvp in _list
                   where kvp.Key.Contains(value)
                   select kvp.Value;
        }

        public override RayGroupList Clone()
        {
            RayGroupList l = new RayGroupList();

            foreach (var item in _list)
            {
                l._list.Add(item.Key, item.Value);
            }

            return l;
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            XmlNode n = xmlDoc.CreateElement("groups");

            foreach (var item in this)
            {
                n.AppendChild(item.CreateXmlNode(xmlDoc));
            }

            return n;
        }

        public IEnumerator<RayGroup> GetEnumerator()
        {
            return _list.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.Values.GetEnumerator();
        }
    }
}
