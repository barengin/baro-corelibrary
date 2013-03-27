using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    internal class RayUserList
    {
        private ConcurrentDictionary<string, RayUser> _mapUsers = new ConcurrentDictionary<string, RayUser>(Environment.ProcessorCount, 1000);
        private ConcurrentDictionary<string, RayUser> _mapAlias = new ConcurrentDictionary<string, RayUser>(Environment.ProcessorCount, 1000);

        public void RemoveAllUsers()
        {
            _mapAlias.Clear();
            _mapUsers.Clear();
        }

        public void Add(RayUser user)
        {
            _mapUsers.AddOrUpdate(user.Username, user, (key, u) =>
                {
                    return user;
                });

            foreach (var item in user.Aliases)
            {
                _mapAlias.AddOrUpdate(item, user, (key, u) =>
                {
                    return user;
                });                
            }
        }

        public bool Remove(string username)
        {
            RayUser u, u2;
            bool r = _mapUsers.TryRemove(username, out u);

            if (r)
            {
                foreach (var item in u.Aliases)
                {
                    _mapAlias.TryRemove(username, out u2);
                }
            }

            return r;
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
    }
}
