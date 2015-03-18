using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Baro.CoreLibrary.Serializer2;
using Baro.CoreLibrary.Core;

namespace Baro.CoreLibrary.SockServer
{
    public sealed class AckComparer : IEqualityComparer<PredefinedCommands.Ack2>
    {
        public bool Equals(PredefinedCommands.Ack2 x, PredefinedCommands.Ack2 y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(PredefinedCommands.Ack2 obj)
        {
            return obj.GetHashCode();
        }
    }

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

        [Description("Keep-Alive", "Keep-Alive")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 13)]
        public struct KeepAlive
        {
            [Description("Seq", "Seq: {0}")]
            public byte seq;
        }

        [Description("Ack", "Ack")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 14)]
        public struct Ack
        {
            [Description("MsgID", "MsgID")]
            public string MsgId;
        }

        [Description("Ack2", "Ack2")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 15)]
        public struct Ack2 : IEquatable<Ack2>
        {
            [Description("uid1", "uid1: {0:x8}")]
            public uint uid1;

            [Description("uid2", "uid2: {0:x8}")]
            public uint uid2;

            [Description("uid3", "uid3: {0:x8}")]
            public uint uid3;

            [Description("uid4", "uid4: {0:x8}")]
            public uint uid4;

            public Ack2 CreateEmpty()
            {
                return new Ack2() { uid1 = 0, uid2 = 0, uid3 = 0, uid4 = 0 };
            }

            public bool isEmpty
            {
                get { return this.uid1 == 0 && this.uid2 == 0 && this.uid3 == 0 && this.uid4 == 0; }
            }

            public UniqueID CreateUniqueID()
            {
                return new UniqueID(uid1, uid2, uid3, uid4);
            }

            public static Ack2 CreateFrom(UniqueID u)
            {
                return new Ack2() { uid1 = u.Data1, uid2 = u.Data2, uid3 = u.Data3, uid4 = u.Data4 };
            }

            public static Ack2 CreateAck2(MessageHeader h)
            {
                return CreateFrom(h.GetMsgID());
            }

            public override int GetHashCode()
            {
                return (int)(this.uid1 ^ (this.uid2 << 1) ^ this.uid3 ^ (this.uid4 >> 1));
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is PredefinedCommands.Ack2)
                {
                    return Equals((Ack2)obj);
                }
                else
                {
                    return false;
                }
            }

            public bool Equals(Ack2 other)
            {
                return other.uid1 == this.uid1 &&
                       other.uid2 == this.uid2 &&
                       other.uid3 == this.uid3 &&
                       other.uid4 == this.uid4;
            }
        }

        [Description("MsgList", "MsgList")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 20)]
        public struct MsgList
        {
            [Description("Query", "Query: {0}")]
            public string Query;
        }

        [Description("MsgListResult", "MsgListResult")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 21)]
        public struct MsgListResult
        {
            [Description("QueryResult", "QueryResult: {0}")]
            public string Result;
        }

        [Description("MsgGet", "MsgGet")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 22)]
        public struct MsgGet
        {
            [Description("id", "id: {0}")]
            public string MsgId;
        }

        [Description("MsgDelete", "MsgDelete")]
        [StructLayout(LayoutKind.Sequential)]
        [MessageAttribute(ID = 23, SaveToQueue = true)] // Save: Client tarafında silme işlemi sunucuya gönderilmeden önce kaydedilmeli. Bağlantı kopabilir.
        public struct MsgDelete
        {
            [Description("id", "id: {0}")]
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
            [Description("DateTime", "DateTime: {0}", true)]
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
            Message.UnRegisterMessageType(typeof(Ack));
            Message.UnRegisterMessageType(typeof(Ack2));

            Message.RegisterMessageType(typeof(Login));
            Message.RegisterMessageType(typeof(KeepAlive));
            Message.RegisterMessageType(typeof(MsgList));
            Message.RegisterMessageType(typeof(MsgListResult));
            Message.RegisterMessageType(typeof(MsgGet));
            Message.RegisterMessageType(typeof(MsgDelete));
            Message.RegisterMessageType(typeof(ServerInfoRequest));
            Message.RegisterMessageType(typeof(ServerDateTimeResponse));
            Message.RegisterMessageType(typeof(Ack));
            Message.RegisterMessageType(typeof(Ack2));
        }
    }
}
