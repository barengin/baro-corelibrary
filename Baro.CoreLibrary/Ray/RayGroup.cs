using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public class RayGroup: RayItem<RayGroup>
    {
        private string _name;
        private RayPermissionList _permissions = new RayPermissionList();

        public string Name { get { return _name; } }
        public RayPermissionList Permissions { get { return _permissions; } }

        public override RayGroup Clone()
        {
            throw new NotImplementedException();
        }
    }
}
