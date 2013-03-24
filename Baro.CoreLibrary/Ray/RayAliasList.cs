using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public class RayAliasList: RayItem<RayAliasList>, IList<string>
    {
        private List<string> _list = new List<string>();

        public override RayAliasList Clone()
        {
            RayAliasList a = new RayAliasList();

            foreach (var item in _list)
            {
                a._list.Add(item);
            }

            return a;
        }

        public int IndexOf(string item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public void Add(string item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(string item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            return _list.Remove(item);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            XmlNode n = xmlDoc.CreateElement("aliases");

            foreach (var item in _list)
            {
                XmlNode a = xmlDoc.CreateElement("a");
                
                XmlAttribute t = xmlDoc.CreateAttribute("name");
                t.Value = item;
                n.AppendChild(a);
            }

            return n;
        }
    }
}
