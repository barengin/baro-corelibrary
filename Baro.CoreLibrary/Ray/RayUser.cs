﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public class RayUser : RayItem<RayUser>
    {
        private string _username;
        private string _password;

        private RayPermissionList _permissions = new RayPermissionList();
        private RayStringList _groups = new RayStringList();
        private RayAliasList _aliases = new RayAliasList();
        private RayDataStore _dataStore = new RayDataStore();

        private RayServer _server;

        private RayUser()
        {
            throw new NotSupportedException();
        }

        internal RayUser(string username, RayServer successor)
        {
            _username = username;
            
            _server = successor;
            SetSuccessor(successor);
        }

        public RayDataStore DataStore
        {
            get { return _dataStore; }
        }

        public string Username
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifySuccessor(IDU.Update, ObjectHierarchy.Password, null, value);
            }
        }

        public RayStringList Groups
        {
            get { return _groups; }
        }

        public RayAliasList Aliases
        {
            get { return _aliases; }
        }

        public RayPermissionList Permissions
        {
            get { return _permissions; }
        }

        public RayPermission GetPermission(string permissionKeyValue)
        {
            RayPermission p = new RayPermission(permissionKeyValue);

            // Read Group Permissions:
            foreach (var item in Groups)
            {
                string groupName = item;

                // Eğer permission bulunamazsa NULL dönecek.
                RayPermission gp = _server.GetGroup(groupName).Permissions.GetPermission(permissionKeyValue);

                // Eğer permission bulundu ise
                if (gp != null)
                {
                    // Dikkat en son DENIED update edilmeli.
                    p.Allowed = p.Allowed || gp.Allowed;
                    p.Denied = p.Denied || gp.Denied;

                    if (p.Denied)
                    {
                        return p;
                    }
                }
            }

            // Read user permissions:
            // Eğer permission bulunamazsa NULL dönecek.
            RayPermission up = this.Permissions.GetPermission(permissionKeyValue);

            // Eğer permission bulundu ise
            if (up != null)
            {
                // Dikkat en son DENIED update edilmeli.
                p.Allowed = p.Allowed || up.Allowed;
                p.Denied = p.Denied || up.Denied;
            }

            return p;
        }

        public override RayUser Clone()
        {
            throw new NotSupportedException();

            //RayUser u = _server.AddUser(this.Username);

            //u._password = this.Password;

            //u._permissions = this.Permissions.Clone();
            //u._groups = this.Groups.Clone();
            //u._aliases = this.Aliases.Clone();
            //u._dataStore = this.DataStore.Clone();

            //return u;
        }

        public override XmlNode CreateXmlNode(XmlDocument xmlDoc)
        {
            XmlNode n = xmlDoc.CreateElement("user");

            // Username attribute
            XmlAttribute a = xmlDoc.CreateAttribute("usr");
            a.Value = this.Username;
            n.Attributes.Append(a);

            // Password attribute
            a = xmlDoc.CreateAttribute("pwd");
            a.Value = this.Password;
            n.Attributes.Append(a);

            // Groups element
            XmlNode mg = xmlDoc.CreateElement("groups");

            foreach (var item in this.Groups)
            {
                XmlNode g = xmlDoc.CreateElement("group");

                // Group name attribute
                a = xmlDoc.CreateAttribute("name");
                a.Value = item;
                g.Attributes.Append(a);

                mg.AppendChild(g);
            }

            n.AppendChild(mg);

            // Permissions element
            n.AppendChild(this.Groups.CreateXmlNode(xmlDoc));

            // Data element
            n.AppendChild(this.DataStore.CreateXmlNode(xmlDoc));

            // Aliases
            n.AppendChild(this.Aliases.CreateXmlNode(xmlDoc));

            return n;
        }

        protected override void Handle(IDU op, ObjectHierarchy where, string info, object value)
        {
            NotifySuccessor(op, where, info, value);
        }
    }
}
