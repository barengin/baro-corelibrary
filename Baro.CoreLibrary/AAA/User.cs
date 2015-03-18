using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Baro.CoreLibrary.AAA
{
    public sealed class User
    {
        private readonly UserDataCollection m_data = new UserDataCollection();

        public UserDataCollection Data { get { return m_data; } }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public User(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        internal static User ReadFromXml(XmlReader xml)
        {
            User u = new User(xml["usr"], xml["pwd"]);

            while (xml.Read())
            {
                if (xml.NodeType == XmlNodeType.EndElement && xml.Name == "user")
                {
                    break;
                }

                if (xml.NodeType == XmlNodeType.Element && xml.Name == "data")
                {
                    u.Data.ReadFromXml(xml);
                }
            }

            return u;
        }

        internal void WriteToXml(XmlWriter w)
        {
            w.WriteStartElement("user");

            w.WriteAttributeString("usr", this.Username);
            w.WriteAttributeString("pwd", this.Password);

            Data.WriteToXml(w);

            w.WriteEndElement();
        }
    }
}
