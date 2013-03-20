using Baro.CoreLibrary.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public sealed class RayPermissionList : RayItem<RayPermissionList>, IRayQuery<RayPermission>
    {
        private SortedList<string, RayPermission> _list = new SortedList<string, RayPermission>();

        public void Add(RayPermission p)
        {
            _list.Add(p.Key, p);
        }

        public void Remove(string key)
        {
            _list.Remove(key);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public RayPermission GetPermission(string index)
        {
            try
            {
                return _list[index];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public RayPermission this[string index]
        {
            get
            {
                try
                {
                    return _list[index];
                }
                catch (KeyNotFoundException)
                {
                    throw new RayKeyNotFoundException("Key not found: " + index);
                }
            }
            set
            {
                throw new NotSupportedException("This method is read-only. You can use Add(Permission) method instead");
            }
        }

        public IEnumerable<RayPermission> SelectAll()
        {
            return _list.Values;
        }

        public IEnumerable<RayPermission> StartsWith(string value)
        {
            return Utils.StartsWith<RayPermission>(_list, value);
        }

        public IEnumerable<RayPermission> Like(string value)
        {
            return from kvp in _list
                   where kvp.Key.Contains(value)
                   select kvp.Value;
        }

        public override RayPermissionList Clone()
        {
            RayPermissionList l = new RayPermissionList();
            
            foreach (var item in this._list)
            {
                l._list.Add(item.Key, item.Value);
            }

            return l;
        }
    }
}
