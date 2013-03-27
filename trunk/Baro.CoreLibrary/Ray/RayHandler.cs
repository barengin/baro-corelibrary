using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public enum IDU
    {
        Update,
        Insert,
        Delete
    }

    public enum ObjectHierarchy
    {
        AliasList,
        DataStore,
        Permission,
        PermissionList,
        Username,
        Password
    }

    public abstract class RayHandler
    {
        protected RayHandler _successor;

        protected void SetSuccessor(RayHandler s)
        {
            _successor = s;
        }

        protected void NotifySuccessor(IDU op, ObjectHierarchy where, object key, object value)
        {
            if (_successor != null)
            {
                _successor.Handle(op, where, key, value);
            }
        }

        protected abstract void Handle(IDU op, ObjectHierarchy where, object key, object value);
    }
}
