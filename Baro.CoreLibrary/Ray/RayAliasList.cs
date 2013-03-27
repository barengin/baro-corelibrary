using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public class RayAliasList : RayItem<RayAliasList>, IList<string>
    {
        #region Flyweight of list
        private List<string> _flylist = null;

        private List<string> _list
        {
            get { return _flylist ?? (_flylist = new List<string>()); }
        }

        #endregion

        public override RayAliasList Clone()
        {
            RayAliasList a = new RayAliasList();

            ReaderLock(() =>
                {
                    foreach (var item in _list)
                    {
                        a._list.Add(item);
                    }
                }
            );

            return a;
        }

        public int IndexOf(string item)
        {
            return ReaderLock<int>(() => _list.IndexOf(item));
        }

        public void Insert(int index, string item)
        {
            WriterLock(() => _list.Insert(index, item));
            NotifySuccessor(IDU.Insert, ObjectHierarchy.AliasList, null, item);
        }

        public void RemoveAt(int index)
        {
            WriterLock(() => _list.RemoveAt(index));
            NotifySuccessor(IDU.Delete, ObjectHierarchy.AliasList, null, index);
        }

        public string this[int index]
        {
            get
            {
                return ReaderLock<string>(() => _list[index]);
            }
            set
            {
                throw new NotSupportedException(); // WriterLock(() => _list[index] = value);
            }
        }

        public void Add(string item)
        {
            WriterLock(() => _list.Add(item));
            NotifySuccessor(IDU.Insert, ObjectHierarchy.AliasList, null, item);
        }

        public void Clear()
        {
            WriterLock(() => _list.Clear());
            NotifySuccessor(IDU.Delete, ObjectHierarchy.AliasList, null, null);
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
            get
            {
                return ReaderLock<int>(() => _list.Count);
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            bool r = WriterLock<bool>(() => _list.Remove(item));
            if (r) NotifySuccessor(IDU.Delete, ObjectHierarchy.AliasList, null, item);
            return r;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ReaderLock<IEnumerator<string>>(() => _list.GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ReaderLock<System.Collections.IEnumerator>(() => _list.GetEnumerator());
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            XmlNode n = xmlDoc.CreateElement("aliases");

            ReaderLock(() =>
                {
                    foreach (var item in _list)
                    {
                        XmlNode a = xmlDoc.CreateElement("a");

                        XmlAttribute t = xmlDoc.CreateAttribute("name");
                        t.Value = item;
                        n.AppendChild(a);
                    }
                });

            return n;
        }

        protected override void Handle(IDU op, ObjectHierarchy where, object key, object value)
        {
            throw new NotSupportedException();
        }
    }
}
