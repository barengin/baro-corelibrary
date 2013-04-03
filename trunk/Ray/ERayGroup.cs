using Baro.CoreLibrary.Ray;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray.MongoDb.Connector
{
    class ERayGroup
    {
        [BsonId]
        public string Name { get; set; }

        public List<ERayPermission> Permissions { get; set; }

        public ERayGroup()
        {
            Permissions = new List<ERayPermission>();
        }
    }
}
