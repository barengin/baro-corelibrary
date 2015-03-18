using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray.DataSource
{
    internal class DataSourceProxy : IRayDataSource
    {
        private IRayDataSource _source;

        public DataSourceProxy(IRayDataSource source)
        {
            _source = source;
        }

        public void InsertUser(RayUser user)
        {
            if (_source != null)
            {
                _source.InsertUser(user);
            }
        }

        public void UpdateUser(RayUser user)
        {
            if (_source != null)
            {
                _source.UpdateUser(user);
            }
        }

        public void DeleteUser(string username)
        {
            if (_source != null)
            {
                _source.DeleteUser(username);
            }
        }

        public RayUser ReadUser(string username, RayUserList listToAdd)
        {
            if (_source != null)
            {
                return _source.ReadUser(username, listToAdd);
            }
            else
            {
                return null;
            }
        }

        public void InsertGroup(RayGroup group)
        {
            if (_source != null)
            {
                _source.InsertGroup(group);
            }
        }

        public void UpdateGroup(RayGroup group)
        {
            if (_source != null)
            {
                _source.UpdateGroup(group);
            }
        }

        public void DeleteGroup(string groupname)
        {
            if (_source != null)
            {
                _source.DeleteGroup(groupname);
            }
        }

        public RayGroup ReadGroup(string groupname, RayGroupList listToAdd)
        {
            if (_source != null)
            {
                return _source.ReadGroup(groupname, listToAdd);
            }
            else
            {
                return null;
            }
        }

        public string[] GetUserList()
        {
            if (_source != null)
            {
                return _source.GetUserList();
            }
            else
            {
                return null;
            }
        }

        public string[] GetGroupList()
        {
            if (_source != null)
            {
                return _source.GetGroupList();
            }
            else
            {
                return null;
            }
        }


        public void ClearUsers()
        {
            if (_source != null)
            {
                _source.ClearUsers();
            }
        }

        public void ClearGroups()
        {
            if (_source != null)
            {
                _source.ClearGroups();
            }
        }
    }
}
