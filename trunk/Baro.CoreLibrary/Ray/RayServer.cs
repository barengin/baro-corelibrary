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
        private RayUserList _userList = new RayUserList();

        /// <summary>
        /// In-memory configuration
        /// </summary>
        public RayServer()
        {
        }

        /// <summary>
        /// In-memory mapped datasource
        /// </summary>
        /// <param name="dataSource">Data source of RayServer</param>
        public RayServer(IRayDataSource dataSource)
        {
        }

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
            RayUser u = new RayUser(username);
            
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
        }
    }
}
