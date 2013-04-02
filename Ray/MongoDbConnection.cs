using Baro.CoreLibrary.Ray;
using Baro.CoreLibrary.Ray.DataSource;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray.MongoDb.Connector
{
    public sealed class MongoDbConnection: IRayDataSource
    {
        MongoClient client;
        MongoServer server;
        MongoDatabase database;

        public MongoDbConnection(string connectionString, string dbName = "Ray")
        {
            client = new MongoClient(connectionString);
            server = client.GetServer();
            database = server.GetDatabase(dbName);
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

        public void ClearUsers()
        {
            throw new NotImplementedException();
        }

        public void ClearGroups()
        {
            throw new NotImplementedException();
        }
    }
}
