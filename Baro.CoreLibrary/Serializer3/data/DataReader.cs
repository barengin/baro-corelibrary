using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    public class DataReader : IBinaryReader
    {
        private MemoryStream _stream;
        private BinaryReader _br;

        public DataReader(byte[] buffer)
            : this(buffer, 0, buffer.Length)
        {
        }

        public DataReader(ArraySegment<byte> raw)
            : this(raw.Array, raw.Offset, raw.Count)
        {
        }

        public DataReader(byte[] buffer, int index, int count)
        {
            _stream = new MemoryStream(buffer, index, count);
            _br = new BinaryReader(_stream);
        }

        public long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        public byte ReadByte()
        {
            return _br.ReadByte();
        }

        public int ReadInt32()
        {
            return _br.ReadInt32();
        }

        public long ReadInt64()
        {
            return _br.ReadInt64();
        }

        public float ReadSingle()
        {
            return _br.ReadSingle();
        }

        public double ReadDouble()
        {
            return _br.ReadDouble();
        }

        public string ReadString()
        {
            int l = _br.ReadInt32();

            if (l == 0)
            {
                return null;
            }

            byte[] b = _br.ReadBytes(l);

            return Encoding.UTF8.GetString(b);
        }

        public byte[] ReadByteArray()
        {
            int l = _br.ReadInt32();

            if (l == 0)
            {
                return null;
            }

            return _br.ReadBytes(l);
        }
    }
}
