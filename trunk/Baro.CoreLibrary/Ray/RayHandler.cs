﻿using System;
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
        Group,
        GroupList,
        Username,
        Password
    }

    public abstract class RayHandler
    {
        private volatile bool _disableNotifies = false;
        protected RayHandler _successor;

        protected bool DisableNotifies
        {
            get { return _disableNotifies; }
            set { _disableNotifies = value; }
        }

        public void SetSuccessor(RayHandler s)
        {
            _successor = s;
        }

        protected void NotifySuccessor(IDU op, ObjectHierarchy where, string info, object value)
        {
            if (_successor != null)
            {
                _successor.Handle(op, where, info, value);
            }
        }

        protected abstract void Handle(IDU op, ObjectHierarchy where, string info, object value);
    }
}
