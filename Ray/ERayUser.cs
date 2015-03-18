using Baro.CoreLibrary.Ray;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray.MongoDb.Connector
{
    class ERayUser
    {
        // public ObjectId Id { get; set; }

        [BsonId]
        public string Username { get; set; }
        public string Password { get; set; }

        public List<string> AliasList { get; set; }
        public List<KeyValuePair<string, string>> DataStore { get; set; }
        public List<string> Groups { get; set; }
        public List<ERayPermission> Permissions { get; set; }

        public ERayUser()
        {
            AliasList = new List<string>();
            DataStore = new List<KeyValuePair<string, string>>();
            Groups = new List<string>();
            Permissions = new List<ERayPermission>();
        }
    }
}
