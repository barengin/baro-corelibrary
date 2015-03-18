using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using Baro.CoreLibrary.Collections;

namespace Baro.CoreLibrary.AAA
{
    public sealed class AAAServer : IAAAServer
    {
        SynchDictionary<string, User> m_usernameToUser;
        SynchBiDictionary<Token, User> m_token2user;
        ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
        string userXmlFile;

        #region EqComparers
        private sealed class TokenEqComparer : IEqualityComparer<Token>
        {
            public bool Equals(Token x, Token y)
            {
                return x.TokenData == y.TokenData;
            }

            public int GetHashCode(Token obj)
            {
                return obj.TokenData.GetHashCode();
            }
        }

        private sealed class UserEqComparer : IEqualityComparer<User>
        {
            public bool Equals(User x, User y)
            {
                return x.Username == y.Username;
            }

            public int GetHashCode(User obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion

        public AAAServer(string xmlFile = "directory.xml")
        {
            userXmlFile = xmlFile;

            m_usernameToUser = m_usernameToUser = new SynchDictionary<string, User>(20000);
            m_token2user = new SynchBiDictionary<Token, User>(20000, new TokenEqComparer(), new UserEqComparer());
            
            LoadFromXml(xmlFile);
        }

        private void LoadFromXml(string filename)
        {
            m_usernameToUser.Clear();

            XmlReader xml = XmlReader.Create(filename);
            xml.ReadToFollowing("users"); // root

            while (xml.Read())
            {
                if (xml.NodeType == XmlNodeType.EndElement && xml.Name == "users")
                {
                    break;
                }

                if (xml.NodeType == XmlNodeType.Element && xml.Name == "user")
                {
                    User user = User.ReadFromXml(xml);
                    m_usernameToUser.Add(user.Username, user);
                }
            }

            xml.Close();
        }

        public void Save()
        {
            Save(userXmlFile);
        }

        public void Save(string filename)
        {
            m_lock.EnterReadLock();

            try
            {
                KeyValuePair<string, User>[] kvp = m_usernameToUser.ToArray();

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = System.Text.Encoding.UTF8;
                settings.Indent = true;

                XmlWriter w = XmlWriter.Create(filename, settings);
                w.WriteStartElement("users");

                foreach (var item in kvp)
                {
                    item.Value.WriteToXml(w);
                }

                w.WriteEndElement();
                w.Close();
            }
            finally
            {
                m_lock.ExitReadLock();
            }
        }

        private User GetUser(string username, string password)
        {
            User u;

            if (!m_usernameToUser.TryGetValue(username, out u))
            {
                return null;
            }

            if (u.Password != password)
            {
                return null;
            }

            return u;
        }

        public UserData[] UserQuery(string username, string password, string queryString)
        {
            User u = GetUser(username, password);

            if (u == null) return null;

            return u.Data.Search(queryString);
        }

        public AErrorCode AddUserData(string username, string password, UserData[] kvp)
        {
            User u = GetUser(username, password);

            if (u == null) return AErrorCode.UserNotFound;

            if (kvp != null)
            {
                u.Data.Add(kvp);
            }

            return AErrorCode.OK;
        }

        public AErrorCode RemoveUserData(string username, string password, string[] keys)
        {
            User u = GetUser(username, password);

            if (u == null) return AErrorCode.UserNotFound;

            if (keys != null)
            {
                u.Data.Remove(keys);
            }

            return AErrorCode.OK;
        }

        public AErrorCode AddUser(string username, string password, UserData[] kvp)
        {
            m_lock.EnterWriteLock();

            try
            {
                if (m_usernameToUser.Contains(username))
                {
                    return AErrorCode.AlreadyExists;
                }

                User u = new User(username, password);

                if (kvp != null)
                {
                    u.Data.Add(kvp);
                }

                m_usernameToUser.Add(u.Username, u);

                return AErrorCode.OK;
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
        }

        public AErrorCode DeleteUser(string username, string password)
        {
            m_lock.EnterWriteLock();

            try
            {
                User u = GetUser(username, password);

                if (u != null)
                {
                    m_token2user.RemoveValue(u);
                    m_usernameToUser.Remove(username);
                    return AErrorCode.OK;
                }
                else
                {
                    return AErrorCode.UserNotFound;
                }
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
        }

        public UserData[] UserQuery(Token userToken, string queryString)
        {
            User u;

            if (m_token2user.TryGetValue(userToken, out u))
            {
                return u.Data.Search(queryString);
            }
            else
            {
                return null;
            }
        }

        public AErrorCode AddUserData(Token userToken, UserData[] kvp)
        {
            User u;

            if (m_token2user.TryGetValue(userToken, out u))
            {
                u.Data.Add(kvp);
                return AErrorCode.OK;
            }
            else
            {
                return AErrorCode.UserNotFound;
            }
        }

        public AErrorCode RemoveUserData(Token userToken, string[] keys)
        {
            User u;

            if (m_token2user.TryGetValue(userToken, out u))
            {
                u.Data.Remove(keys);
                return AErrorCode.OK;
            }
            else
            {
                return AErrorCode.UserNotFound;
            }
        }

        public AErrorCode CreateToken(string username, string password, out Token userToken)
        {
            User u = GetUser(username, password);

            if (u != null)
            {
                m_lock.EnterWriteLock();

                try
                {
                    // Kullanıcı token olarak zaten kayıtlı ise
                    if (m_token2user.ContainsValue(u))
                    {
                        m_token2user.RemoveValue(u);
                    }

                    byte[] buf = new byte[8];

                    ThreadSafeRandom.NextBytes(buf);

                    userToken = new Token(DateTime.Now + TimeSpan.FromHours(24), BitConverter.ToInt64(buf, 0));

                    m_token2user.Add(userToken, u);

                    return AErrorCode.OK;
                }
                finally
                {
                    m_lock.ExitWriteLock();
                }
            }
            else
            {
                userToken = null;
                return AErrorCode.UserNotFound;
            }
        }

        public void DeleteAllData()
        {
            // TODO: DeleteAll
            throw new NotImplementedException();
        }
    }
}
