using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Baro.CoreLibrary.SockServer;
using System.IO;

namespace Baro.CoreLibrary.YolbilClient
{
    public sealed class ConnectionSettings
    {
        public IPEndPoint Address { get; set; }
        private string _sentFolder;
        private string _receivedFolder;

        private void CheckFolder(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        public ConnectionSettings()
        {
            this.SentFolder = null;
            this.ReceivedFolder = null;

            this.Login = new PredefinedCommands.Login() { username = "", password = "" };
        }

        /// <summary>
        /// Gönderilen mesajların kaydedileceği yer default değerin dışında mı?
        /// </summary>
        public bool OverrideSentFolder
        {
            get { return SentFolder == null ? false : true; }
        }

        /// <summary>
        /// Gönderilen mesajların kaydedileceği yer.
        /// </summary>
        public string SentFolder
        {
            get
            {
                return _sentFolder == null ? App2.AppPath + "SentFolder\\" : _sentFolder;
            }
            set
            {
                _sentFolder = value;
                CheckFolder(SentFolder);
            }
        }

        /// <summary>
        /// Alınan mesajların kaydedileceği yer default değerin dışında mı?
        /// </summary>
        public bool OverrideReceivedFolder
        {
            get { return ReceivedFolder == null ? false : true; }
        }

        /// <summary>
        /// Alınan mesajların kaydedileceği yer
        /// </summary>
        public string ReceivedFolder
        {
            get
            {
                return _receivedFolder == null ? App2.AppPath + "ReceivedFolder\\" : _receivedFolder;
            }
            set
            {
                _receivedFolder = value;
                CheckFolder(ReceivedFolder);
            }
        }

        /// <summary>
        /// Login işlemi için kullanıcı adı/şifre
        /// </summary>
        public PredefinedCommands.Login Login { get; set; }
    }
}
