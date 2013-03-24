using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public sealed class RayDataStore: RayItem<RayDataStore>, IRayQuery<string>, IEnumerable<KeyValuePair<string, string>>
    {
        private SortedList<string, string> _list = new SortedList<string, string>();

        public string this[string index]
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

        public IEnumerable<string> SelectAll()
        {
            return _list.Values;
        }

        public IEnumerable<KeyValuePair<string, string>> StartsWithKey(string keyValue)
        {
            return Utils.StartsWith(_list, keyValue);
        }

        public IEnumerable<string> StartsWith(string value)
        {
            return Utils.StartsWith<string>(_list, value);
        }

        public IEnumerable<string> Like(string value)
        {
            return from kvp in _list
                   where kvp.Key.Contains(value)
                   select kvp.Value;
        }

        public override RayDataStore Clone()
        {
            RayDataStore d = new RayDataStore();

            foreach (var item in _list)
            {
                d._list.Add(item.Key, item.Value);
            }

            return d;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            XmlNode n = xmlDoc.CreateElement("data");

            foreach (var item in this)
            {
                XmlNode d = xmlDoc.CreateElement("d");

                XmlAttribute a = xmlDoc.CreateAttribute("key");
                a.Value = item.Key;
                d.Attributes.Append(a);

                a = xmlDoc.CreateAttribute("value");
                a.Value = item.Value;
                d.Attributes.Append(a);

                n.AppendChild(d);
            }

            return n;
        }
    }
}
