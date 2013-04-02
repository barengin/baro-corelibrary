using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray.MongoDb.Connector
{
    class ERayPermission
    {
        public string Name { get; set; }

        public bool Allowed { get; set; }
        public bool Denied { get; set; }
    }
}
