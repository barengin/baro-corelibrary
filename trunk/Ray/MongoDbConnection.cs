using Baro.CoreLibrary.Ray;
using Baro.CoreLibrary.Ray.DataSource;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray.MongoDb.Connector
{
    public sealed class MongoDbConnection : IRayDataSource
    {
        MongoClient client;
        MongoServer server;
        MongoDatabase database;

        private MongoCollection<ERayUser> Users
        {
            get { return database.GetCollection<ERayUser>("Users"); }
        }

        private MongoCollection<ERayGroup> Groups
        {
            get { return database.GetCollection<ERayGroup>("Groups"); }
        }

        public MongoDbConnection(string connectionString, string dbName = "Ray")
        {
            //BsonClassMap.RegisterClassMap<KeyValuePair<string, string>>(cm =>
            //{
            //    cm.AutoMap();
            //    cm.MapCreator(c => new KeyValuePair<string, string>(c.Key, c.Value));
            //    cm.GetMemberMap(c => c.Key).SetElementName("key");
            //    cm.GetMemberMap(c => c.Value).SetElementName("value");
            //});

            client = new MongoClient(connectionString);
            server = client.GetServer();
            database = server.GetDatabase(dbName);
        }

        public void InsertUser(RayUser user)
        {
            ERayUser u = new ERayUser()
            {
                Username = user.Username,
                Password = user.Password
            };

            foreach (var item in user.Permissions)
            {
                u.Permissions.Add(new ERayPermission() { Name = item.Key, Allowed = item.Allowed, Denied = item.Denied });
            }

            foreach (var item in user.Groups)
            {
                u.Groups.Add(item);
            }

            foreach (var item in user.DataStore)
            {
                u.DataStore.Add(item);
            }

            foreach (var item in user.Aliases)
            {
                u.AliasList.Add(item);
            }

            this.Users.Insert(u);
        }

        public void UpdateUser(RayUser user)
        {
            ERayUser u = new ERayUser()
            {
                Username = user.Username,
                Password = user.Password
            };

            foreach (var item in user.Permissions)
            {
                u.Permissions.Add(new ERayPermission() { Name = item.Key, Allowed = item.Allowed, Denied = item.Denied });
            }

            foreach (var item in user.Groups)
            {
                u.Groups.Add(item);
            }

            foreach (var item in user.DataStore)
            {
                u.DataStore.Add(item);
            }

            foreach (var item in user.Aliases)
            {
                u.AliasList.Add(item);
            }

            this.Users.Save(u);
        }

        public void DeleteUser(string username)
        {
            var query = Query<ERayUser>.EQ(e => e.Username, username);
            this.Users.Remove(query);
        }

        public RayUser ReadUser(string username, RayUserList listToAdd)
        {
            MongoCollection<ERayUser> c = this.Users;

            ERayUser r = c.FindOne(Query<ERayUser>.EQ(e => e.Username, username));

            // Dosya bulunamadı
            if (r == null)
            {
                return null;
            }

            RayUser u = listToAdd.AddNew(username);

            u.Password = r.Password;

            foreach (var item in r.Permissions)
            {
                RayPermission p = u.Permissions.AddNew(item.Name);
                p.Allowed = item.Allowed;
                p.Denied = item.Denied;
            }

            foreach (var item in r.Groups)
            {
                u.Groups.Add(item);
            }

            foreach (var item in r.DataStore)
            {
                u.DataStore[item.Key] = item.Value;
            }

            foreach (var item in r.AliasList)
            {
                u.Aliases.Add(item);
            }

            return u;
        }

        public void InsertGroup(RayGroup group)
        {
            ERayGroup g = new ERayGroup() { Name = group.Name };

            foreach (var item in group.Permissions)
            {
                g.Permissions.Add(new ERayPermission() { Name = item.Key, Allowed = item.Allowed, Denied = item.Denied });
            }

            this.Groups.Insert(g);
        }

        public void UpdateGroup(RayGroup group)
        {
            ERayGroup g = new ERayGroup() { Name = group.Name };

            foreach (var item in group.Permissions)
            {
                g.Permissions.Add(new ERayPermission() { Name = item.Key, Allowed = item.Allowed, Denied = item.Denied });
            }

            this.Groups.Save(g);
        }

        public void DeleteGroup(string groupname)
        {
            var query = Query<ERayGroup>.EQ(e => e.Name, groupname);
            this.Groups.Remove(query);
        }

        public RayGroup ReadGroup(string groupname, RayGroupList listToAdd)
        {
            MongoCollection<ERayGroup> c = this.Groups;

            ERayGroup r = c.FindOne(Query<ERayGroup>.EQ(e => e.Name, groupname));

            // Dosya bulunamadı
            if (r == null)
            {
                return null;
            }

            RayGroup g = listToAdd.AddNew(groupname);

            foreach (var item in r.Permissions)
            {
                RayPermission p = g.Permissions.AddNew(item.Name);
                p.Allowed = item.Allowed;
                p.Denied = item.Denied;
            }

            return g;
        }

        public string[] GetUserList()
        {
            MongoCollection c = this.Users;

            var q = from e in c.AsQueryable<ERayUser>()
                    select e.Username;

            return q.ToArray<string>();

            //MongoCollection c = database.GetCollection<ERayUser>("Users");

            //ERayUser u = new ERayUser();
            //u.Username = "u2";
            //u.Password = "p1";

            //u.AliasList.Add("alias1");
            //u.DataStore.Add(new KeyValuePair<string, string>("k1", "v1"));
            //u.Groups.Add("G1");
            //u.Permissions.Add(new ERayPermission() { Name = "P1N", Allowed = true });

            //c.Insert(u);

            //return null;
        }

        public string[] GetGroupList()
        {
            MongoCollection c = this.Groups;

            var q = from e in c.AsQueryable<ERayGroup>()
                    select e.Name;

            return q.ToArray<string>();
        }

        public void ClearUsers()
        {
            this.Users.Drop();
        }

        public void ClearGroups()
        {
            this.Groups.Drop();
        }
    }
}
