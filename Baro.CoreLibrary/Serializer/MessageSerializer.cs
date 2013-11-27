using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Baro.CoreLibrary.Core;

namespace Baro.CoreLibrary.Serializer2
{
    internal sealed class SerializerDATA
    {
        public byte[] buf8 = new byte[8];
        public byte[] data = new byte[128];
        public int size = Message.MESSAGE_INTERNAL_HEADER_SIZE;

        public void grow(int requiredFreeSize)
        {
            int free = data.Length - size;

            if (requiredFreeSize > free)
            {
                Array.Resize<byte>(ref data, data.Length * 2);
                grow(requiredFreeSize);
            }
        }
    }

    internal sealed class MessageSerializer
    {
        private void addToByteArray(byte[] source, int sourceSize, SerializerDATA d)
        {
            d.grow(sourceSize);
            Buffer.BlockCopy(source, 0, d.data, d.size, sourceSize);
            d.size += sourceSize;
        }

        private void AddField(object o, SerializerDATA d)
        {
            if (o == null)
                throw new NullReferenceException("Gönderilen yapının içerisinde null değer olamaz.");

            Type objType = o.GetType();

            if (objType == typeof(bool))
            {
                d.buf8[0] = ((Boolean)o) ? (Byte)1 : (Byte)0;
                addToByteArray(d.buf8, 1, d);
                return;
            }

            if (objType == typeof(byte))
            {
                d.buf8[0] = (Byte)o;
                addToByteArray(d.buf8, 1, d);
                return;
            }

            if (objType == typeof(SByte))
            {
                d.buf8[0] = (Byte)((SByte)o);
                addToByteArray(d.buf8, 1, d);
                return;
            }

            if (objType == typeof(Int32))
            {
                Converter.GetBytes(d.buf8, (Int32)o);
                addToByteArray(d.buf8, sizeof(Int32), d);
                return;
            }

            if (objType == typeof(UInt32))
            {
                Converter.GetBytes(d.buf8, (UInt32)o);
                addToByteArray(d.buf8, sizeof(UInt32), d);
                return;
            }

            if (objType == typeof(Int16))
            {
                Converter.GetBytes(d.buf8, (Int16)o);
                addToByteArray(d.buf8, sizeof(Int16), d);
                return;
            }

            if (objType == typeof(UInt16))
            {
                Converter.GetBytes(d.buf8, (UInt16)o);
                addToByteArray(d.buf8, sizeof(UInt16), d);
                return;
            }

            if (objType == typeof(Int64))
            {
                Converter.GetBytes(d.buf8, (Int64)o);
                addToByteArray(d.buf8, sizeof(Int64), d);
                return;
            }

            if (objType == typeof(UInt64))
            {
                Converter.GetBytes(d.buf8, (UInt64)o);
                addToByteArray(d.buf8, sizeof(UInt64), d);
                return;
            }

            if (objType == typeof(Single))
            {
                Converter.GetBytes(d.buf8, (Single)o);
                addToByteArray(d.buf8, sizeof(Single), d);
                return;
            }

            if (objType == typeof(Double))
            {
                Converter.GetBytes(d.buf8, (Double)o);
                addToByteArray(d.buf8, sizeof(Double), d);
                return;
            }

            if (objType == typeof(DateTime))
            {
                Converter.GetBytes(d.buf8, ((DateTime)o).ToOADate());
                addToByteArray(d.buf8, sizeof(Double), d);
                return;
            }

            if (objType == typeof(string))
            {
                byte[] b = System.Text.Encoding.UTF8.GetBytes((String)o);

                // Size
                Converter.GetBytes(d.buf8, b.Length);
                addToByteArray(d.buf8, sizeof(Int32), d);

                // String
                addToByteArray(b, b.Length, d);
                return;
            }

            if (objType == typeof(byte[]))
            {
                byte[] b = (Byte[])o;

                // Size
                Converter.GetBytes(d.buf8, b.Length);
                addToByteArray(d.buf8, sizeof(Int32), d);

                // String
                addToByteArray(b, b.Length, d);
                return;
            }

            if (objType == typeof(MessageStructure))
            {
                d.buf8[0] = (byte)((MessageStructure)o);
                addToByteArray(d.buf8, 1, d);
                return;
            }

            throw new InvalidOperationException("Bilinmeyen tip!");
        }

        private void AddStruct(object command, SerializerDATA d)
        {
            Type t = command.GetType();

            FieldInfo[] fi = t.GetFields();

            for (int k = 0; k < fi.Length; k++)
            {
                if (fi[k].GetType() == typeof(string))
                {
                    var obj = fi[k].GetValue(command);

                    if (obj == null)
                        AddField(string.Empty, d);
                    else
                        AddField(obj, d);
                }
                else
                {
                    AddField(fi[k].GetValue(command), d);
                }
            }
        }

        public unsafe Message Serialize(MessageInfo msgInfo, object command, bool compress, MessageKey key)
        {
            // InternalHeader kadar boşluk bırakıyor. En son header yazacağız.
            SerializerDATA data = new SerializerDATA();

            // Internal header
            MessageHeader iHeader = msgInfo.CreateMessageHeader();

            // Unique ID
            UniqueID uid = UniqueID.CreateNew();
            iHeader.UniqueID1 = uid.Data1;
            iHeader.UniqueID2 = uid.Data2;
            iHeader.UniqueID3 = uid.Data3;
            iHeader.UniqueID4 = uid.Data4;

            // CommandID
            iHeader.CommandID = MessageAttribute.GetMessageID(command);

            // Komutumuzu yaz
            AddStruct(command, data);

            // Yeni boyutumuz
            iHeader.Size = data.size;
            iHeader.CRC = iHeader.Size ^ Message.CRC_MAGIC_NUMBER;

            // Verinin CMD kısmını sıkıştır. Header'a dokunma.
            if (compress)
            {
                throw new NotSupportedException("Compression is not supported");
            }
            else
            {
                iHeader.Structure = MessageStructure.Default;
            }

            // Encrypt the data
            if (key != null)
            {
                // Sign the data as encrypted
                iHeader.Structure = iHeader.Structure | MessageStructure.Encrypted;

                // Values
                int m = 0;
                byte[] pwd = key.Pwd;
                int keySize = pwd.Length;

                // Loop // TODO: Make some performance improvments.
                for (int k = Message.MESSAGE_INTERNAL_HEADER_SIZE; k < iHeader.Size; k++)
                {
                    data.data[k] = (byte)(data.data[k] ^ pwd[m]);
                    m++;
                    m = m % keySize;
                }
            }

            // Baş tarafa header yaz
            data.size = 0;
            AddStruct(iHeader, data);
            data.size = iHeader.Size;

            return new Message(iHeader, data.data, iHeader.Size);
        }
    }
}
