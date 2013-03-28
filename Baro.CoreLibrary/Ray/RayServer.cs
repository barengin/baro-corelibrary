using Baro.CoreLibrary.Ray.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public sealed class RayServer : RayItem<RayServer>
    {
        private IRayDataSource _dataSource;
        private RayUserList _userList = new RayUserList();
        private RayGroupList _groupList = new RayGroupList();

        public RayUserList Users
        {
            get { return _userList; }
        }

        public RayGroupList Groups
        {
            get { return _groupList; }
        }

        #region cTors
        /// <summary>
        /// In-memory configuration
        /// </summary>
        public RayServer() : this(null)
        {
        }

        /// <summary>
        /// In-memory mapped datasource
        /// </summary>
        /// <param name="dataSource">Data source of RayServer</param>
        public RayServer(IRayDataSource dataSource)
        {
            _dataSource = new DataSourceProxy(dataSource);

            _groupList.SetSuccessor(this);
            _userList.SetSuccessor(this);
        }

        #endregion

        new private void SetSuccessor(RayHandler s)
        {
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
    }
}
