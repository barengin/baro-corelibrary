using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public sealed class RayUserList: RayItem<RayUserList>, IRayBagList<string, RayUser>
    {
        private ConcurrentDictionary<string, RayUser> _mapUsers = new ConcurrentDictionary<string, RayUser>(Environment.ProcessorCount, 1000);
        private ConcurrentDictionary<string, RayUser> _mapAlias = new ConcurrentDictionary<string, RayUser>(Environment.ProcessorCount, 1000);

        internal RayUserList()
        {
        }

        public RayUser GetByAlias(string alias)
        {
            RayUser u;

            if (_mapAlias.TryGetValue(alias, out u))
            {
                return u;
            }
            else
            {
                return null;
            }
        }

        public RayUser GetByName(string username)
        {
            RayUser u;

            if (_mapUsers.TryGetValue(username, out u))
            {
                return u;
            }
            else
            {
                return null;
            }
        }

        public override RayUserList Clone()
        {
            throw new NotImplementedException();
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            XmlNode n = xmlDoc.CreateElement("users");

            ReaderLock(() =>
            {
                foreach (var item in _mapUsers.Values)
                {
                    n.AppendChild(item.CreateXmlNode(xmlDoc));
                }
            });

            return n;
        }

        public RayUser AddNew(string username)
        {
            RayUser u = new RayUser(username, (RayServer)_successor);

            _mapUsers.AddOrUpdate(username, u, (key, update) =>
            {
                return u;
            });

            NotifySuccessor(IDU.Insert, ObjectHierarchy.UserList, null, u);

            return u;
        }

        public bool Remove(string username)
        {
            RayUser u, u2;
            bool r = _mapUsers.TryRemove(username, out u);

            if (r)
            {
                u.SetSuccessor(null);

                foreach (var item in u.Aliases)
                {
                    _mapAlias.TryRemove(username, out u2);
                }

                NotifySuccessor(IDU.Delete, ObjectHierarchy.UserList, null, u);
            }

            return r;
        }

        public bool Remove(RayUser value)
        {
            return Remove(value.Username);
        }

        public void Clear()
        {
            _mapAlias.Clear();
            _mapUsers.Clear();

            NotifySuccessor(IDU.Delete, ObjectHierarchy.UserList, null, null);
        }

        public RayUser this[string username]
        {
            get
            {
                return GetByName(username);
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}
