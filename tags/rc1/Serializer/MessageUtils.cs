﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Serializer2
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class MessageAttribute : Attribute
    {
        public UInt16 ID { get; set; }

        public static UInt16 GetMessageID(Type t)
        {
            object[] attr = t.GetCustomAttributes(typeof(MessageAttribute), true);

            if (attr == null || attr.Length == 0)
            {
                throw new Exception("Bu struct CommandAttribute ile işaretlenmemiş!");
            }

#if PocketPC || WindowsCE
#else
            if (t.StructLayoutAttribute.Value != LayoutKind.Sequential)
            {
                throw new Exception("Bu struct [StructLayout(LayoutKind.Sequential)] şeklinde işaretlenmemiş!");
            }
#endif

            return (attr[0] as MessageAttribute).ID;
        }

        public static UInt16 GetMessageID(object o)
        {
            return GetMessageID(o.GetType());
        }
    }

    public struct MessageInfo
    {
        public Int32 CorrelationID;
        public UInt16 ToInbox;
        public DateTime ExpireDate;

        public MessageInfo(DateTime expireDate)
        {
            this.ExpireDate = expireDate;
            this.ToInbox = 0;
            this.CorrelationID = Environment.TickCount;
        }

        internal MessageInternalHeader CreateInternalHeader()
        {
            return new MessageInternalHeader()
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
    [Description("MessageInternalHeader", "Header")]
#if PocketPC || WindowsCE
    [StructLayout(LayoutKind.Sequential)]
#else
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
    public struct MessageInternalHeader
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
        
        // Expire Date
        [Description("ExpireDate", "ExpireDate: {0}")]
        public DateTime ExpireDate;

        internal MessageInfo CreateMessageInfo()
        {
            return new MessageInfo()
            {
                CorrelationID = this.CorrelationID,
                ExpireDate = this.ExpireDate,
                ToInbox = this.ToInbox
            };
        }

        internal bool isCrcOk()
        {
            return CRC == (Size ^ Message.CRC_MAGIC_NUMBER);
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
