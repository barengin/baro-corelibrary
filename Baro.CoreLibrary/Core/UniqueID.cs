using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.Core
{
    public enum UniqueIDFormat
    {
        Legacy,
        Hex,
        HexCRC
    }

    public struct UniqueID
    {
        private const uint MAGIC_NUMBER = 0x7EC40834;

        private uint _data1;
        private uint _data2;
        private uint _data3;
        private uint _data4;
        private uint _crc;

        public uint Data1
        {
            get { return _data1; }
        }

        public uint Data2
        {
            get { return _data2; }
        }

        public uint Data3
        {
            get { return _data3; }
        }

        public uint Data4
        {
            get { return _data4; }
        }

        public uint Crc
        {
            get { return _crc; }
        }

        public override string ToString()
        {
            return ToString(UniqueIDFormat.Hex);
        }

        public string ToString(UniqueIDFormat format)
        {
            switch (format)
            {
                case UniqueIDFormat.Legacy:
                    return string.Format("{0}-{1}-{2}-{3}-{4}", _data1.ToString("x8"), _data2.ToString("x8"), _data3.ToString("x8"), _data4.ToString("x8"), _crc.ToString("x8"));
                case UniqueIDFormat.Hex:
                    return string.Format("{0}{1}{2}{3}", _data1.ToString("x8"), _data2.ToString("x8"), _data3.ToString("x8"), _data4.ToString("x8"));
                case UniqueIDFormat.HexCRC:
                    return string.Format("{0}{1}{2}{3}{4}", _data1.ToString("x8"), _data2.ToString("x8"), _data3.ToString("x8"), _data4.ToString("x8"), _crc.ToString("x8"));
                default:
                    throw new InvalidOperationException("invalid format enum");
            }
        }

        public UniqueID(uint data1, uint data2, uint data3, uint data4)
        {
            _data1 = data1;
            _data2 = data2;
            _data3 = data3;
            _data4 = data4;

            _crc = MAGIC_NUMBER;
            _crc = (_crc >> 8) ^ _data1 ^ (_crc & 0xff);
            _crc = (_crc >> 8) ^ _data2 ^ (_crc & 0xff);
            _crc = (_crc >> 8) ^ _data3 ^ (_crc & 0xff);
            _crc = (_crc >> 8) ^ _data4 ^ (_crc & 0xff);
        }

        public static bool TryParse(uint data1, uint data2, uint data3, uint data4, uint crc, out UniqueID uid)
        {
            uint _crc = MAGIC_NUMBER;
            _crc = (_crc >> 8) ^ data1 ^ (_crc & 0xff);
            _crc = (_crc >> 8) ^ data2 ^ (_crc & 0xff);
            _crc = (_crc >> 8) ^ data3 ^ (_crc & 0xff);
            _crc = (_crc >> 8) ^ data4 ^ (_crc & 0xff);

            if (_crc != crc)
            {
                // CRC Error !!!
                uid = new UniqueID();
                return false;
            }
            else
            {
                // OK
                uid = new UniqueID(data1, data2, data3, data4);
                return true;
            }
        }

        public static UniqueID CreateNew()
        {
            uint d1 = ThreadSafeRandom.Next();
            uint d2 = ThreadSafeRandom.Next();
            uint d3 = ThreadSafeRandom.Next();
            uint d4 = ThreadSafeRandom.Next();

            return new UniqueID(d1, d2, d3, d4);
        }

        public static UniqueID Parse(string id)
        {
            if (id.Length == 32)
            {
                uint d1 = uint.Parse(id.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);
                uint d2 = uint.Parse(id.Substring(8, 8), System.Globalization.NumberStyles.HexNumber);
                uint d3 = uint.Parse(id.Substring(16, 8), System.Globalization.NumberStyles.HexNumber);
                uint d4 = uint.Parse(id.Substring(24, 8), System.Globalization.NumberStyles.HexNumber);

                return new UniqueID(d1, d2, d3, d4);
            }

            throw new InvalidOperationException("Sadece 32 karakterli ve bitişik hex yazımı destekleniyor");
        }
    }
}
