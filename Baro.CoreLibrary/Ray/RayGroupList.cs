using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public class RayGroupList: IRayQuery<RayGroup>
    {
        private SortedList<string, RayGroup> _list = new SortedList<string, RayGroup>();

        public void Add(RayGroup group)
        {
            _list.Add(group.Name, group);
        }

        public void Remove(RayGroup group)
        {
            _list.Remove(group.Name);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public RayGroup this[string index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public IEnumerable<RayGroup> SelectAll()
        {
            return _list.Values;
        }

        public IEnumerable<RayGroup> StartsWith(string value)
        {
            return Utils.StartsWith<RayGroup>(_list, value);
        }

        public IEnumerable<RayGroup> Like(string value)
        {
            return from kvp in _list
                   where kvp.Key.Contains(value)
                   select kvp.Value;
        }
    }
}
