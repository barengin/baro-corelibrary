using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    /// <summary>
    /// This class contains parameters used by the Dial function to establish a remote access connection.
    /// </summary> 
    public class RasDialParams
    {
        private RASDIALPARAMS rasDialParams;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RasDialParams()
        {
            rasDialParams = new RASDIALPARAMS();
        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="entryName">the name of the entry</param>
        public RasDialParams(string entryName)
            : this()
        {
            EntryName = entryName;
        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="entryName">the name of the entry</param>
        /// <param name="username">the username to be used for the entry</param>
        /// <param name="password">the password to be used for the entry</param>
        public RasDialParams(string entryName, string username, string password)
            : this(entryName)
        {
            UserName = username;
            Password = password;
        }

        internal void Update()
        {
            if (rasDialParams != null)
            {
                Native.RasSetEntryDialParams(null, rasDialParams.Data, 0);
            }

        }

        internal void GetData()
        {
            //rasDialParams.szEntryName = entryName;
            int bPass = 0;
            int ret = Native.RasGetEntryDialParams(null, rasDialParams.Data, ref bPass);

        }

        internal byte[] Data
        {
            get
            {
                return rasDialParams.Data;
            }
        }

        /// <summary>
        /// Gets or sets string that contains the phone-book entry to use to establish the connection.
        /// </summary> 
        public string EntryName
        {
            get { return rasDialParams.szEntryName; }
            set { rasDialParams.szEntryName = value; }
        }

        /// <summary>
        /// Gets or sets string that contains the user’s user name. This string is used to authenticate the user’s access to the remote access server. 
        /// </summary> 
        public string UserName
        {
            get { return rasDialParams.szUserName; }
            set { rasDialParams.szUserName = value; }
        }

        /// <summary>
        /// Gets or sets string  that contains the user’s password. This string is used to authenticate the user’s access to the remote access server. 
        /// </summary> 
        public string Password
        {
            get { return rasDialParams.szPassword; }
            set { rasDialParams.szPassword = value; }
        }

        /// <summary>
        /// Gets or sets string that contains the domain on which authentication is to occur.
        /// </summary> 
        public string Domain
        {
            get { return rasDialParams.szDomain; }
            set { rasDialParams.szDomain = value; }
        }
    }
}
