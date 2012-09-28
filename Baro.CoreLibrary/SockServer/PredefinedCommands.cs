using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Baro.CoreLibrary.Serializer2;

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

        [Description("Keep-Alive")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 13)]
        public struct KeepAlive
        {
            [Description("KeepAlive", "KeepAlive")]
            public byte dummy;
        }

        [Description("MsgList")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 20)]
        public struct MsgList
        {
            [Description("Query", "Query")]
            public string Query;
        }

        [Description("MsgListResult")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 21)]
        public struct MsgListResult
        {
            [Description("QueryResult", "QueryResult")]
            public string Result;
        }

        [Description("MsgGet")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 22)]
        public struct MsgGet
        {
            [Description("id", "id")]
            public string MsgId;
        }

        [Description("MsgDelete")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 23)]
        public struct MsgDelete
        {
            [Description("id", "id")]
            public string MsgId;
        }

        [Description("Sunucu ile ilgili bazı bilgilerin sorgulanması", "ServerInfoRequest")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 30)]
        public struct ServerInfoRequest
        {
            /// <summary>
            /// 0 - NOP
            /// 1 - Get date/time
            /// </summary>
            [Description("InfoRequest", "InfoRequest {0}")]
            public int InfoRequest;
        }

        [Description("Sunucu saati", "ServerDateTimeResponse")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 31)]
        public struct ServerDateTimeResponse
        {
            [Description("DateTime", "DateTime: {0}")]
            public double DateTime;
        }

        public static void Init()
        {
            Message.UnRegisterMessageType(typeof(Login));
            Message.UnRegisterMessageType(typeof(KeepAlive));
            Message.UnRegisterMessageType(typeof(MsgList));
            Message.UnRegisterMessageType(typeof(MsgListResult));
            Message.UnRegisterMessageType(typeof(MsgGet));
            Message.UnRegisterMessageType(typeof(MsgDelete));
            Message.UnRegisterMessageType(typeof(ServerInfoRequest));
            Message.UnRegisterMessageType(typeof(ServerDateTimeResponse));

            Message.RegisterMessageType(typeof(Login));
            Message.RegisterMessageType(typeof(KeepAlive));
            Message.RegisterMessageType(typeof(MsgList));
            Message.RegisterMessageType(typeof(MsgListResult));
            Message.RegisterMessageType(typeof(MsgGet));
            Message.RegisterMessageType(typeof(MsgDelete));
            Message.RegisterMessageType(typeof(ServerInfoRequest));
            Message.RegisterMessageType(typeof(ServerDateTimeResponse));
        }
    }
}
