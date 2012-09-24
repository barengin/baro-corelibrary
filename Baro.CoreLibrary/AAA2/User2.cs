using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Baro.CoreLibrary.AAA2
{
    public sealed class User2
    {
        private readonly UserData2Collection m_data = new UserData2Collection();

        public UserData2Collection Data { get { return m_data; } }
        public AAA2Credential Credential { get; private set; }

        public bool KeyQuery { get; internal set; }
        public bool RemoveUser { get; internal set; }
        public bool AddUser { get; internal set; }

        public User2(AAA2Credential credential)
        {
            this.Credential = credential;
        }

        internal static User2 CreateFromXml(XmlReader xml)
        {
            User2 u = new User2(new AAA2Credential(xml["usr"], xml["pwd"]));

            u.AddUser = (xml["adduser"] == "1") || (xml["adduser"] == "true");
            u.RemoveUser = (xml["removeuser"] == "1") || (xml["removeuser"] == "true");
            u.KeyQuery = (xml["keyquery"] == "1") || (xml["keyquery"] == "true");

            while (xml.Read())
            {
                if (xml.NodeType == XmlNodeType.EndElement && xml.Name == "user")
                {
                    break;
                }

                if (xml.NodeType == XmlNodeType.Element && xml.Name == "data")
                {
                    u.Data.CreateFromXml(xml);
                }
                
                if (xml.NodeType == XmlNodeType.Element && xml.Name == "keys")
                {
                    u.Data.CreateKeys(xml);
                }
            }

            return u;
        }
    }
}
