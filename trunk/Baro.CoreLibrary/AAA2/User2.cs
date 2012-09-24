using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.AAA2
{
    public sealed class User2
    {
        private readonly UserData2Collection m_data = new UserData2Collection();

        public UserData2Collection Data { get { return m_data; } }
        public AAA2Credential Credential { get; private set; }

        public bool KeyQuery { get; set; }
        public bool RemoveUser { get; set; }
        public bool AddUser { get; set; }

        public User2(AAA2Credential credential)
        {
            this.Credential = credential;
        }
    }
}
