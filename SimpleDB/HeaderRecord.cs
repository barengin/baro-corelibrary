using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.SimpleDB
{
    internal class RawHeader
    {
        private byte[] data = new byte[128];
        private int position = 0;

        private static unsafe void SetInt32(byte[] buffer, int index, int value)
        {
            fixed (byte* numRef = &buffer[index])
            {
                *((int*)numRef) = value;
            }
        }

        internal void WriteInt32(int value)
        {
            Grow(4);
            SetInt32(data, position, value);
            position += 4;
        }

        private void Grow(int additionalStorage)
        {
            if (data.Length < position + additionalStorage)
            {
                Array.Resize<byte>(ref data, data.Length * 2);
            }
        }

        internal void WriteInt8(byte value)
        {
            Grow(1);
            data[position] = value;
            position += 1;
        }

        internal byte[] Trim()
        {
            Array.Resize<byte>(ref data, position);

            return data;
        }
    }

    internal static class HeaderRecord
    {
        internal static int CalculateRawRecordSize(Header header)
        {
            int size = 0;

            for (int k = 0; k < header.Count; k++)
            {
                SimpleField f = header[k];

                switch (f.Type)
                {
                    case SimpleFieldType.Int32:
                        size += 4;
                        break;

                    case SimpleFieldType.UInt32:
                        size += 4;
                        break;

                    case SimpleFieldType.Int16:
                        size += 2;
                        break;

                    case SimpleFieldType.UInt16:
                        size += 2;
                        break;

                    case SimpleFieldType.Int8:
                        size += 1;
                        break;

                    case SimpleFieldType.UInt8:
                        size += 1;
                        break;

                    case SimpleFieldType.Int64:
                        size += 8;
                        break;

                    case SimpleFieldType.UInt64:
                        size += 8;
                        break;

                    case SimpleFieldType.DateTime:
                        size += 8;
                        break;

                    case SimpleFieldType.Float32:
                        size += 4;
                        break;

                    case SimpleFieldType.Float64:
                        size += 8;
                        break;

                    case SimpleFieldType.String:
                        size += f.Size;
                        break;

                    case SimpleFieldType.ByteArray:
                        size += f.Size;
                        break;

                    default:
                        throw new SimpleDBException("Desteklenmeyen tip veya dosya bozulmuş");
                }

                if (f.Type == SimpleFieldType.ByteArray || f.Type == SimpleFieldType.String)
                    size += 4;
            }

            return size;        
        }

        /// <summary>
        /// Bu method sadece Header nesnesinin RAW olarak büyüklüğünü hesaplar!
        /// </summary>
        /// <param name="header"></param>
        /// <returns>Size in bytes</returns>
        internal static int CalculateHeaderSize(Header header)
        {
            int size = 4;

            for (int k = 0; k < header.Count; k++)
            {
                SimpleField f = header[k];

                size += 1;

                if (f.Type == SimpleFieldType.ByteArray || f.Type == SimpleFieldType.String)
                    size += 4;
            }

            return size;
        }

        internal static byte[] Create(Header header)
        {
            RawHeader h = new RawHeader();
            h.WriteInt32(header.Count);

            for (int k = 0; k < header.Count; k++)
            {
                SimpleField f = header[k];

                h.WriteInt8((byte)f.Type);

                if (f.Type == SimpleFieldType.ByteArray || f.Type == SimpleFieldType.String)
                    h.WriteInt32(f.Size);
            }

            return h.Trim();
        }

        internal static List<SimpleField> Parse(byte[] data)
        {
            List<SimpleField> list = new List<SimpleField>();

            int count = BitConverter.ToInt32(data, 0);
            int position = 4;

            for (int k = 0; k < count; k++)
            {
                SimpleFieldType ft = (SimpleFieldType)data[position];
                position++;

                switch (ft)
                {
                    case SimpleFieldType.Int32:
                        list.Add(new SimpleField(SimpleFieldType.Int32));
                        break;

                    case SimpleFieldType.UInt32:
                        list.Add(new SimpleField(SimpleFieldType.UInt32));
                        break;

                    case SimpleFieldType.Int16:
                        list.Add(new SimpleField(SimpleFieldType.Int16));
                        break;

                    case SimpleFieldType.UInt16:
                        list.Add(new SimpleField(SimpleFieldType.UInt16));
                        break;

                    case SimpleFieldType.Int8:
                        list.Add(new SimpleField(SimpleFieldType.Int8));
                        break;

                    case SimpleFieldType.UInt8:
                        list.Add(new SimpleField(SimpleFieldType.UInt8));
                        break;

                    case SimpleFieldType.Int64:
                        list.Add(new SimpleField(SimpleFieldType.Int64));
                        break;

                    case SimpleFieldType.UInt64:
                        list.Add(new SimpleField(SimpleFieldType.UInt64));
                        break;

                    case SimpleFieldType.DateTime:
                        list.Add(new SimpleField(SimpleFieldType.DateTime));
                        break;

                    case SimpleFieldType.Float32:
                        list.Add(new SimpleField(SimpleFieldType.Float32));
                        break;

                    case SimpleFieldType.Float64:
                        list.Add(new SimpleField(SimpleFieldType.Float64));
                        break;

                    case SimpleFieldType.String:
                        {
                            int size = BitConverter.ToInt32(data, position);
                            position += 4;

                            list.Add(new SimpleField(SimpleFieldType.String, size));
                        }
                        break;

                    case SimpleFieldType.ByteArray:
                        {
                            int size = BitConverter.ToInt32(data, position);
                            position += 4;

                            list.Add(new SimpleField(SimpleFieldType.ByteArray, size));
                        }
                        break;

                    default:
                        throw new SimpleDBException("Desteklenmeyen tip veya dosya bozulmuş");
                }

            }

            return list;
        }
    }
}
