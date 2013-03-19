using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public class RayUser: RayItem<RayUser>
    {
        private string _username;
        private string _password;
        private RayPermissionList _permissions = new RayPermissionList();

        public RayPermissionList Permissions
        {
            get { return _permissions; }
        }

        public string Password
        {
            get { return _password; }
        }

        public string Username
        {
            get { return _username; }
        }

        public override RayUser Clone()
        {
            throw new NotImplementedException();
        }
    }
}
