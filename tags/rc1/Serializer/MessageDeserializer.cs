using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Serializer2
{
    internal sealed class MessageDeserializer
    {
        private object ReadValue(int pos, byte[] source, Type type, out int size)
        {
            if (type == typeof(Byte))
            {
                size = 1;
                return source[pos];
            }

            if (type == typeof(SByte))
            {
                size = 1;
                return (SByte)source[pos];
            }

            if (type == typeof(Int32))
            {
                size = 4;
                return BitConverter.ToInt32(source, pos);
            }

            if (type == typeof(UInt32))
            {
                size = 4;
                return BitConverter.ToUInt32(source, pos);
            }

            if (type == typeof(Int16))
            {
                size = 2;
                return BitConverter.ToInt16(source, pos);
            }

            if (type == typeof(UInt16))
            {
                size = 2;
                return BitConverter.ToUInt16(source, pos);
            }

            if (type == typeof(Int64))
            {
                size = 8;
                return BitConverter.ToInt64(source, pos);
            }

            if (type == typeof(UInt64))
            {
                size = 8;
                return BitConverter.ToUInt64(source, pos);
            }

            if (type == typeof(Single))
            {
                size = 4;
                return BitConverter.ToSingle(source, pos);
            }

            if (type == typeof(Double))
            {
                size = 8;
                return BitConverter.ToDouble(source, pos);
            }

            if (type == typeof(DateTime))
            {
                size = 8;
                return DateTime.FromOADate(BitConverter.ToDouble(source, pos));
            }

            if (type == typeof(String))
            {
                int dummy;
                size = (Int32)ReadValue(pos, source, typeof(Int32), out dummy);
                string s = new string(System.Text.Encoding.UTF8.GetChars(source, pos + 4, size));
                size += 4;
                return s;
            }

            if (type == typeof(Byte[]))
            {
                int dummy;
                size = (Int32)ReadValue(pos, source, typeof(Int32), out dummy);
                byte[] b = new byte[size];
                Buffer.BlockCopy(source, pos + 4, b, 0, size);
                size += 4;
                return b;
            }

            if (type == typeof(Boolean))
            {
                size = 1;
                return source[pos] == 1 ? true : false;
            }

            if (type == typeof(MessageStructure))
            {
                size = 1;
                return (MessageStructure)source[pos];
            }

            throw new NotSupportedException("Geçersiz TYPE");
        }

        private object GetStruct(byte[] obj, Type type, int pos)
        {
            FieldInfo[] fi = type.GetFields();

            object r = Activator.CreateInstance(type);

            for (int k = 0; k < fi.Length; k++)
            {
                FieldInfo f = fi[k];

                int size;
                object val = ReadValue(pos, obj, f.FieldType, out size);
                pos += size;

                f.SetValue(r, val);
            }

            return r;
        }

        internal MessageInternalHeader GetHeader(byte[] obj)
        {
            MessageInternalHeader h = (MessageInternalHeader)GetStruct(obj, typeof(MessageInternalHeader), 0);

            if (!h.isCrcOk())
                throw new MessageCrcException("Mesaj CRC hatası. Muhtemel veri kaybı...");

            return h;
        }

        private static void Decrypt(byte[] obj, int size, MessageKey key)
        {
            // Values
            int m = 0;
            byte[] pwd = key.Pwd;
            int keySize = pwd.Length;

            // Loop // TODO: Make some performance improvments.
            for (int k = Message.MESSAGE_INTERNAL_HEADER_SIZE; k < size; k++)
            {
                obj[k] = (byte)(obj[k] ^ pwd[m]);
                m++;
                m = m % keySize;
            }
        }

        public unsafe object Deserialize(MessageInternalHeader header, byte[] obj, MessageKey key)
        {
            Type t = Message.GetTypeFromID(header.CommandID);

            if ((header.Structure & MessageStructure.Encrypted) == MessageStructure.Encrypted)
            {
                if (key == null) throw new InvalidOperationException("Key null olamaz. Data encrypted!");
                Decrypt(obj, header.Size, key);
            }

            if ((header.Structure & MessageStructure.Compressed) == MessageStructure.Compressed)
            {
                // TODO: Compression Support
                throw new NotSupportedException("Compression is not supported");
            }
            else
            {
                return GetStruct(obj, t, Message.MESSAGE_INTERNAL_HEADER_SIZE);
            }
        }

        public unsafe object Deserialize(byte[] obj, out MessageInternalHeader header, MessageKey key)
        {
            header = GetHeader(obj);
            return Deserialize(header, obj, key);
        }
    }
}
