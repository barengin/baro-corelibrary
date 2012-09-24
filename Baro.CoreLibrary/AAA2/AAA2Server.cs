using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Xml;

namespace Baro.CoreLibrary.AAA2
{
    public sealed class AAA2Server: IAAA2
    {
        ConcurrentDictionary<string, User2> _users = new ConcurrentDictionary<string, User2>(Environment.ProcessorCount, 20000);
        Keys _keys = new Keys();

        public AAA2Server(string xmlfile)
        {
            LoadXml(xmlfile);
        }

        private void LoadXml(string xmlfile)
        {
            XmlReader xml = XmlReader.Create(xmlfile);
            xml.ReadToFollowing("users"); // root

            try
            {
                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.EndElement && xml.Name == "users")
                    {
                        break;
                    }

                    if (xml.NodeType == XmlNodeType.Element && xml.Name == "user")
                    {
                        User2 user = User2.CreateFromXml(xml);

                        if (!_users.TryAdd(user.Credential.Username, user))
                        {
                            throw new ArgumentException("Bu isimde bir kullanıcı zaten sistemde kaytlı. " + user.Credential.Username);
                        }

                        UserData2[] data = user.Data.Search("Keys.*");

                        foreach (var item in data)
                        {
                            string keyName = item.Key.Substring(5);

                            if (!_keys.Contains(keyName))
                            {
                                _keys.Add(keyName);
                            }

                            Key k;
                            if (_keys.TryGetKey(keyName, out k))
                            {
                                k.Add(item.Value, user);
                            }

                            user.Data.Remove(item.Key);
                        }
                    }
                }
            }
            finally
            {
                xml.Close();
            }
        }

        private AAA2ErrorCode GetUser(AAA2Credential credential, out User2 user)
        {
            if (_users.TryGetValue(credential.Username, out user))
            {
                if (user.Credential.Password == credential.Password)
                {
                    return AAA2ErrorCode.OK;
                }
                else
                {
                    return AAA2ErrorCode.UserNotFound;
                }
            }
            else
            {
                return AAA2ErrorCode.UserNotFound;
            }
        }

        public AAA2ErrorCode UserQuery(AAA2Credential userCredential, string queryString, out UserData2[] data)
        {
            data = null;

            User2 user;
            AAA2ErrorCode error = GetUser(userCredential, out user);

            if (error != AAA2ErrorCode.OK)
                return error;

            data = user.Data.Search(queryString);
            
            return AAA2ErrorCode.OK;
        }

        /// <summary>
        /// Anahtar kelime üzerinde sorgulama.
        /// </summary>
        /// <param name="loginCredential">İşlemi yapacak olan kullanıcı</param>
        /// <param name="keyOfUser">Key name (örnek: "PLAKA")</param>
        /// <param name="valueOfKey">Key değeri (örnek: "06be5409")</param>
        /// <param name="queryStringOfKey">İlgili key ile ilişkili kullanıcının verileri üzerinde yapılacak sorgu</param>
        /// <param name="data">UserData</param>
        /// <returns></returns>
        public AAA2ErrorCode UserQueryByKey(AAA2Credential loginCredential, string keyOfUser, string valueOfKey, string queryStringOfKey, out UserData2[] data)
        {
            data = null;

            User2 user;
            AAA2ErrorCode error = GetUser(loginCredential, out user);

            if (error != AAA2ErrorCode.OK)
                return error;

            if (!user.KeyQuery)
                return AAA2ErrorCode.PermissionRequired;

            Key k;

            if (_keys.TryGetKey(keyOfUser, out k))
            {
                if (k.Get(valueOfKey, out user))
                {
                    data = user.Data.Search(queryStringOfKey);
                    return AAA2ErrorCode.OK;
                }
                else
                {
                    return AAA2ErrorCode.KeyNotFound;
                }
            }
            else
            {
                return AAA2ErrorCode.KeyNotFound;
            }
        }

        public AAA2ErrorCode AddUserData(AAA2Credential userCredential, UserData2[] data)
        {
            User2 user;
            AAA2ErrorCode error = GetUser(userCredential, out user);

            if (error != AAA2ErrorCode.OK)
                return error;

            user.Data.Add(data);

            return AAA2ErrorCode.OK;
        }

        public AAA2ErrorCode RemoveUserData(AAA2Credential userCredential, string[] keys)
        {
            User2 user;
            AAA2ErrorCode error = GetUser(userCredential, out user);

            if (error != AAA2ErrorCode.OK)
                return error;

            user.Data.Remove(keys);

            return AAA2ErrorCode.OK;
        }

        /// <summary>
        /// Kullanıcı ekle
        /// </summary>
        /// <param name="loginCredential">İşlem yapacak kullanıcının bilgileri</param>
        /// <param name="userToAdd">Eklenecek kullanıcı</param>
        /// <param name="data">Eklenen kullanıcıya ait bilgiler</param>
        /// <returns></returns>
        public AAA2ErrorCode AddUser(AAA2Credential loginCredential, AAA2Credential userToAdd, UserData2[] data)
        {
            User2 user;
            AAA2ErrorCode error = GetUser(loginCredential, out user);

            if (error != AAA2ErrorCode.OK)
                return error;
            
            if (!user.AddUser)
                return AAA2ErrorCode.PermissionRequired;

            user = new User2(userToAdd);
            user.Data.Add(data);

            if (_users.TryAdd(user.Credential.Username, user))
            {
                return AAA2ErrorCode.OK;
            }
            else
            {
                return AAA2ErrorCode.AlreadyExists;
            }
        }

        /// <summary>
        /// Kullanıcı sil
        /// </summary>
        /// <param name="loginCredential">İşlemi yapacak ve hakkı olması gereken kullanıcı</param>
        /// <param name="userToDelete">Silinecek kullanıcı</param>
        /// <returns></returns>
        public AAA2ErrorCode DeleteUser(AAA2Credential loginCredential, AAA2Credential userToDelete)
        {
            User2 user;
            AAA2ErrorCode error = GetUser(loginCredential, out user);

            if (error != AAA2ErrorCode.OK)
                return error;

            if (!user.RemoveUser)
                return AAA2ErrorCode.PermissionRequired;

            if (_users.TryRemove(userToDelete.Username, out user))
            {
                return AAA2ErrorCode.OK;
            }
            else
            {
                return AAA2ErrorCode.UserNotFound;
            }
        }

        /// <summary>
        /// Kullanıcı silmeye hakkı olan birisi bütün sistemi boşaltabilir.
        /// </summary>
        /// <param name="loginCredential">İşlem yapacak kullanıcı bilgileri</param>
        /// <returns></returns>
        public AAA2ErrorCode RemoveAllUsers(AAA2Credential loginCredential)
        {
            User2 user;
            AAA2ErrorCode error = GetUser(loginCredential, out user);

            if (error != AAA2ErrorCode.OK)
                return error;

            if (!user.RemoveUser)
                return AAA2ErrorCode.PermissionRequired;

            _users.Clear();

            return AAA2ErrorCode.OK;
        }

        /// <summary>
        /// Hem kullanıcı adı hem de şifresi ile sistemi kontrol eder. Şifresi yanlışsa kullanıcı yok der.
        /// </summary>
        /// <param name="userCredential">Kontrol edilecek kullanıcı bilgileri</param>
        /// <returns></returns>
        public AAA2ErrorCode CheckUser(AAA2Credential userCredential)
        {
            User2 user;
            return GetUser(userCredential, out user);
        }
    }
}
