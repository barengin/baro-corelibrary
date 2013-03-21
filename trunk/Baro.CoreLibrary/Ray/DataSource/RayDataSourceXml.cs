using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray.DataSource
{
    public class RayDataSourceXml: IRayDataSource
    {
        private XmlDocument _xml = new XmlDocument();

        public RayDataSourceXml(string filename)
        {
            _xml.Load(filename);
        }

        public void InsertUser(RayUser user)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(RayUser user)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public RayUser ReadUser(string username)
        {
            throw new NotImplementedException();
        }

        public void InsertGroup(RayGroup group)
        {
            throw new NotImplementedException();
        }

        public void UpdateGroup(RayGroup group)
        {
            throw new NotImplementedException();
        }

        public void DeleteGroup(string groupname)
        {
            throw new NotImplementedException();
        }

        public RayGroup ReadGroup(string groupname)
        {
            throw new NotImplementedException();
        }

        public string[] GetUserList()
        {
            throw new NotImplementedException();
        }

        public string[] GetGroupList()
        {
            throw new NotImplementedException();
        }
    }
}
