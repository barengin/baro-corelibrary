using Baro.CoreLibrary.Ray.DataSource;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private bool _doNotProcessNotifications = false;

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

            _doNotProcessNotifications = true;
            Load();
            _doNotProcessNotifications = false;
        }

        private void Load()
        {
            string[] groups = _dataSource.GetGroupList();
            string[] users = _dataSource.GetUserList();

            foreach (var item in groups)
            {
                RayGroup g = _dataSource.ReadGroup(item, this.Groups);
            }

            foreach (var item in users)
            {
                RayUser u = _dataSource.ReadUser(item, this.Users);
            }
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
            if (_doNotProcessNotifications)
                return;

            Debug.WriteLine("{0} - {1} - {2} - {3}", op.ToString(), where.ToString(), info, value);

            switch (where)
            {
                case ObjectHierarchy.Group:
                    ProcessGroup(op, where, info, value);
                    break;

                case ObjectHierarchy.GroupList:
                    ProcessGroupList(op, where, info, value);
                    break;

                case ObjectHierarchy.User:
                    ProcessUser(op, where, info, value);
                    break;

                case ObjectHierarchy.UserList:
                    ProcessUserList(op, where, info, value);
                    break;

                default:
                    throw new InvalidOperationException("Buraya hatalı OH geliyor.");
            }
        }

        private void ProcessUserList(IDU op, ObjectHierarchy where, string info, object value)
        {
            switch (op)
            {
                case IDU.Insert:
                    _dataSource.InsertUser((RayUser)value);
                    break;

                case IDU.Delete:
                    if (value == null)
                    {
                        _dataSource.ClearUsers();
                    }
                    else
                    {
                        _dataSource.DeleteUser((string)value);
                    }
                    break;
                
                default:
                    break;
            }
        }

        private void ProcessUser(IDU op, ObjectHierarchy where, string info, object value)
        {
            switch (op)
            {
                case IDU.Update:
                    _dataSource.UpdateUser((RayUser)value);
                    break;

                default:
                    break;
            }
        }

        private void ProcessGroupList(IDU op, ObjectHierarchy where, string info, object value)
        {
            switch (op)
            {
                case IDU.Insert:
                    _dataSource.InsertGroup((RayGroup)value);
                    break;

                case IDU.Delete:
                    if (value == null)
                    {
                        _dataSource.ClearGroups();
                    }
                    else
                    {
                        _dataSource.DeleteGroup((string)value);
                    }
                    break;

                default:
                    break;
            }
        }

        private void ProcessGroup(IDU op, ObjectHierarchy where, string info, object value)
        {
            switch (op)
            {
                case IDU.Update:
                    _dataSource.UpdateGroup((RayGroup)value);
                    break;
                                
                default:
                    break;
            }
        }
    }
}
