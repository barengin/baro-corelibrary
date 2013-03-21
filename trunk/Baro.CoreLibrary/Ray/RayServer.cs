using Baro.CoreLibrary.Ray.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public sealed class RayServer: RayItem<RayServer>
    {
        private IRayDataSource _dataSource = null;

        public IRayDataSource DataSource { get { return _dataSource; } }

        /// <summary>
        /// In-memory configuration
        /// </summary>
        public RayServer()
        {
        }

        /// <summary>
        /// In-memory mapped datasource
        /// </summary>
        /// <param name="dataSource">Data source of RayServer</param>
        public RayServer(IRayDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public RayUser GetUser(string username)
        {
            return null;
        }

        public override RayServer Clone()
        {
            throw new NotImplementedException();
        }
    }
}
