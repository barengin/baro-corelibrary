using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    public class DataWriter : IDisposable, IBinaryWriter
    {
        private MemoryStream _stream;
        private BinaryWriter _bw;

        public DataWriter()
        {
            _stream = new MemoryStream();
            _bw = new BinaryWriter(_stream);
        }

        public DataWriter(int capacity)
        {
            _stream = new MemoryStream(capacity);
            _bw = new BinaryWriter(_stream);
        }

        public void WriteByte(byte b)
        {
            _bw.Write(b);
        }

        public void WriteInt32(int i)
        {
            _bw.Write(i);
        }

        public void WriteInt64(long i)
        {
            _bw.Write(i);
        }

        public void WriteSingle(float s)
        {
            _bw.Write(s);
        }

        public void WriteDouble(double d)
        {
            _bw.Write(d);
        }

        public void WriteString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                _bw.Write((int)0);
                return;
            }

            byte[] b = Encoding.UTF8.GetBytes(s);
            int l = b.Length;

            _bw.Write(l);
            _bw.Write(b);
        }

        public void WriteByteArray(byte[] b)
        {
            if (b == null)
            {
                _bw.Write((int)0);
                return;
            }

            int l = b.Length;

            _bw.Write(l);
            _bw.Write(b);
        }

        public void Dispose()
        {
            _bw.Dispose();
            _stream.Dispose();
        }

        public byte[] GetBuffer()
        {
            return _stream.GetBuffer();
        }

        public long Length
        {
            get { return _stream.Length; }
        }
    }
}
