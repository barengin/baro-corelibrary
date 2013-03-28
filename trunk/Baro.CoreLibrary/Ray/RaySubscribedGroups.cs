using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public class RaySubscribedGroups: RayItem<RaySubscribedGroups>, IList<string>
    {
        #region Flyweight of list
        private List<string> _flylist = new List<string>();

        private List<string> _list
        {
            get { return _flylist ?? (_flylist = new List<string>()); }
        }

        #endregion

        public int IndexOf(string item)
        {
            return ReaderLock<int>(() => _list.IndexOf(item));
        }

        public void Insert(int index, string item)
        {
            WriterLock(() => _list.Insert(index, item));
        }

        public void RemoveAt(int index)
        {
            WriterLock(() => _list.RemoveAt(index));
        }

        public string this[int index]
        {
            get
            {
                return ReaderLock<string>(() => _list[index]);
            }
            set
            {
                WriterLock(() => _list[index] = value);
            }
        }

        public void Add(string item)
        {
            WriterLock(() => _list.Add(item));
        }

        public void Clear()
        {
            WriterLock(() => _list.Clear());
        }

        public bool Contains(string item)
        {
            return ReaderLock<bool>(() => _list.Contains(item));
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            ReaderLock(() => _list.CopyTo(array, arrayIndex));
        }

        public int Count
        {
            get { return ReaderLock<int>(() => _list.Count); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            return WriterLock<bool>(() => _list.Remove(item));
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ReaderLock<IEnumerator<string>>(() => _list.GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ReaderLock<System.Collections.IEnumerator>(() => _list.GetEnumerator());
        }

        public override RaySubscribedGroups Clone()
        {
            RaySubscribedGroups l = new RaySubscribedGroups();

            ReaderLock(() =>
                {
                    foreach (var item in _list)
                    {
                        l.Add(item);
                    }
                });

            return l;
        }

        public XmlNode CreateXmlNode(XmlDocument xmlDoc, string elementName)
        {
            XmlNode n = xmlDoc.CreateElement(elementName);

            ReaderLock(() =>
            {
                foreach (var item in _list)
                {
                    XmlNode a = xmlDoc.CreateElement("item");

                    XmlAttribute t = xmlDoc.CreateAttribute("name");
                    t.Value = item;
                    n.AppendChild(a);
                }
            });

            return n;
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            return CreateXmlNode(xmlDoc, "list");
        }

        protected override void Handle(IDU op, ObjectHierarchy where, string info, object value)
        {
            NotifySuccessor(op, where, info, value);
        }
    }
}
