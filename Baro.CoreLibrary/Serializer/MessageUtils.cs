using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Baro.CoreLibrary.Core;

namespace Baro.CoreLibrary.Serializer2
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class MessageAttribute : Attribute
    {
        public UInt16 ID { get; set; }
        
        public bool SaveToQueue { get; set; }

        public static MessageAttribute GetMessageAttribute(Type t)
        {
            object[] attr = t.GetCustomAttributes(typeof(MessageAttribute), true);

            if (attr == null || attr.Length == 0)
            {
                throw new Exception("Bu struct MessageAttribute ile işaretlenmemiş!");
            }

#if PocketPC || WindowsCE
#else
            if (t.StructLayoutAttribute.Value != LayoutKind.Sequential)
            {
                throw new Exception("Bu struct [StructLayout(LayoutKind.Sequential)] şeklinde işaretlenmemiş!");
            }
#endif

            return (attr[0] as MessageAttribute);
        }

        public static MessageAttribute GetMessageAttribute(object o)
        {
            return GetMessageAttribute(o.GetType());
        }

        public static UInt16 GetMessageID(Type t)
        {
            return GetMessageAttribute(t).ID;
        }

        public static UInt16 GetMessageID(object o)
        {
            return GetMessageAttribute(o).ID;
        }

        public static UInt16 GetMessageID<T>()
        {
            Type t = typeof(T);
            return GetMessageID(t);
        }
    }

    public sealed class MessageInfo
    {
        public Int32 CorrelationID;
        public UInt16 ToInbox;
        public DateTime ExpireDate;

        public MessageInfo()
        {
            this.ExpireDate = DateTime.Now + new TimeSpan(48, 0, 0);
            this.CorrelationID = Environment.TickCount;
        }

        public MessageInfo(ushort toInbox)
        {
            this.ExpireDate = DateTime.Now + new TimeSpan(48, 0, 0);
            this.ToInbox = toInbox;
            this.CorrelationID = Environment.TickCount;
        }

        public MessageInfo(ushort toInbox, int hours)
        {
            this.ExpireDate = DateTime.Now + new TimeSpan(hours, 0, 0);
            this.ToInbox = toInbox;
            this.CorrelationID = Environment.TickCount;
        }

        public MessageInfo(ushort toInbox, TimeSpan timeSpan)
        {
            this.ExpireDate = DateTime.Now + timeSpan;
            this.ToInbox = toInbox;
            this.CorrelationID = Environment.TickCount;
        }

        public MessageInfo(ushort toInbox, int hours, int correlationID)
        {
            this.ExpireDate = DateTime.Now + new TimeSpan(hours, 0, 0);
            this.ToInbox = toInbox;
            this.CorrelationID = correlationID;
        }

        internal MessageHeader CreateMessageHeader()
        {
            return new MessageHeader()
            {
                CorrelationID = this.CorrelationID,
                ToInbox = this.ToInbox,
                ExpireDate = this.ExpireDate
            };
        }
    }

    /// <summary>
    /// !!! DİKKAT !!!
    /// 
    /// Aşağıdaki yapıda SIZE alanı hiçbir zaman değişmeyecek ve ilk eleman olarak kalacaktır.
    /// </summary>
    [Description("MessageHeader", "Header")]
#if PocketPC || WindowsCE
    [StructLayout(LayoutKind.Sequential)]
#else
    [StructLayout(LayoutKind.Sequential)]
#endif
    public struct MessageHeader
    {
        // Message SIZE
        [Description("Size", "Size: {0}")]
        public Int32 Size;

        // Corr-ID
        [Description("CorrelationID", "CorrelationID: {0}")]
        public Int32 CorrelationID;

        // CmdID
        [Description("CommandID", "CommandID: {0}")]
        public UInt16 CommandID;

        // Serialize as BYTE
        [Description("Structure", "Structure: {0}")]
        public MessageStructure Structure;

        // CRC
        [Description("CRC", "CRC: {0}")]
        public Int32 CRC;

        // ToInbox
        [Description("ToInbox", "ToInbox: {0}")]
        public UInt16 ToInbox;

        [Description("UniqueID1", "UniqueID1: {0}")]
        public UInt32 UniqueID1;

        [Description("UniqueID2", "UniqueID2: {0}")]
        public UInt32 UniqueID2;

        [Description("UniqueID3", "UniqueID3: {0}")]
        public UInt32 UniqueID3;

        [Description("UniqueID4", "UniqueID4: {0}")]
        public UInt32 UniqueID4;

        // Expire Date
        [Description("ExpireDate", "ExpireDate: {0}")]
        public DateTime ExpireDate;

        //internal MessageInfo CreateMessageInfo()
        //{
        //    return new MessageInfo()
        //    {
        //        CorrelationID = this.CorrelationID,
        //        ExpireDate = this.ExpireDate,
        //        ToInbox = this.ToInbox
        //    };
        //}

        public UniqueID GetMsgID()
        {
            return new UniqueID(UniqueID1, UniqueID2, UniqueID3, UniqueID4);
        }

        internal bool isCrcOk()
        {
            return CRC == (Size ^ Message.CRC_MAGIC_NUMBER);
        }

        public bool isServerSideCommand()
        {
            return this.CommandID < 1024;
        }

        public bool isExpired()
        {
            return (DateTime.Now > this.ExpireDate);
        }
    }

    [Flags]
    public enum MessageStructure : byte
    {
        Default = 0x00,
        Compressed = 0x01,
        Encrypted = 0x02
    }

    internal static class Converter
    {
        public static unsafe void GetBytes(byte[] buffer, double value)
        {
            GetBytes(buffer, *((long*)&value));
        }

        public static unsafe void GetBytes(byte[] buffer, float value)
        {
            GetBytes(buffer, *((int*)&value));
        }

        public static void GetBytes(byte[] buffer, ulong value)
        {
            GetBytes(buffer, (long)value);
        }

        public static unsafe void GetBytes(byte[] buffer, long value)
        {
            // byte[] buffer = new byte[8];
            fixed (byte* numRef = buffer)
            {
                *((long*)numRef) = value;
            }
            // return buffer;
        }

        public static void GetBytes(byte[] buffer, ushort value)
        {
            GetBytes(buffer, (short)value);
        }

        public static unsafe void GetBytes(byte[] buffer, short value)
        {
            // byte[] buffer = new byte[2];
            fixed (byte* numRef = buffer)
            {
                *((short*)numRef) = value;
            }
            // return buffer;
        }

        public static void GetBytes(byte[] buffer, uint value)
        {
            GetBytes(buffer, (int)value);
        }

        public static unsafe void GetBytes(byte[] buffer, int value)
        {
            // byte[] buffer = new byte[4];
            fixed (byte* numRef = buffer)
            {
                *((int*)numRef) = value;
            }
            //return buffer;
        }
    }
}
