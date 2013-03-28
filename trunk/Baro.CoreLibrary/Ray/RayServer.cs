using Baro.CoreLibrary.Ray.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public sealed class RayServer: RayItem<RayServer>
    {
        private IRayDataSource _dataSource;
        private RayUserList _userList = new RayUserList();
        private RayGroupList _groupList = new RayGroupList();

        #region cTors
        /// <summary>
        /// In-memory configuration
        /// </summary>
        public RayServer()
        {
            _dataSource = new DataSourceProxy(null);
        }

        /// <summary>
        /// In-memory mapped datasource
        /// </summary>
        /// <param name="dataSource">Data source of RayServer</param>
        public RayServer(IRayDataSource dataSource)
        {
            _dataSource = new DataSourceProxy(dataSource);
        }

        #endregion

        #region Users
        public RayUser GetUser(string username)
        {
            return _userList.GetByName(username);
        }

        public RayUser GetUserByAlias(string alias)
        {
            return _userList.GetByAlias(alias);
        }

        public RayUser AddUser(string username)
        {
            RayUser u = new RayUser(username, this);

            _userList.Add(u);

            return u;
        }

        public void AddUser(RayUser user)
        {
            _userList.Add(user);
        }

        public bool RemoveUser(string username)
        {
            return _userList.Remove(username);
        }

        public void RemoveAllUsers()
        {
            _userList.RemoveAllUsers();
        }

        #endregion
        
        public override RayServer Clone()
        {
            throw new NotSupportedException();
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            throw new NotSupportedException();
        }

        protected override void Handle(IDU op, ObjectHierarchy where, string info, object value)
        {
            // TODO: Alt nesnelerde değişim var. Gerekli bilgiler de geliyor ancak bu sadece bizim dizini kaydetmemiz
            //       için bir işaret.
            //
            // KAYDET !!!

            switch (where)
            {
                case ObjectHierarchy.AliasList:
                    break;
                case ObjectHierarchy.DataStore:
                    break;
                case ObjectHierarchy.Permission:
                    break;
                case ObjectHierarchy.PermissionList:
                    break;
                case ObjectHierarchy.Group:
                    break;
                case ObjectHierarchy.GroupList:
                    break;
                case ObjectHierarchy.Username:
                    break;
                case ObjectHierarchy.Password:
                    break;
                default:
                    break;
            }
        }

        public RayGroup GetGroup(string groupName)
        {
            return _groupList[groupName];
        }
    }
}
