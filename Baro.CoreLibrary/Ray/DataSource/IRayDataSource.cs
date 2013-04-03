using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray.DataSource
{
    public interface IRayDataSource
    {
        void InsertUser(RayUser user);
        void UpdateUser(RayUser user);
        void DeleteUser(string username);
        RayUser ReadUser(string username, RayUserList listToAdd);

        void InsertGroup(RayGroup group);
        void UpdateGroup(RayGroup group);
        void DeleteGroup(string groupname);
        RayGroup ReadGroup(string groupname, RayGroupList listToAdd);

        string[] GetUserList();
        string[] GetGroupList();

        void ClearUsers();
        void ClearGroups();
    }
}
