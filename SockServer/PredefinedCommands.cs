using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Baro.CoreLibrary.Serializer2;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.SockServer
{
    /// <summary>
    /// 0-1023 arası ID'ye sahip komutlar özel komutlardır.
    /// </summary>
    public static class PredefinedCommands
    {
        [Description("Sisteme ilk girişte kullanılan login methodu istemcilerden sunucuya gönderilir", "Login")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 10)]
        public struct Login
        {
            [Description("Kullanıcı adı", "Usr: {0}")]
            public string username;

            [Description("Şifre", "Pwd: {0}")]
            public string password;
        }

        [Description("Diğer kullanıcılardan hangilerinin online/offline olduğunu anlamak için sunucudan istemcilere gönderilir", "OnlineNotification")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 11)]
        public struct OnlineNotification
        {
            [Description("Inbox no", "Inbox: {0}")]
            public ushort userInbox;

            [Description("Online olup olmadığı", "Online: {0}")]
            public bool online;
        }

        [Description("Bir kullanıcının online olup olmadığı bilgisinin otomatik gelmesi için sisteme kayıt yapar.", "RegisterForOnlineNotification")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 12)]
        public struct RegisterForOnlineNotification
        {
            [Description("Inbox no", "Inbox: {0}")]
            public ushort registeredInbox;

            [Description("Register", "Register/UnRegister: {0}")]
            public bool register;
        }

        [Description("Keep-Alive")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 13)]
        public struct KeepAlive
        {
            [Description("KeepAlive", "KeepAlive")]
            public byte dummy;
        }

        public static void Init()
        {
            Message.RegisterMessageType(typeof(Login));
            Message.RegisterMessageType(typeof(OnlineNotification));
            Message.RegisterMessageType(typeof(RegisterForOnlineNotification));
            Message.RegisterMessageType(typeof(KeepAlive));
        }
    }
}
