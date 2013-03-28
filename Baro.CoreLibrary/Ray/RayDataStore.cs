using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public sealed class RayDataStore : RayItem<RayDataStore>, IRayQuery<string>, IEnumerable<KeyValuePair<string, string>>
    {
        #region Flyweight of SortedList
        private SortedList<string, string> _flylist = null;

        private SortedList<string, string> _list
        {
            get { return _flylist ?? (_flylist = new SortedList<string, string>()); }
        }

        #endregion

        #region cTors
        internal RayDataStore()
        {
        }

        #endregion

        public void Clear()
        {
            WriterLock(() =>
            {
                _list.Clear();
            });

            NotifySuccessor(IDU.Delete, ObjectHierarchy.DataStore, null, null);
        }

        public bool Remove(string key)
        {
            bool r = WriterLock<bool>(() => _list.Remove(key));
            if (r) NotifySuccessor(IDU.Delete, ObjectHierarchy.DataStore, null, key);
            return r;
        }

        public string this[string index]
        {
            get
            {
                return ReaderLock<string>(() =>
                {
                    try
                    {
                        return _list[index];
                    }
                    catch (KeyNotFoundException)
                    {
                        return null;
                    }
                });
            }
            set
            {
                WriterLock(() => _list[index] = value);
                NotifySuccessor(IDU.Update, ObjectHierarchy.DataStore, null, new KeyValuePair<string, string>(index, value));
            }
        }

        public IEnumerable<string> SelectAll()
        {
            return ReaderLock<IEnumerable<string>>(() => _list.Values);
        }

        public IEnumerable<KeyValuePair<string, string>> StartsWithKey(string keyValue)
        {
            return ReaderLock<IEnumerable<KeyValuePair<string, string>>>(() => Utils.StartsWith(_list, keyValue));
        }

        public IEnumerable<string> StartsWith(string value)
        {
            return ReaderLock<IEnumerable<string>>(() => Utils.StartsWith<string>(_list, value));
        }

        public IEnumerable<string> Like(string value)
        {
            return ReaderLock<IEnumerable<string>>(() => from kvp in _list
                                                         where kvp.Key.Contains(value)
                                                         select kvp.Value);
        }

        public override RayDataStore Clone()
        {
            RayDataStore d = new RayDataStore();

            ReaderLock(() =>
                {
                    foreach (var item in _list)
                    {
                        d._list.Add(item.Key, item.Value);
                    }
                }
            );

            return d;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ReaderLock<IEnumerator<KeyValuePair<string, string>>>(() => _list.GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ReaderLock<System.Collections.IEnumerator>(() => _list.GetEnumerator());
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            XmlNode n = xmlDoc.CreateElement("data");

            ReaderLock(() =>
                {
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
                });

            return n;
        }
    }
}
