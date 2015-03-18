using System;

using System.Collections.Generic;
using System.IO;

namespace Baro.CoreLibrary.Crc
{
    public enum CRCPolynomial : ulong
    {
        CRC_10 = 0x233L,
        CRC_12 = 0x80fL,
        CRC_16 = 0xa001L,
        CRC_16ARC = 0x8005L,
        CRC_24 = 0x1864cfbL,
        CRC_32 = 0x4c11db7L,
        CRC_64 = 0x1bL,
        CRC_8 = 0xe0L,
        CRC_CCITT16 = 0x1021L,
        CRC_CCITT32 = 0xedb88320L
    }

    public static class CRC
    {
        // Fields
        private static ulong m_polynomial = ulong.MaxValue;
        private static ulong[] m_table;

        // Methods
        public static ulong GenerateChecksum(byte[] data)
        {
            return GenerateChecksum(data, 0x20, CRCPolynomial.CRC_CCITT32);
        }

        public static ulong GenerateChecksum(byte[] data, int crcBitLength)
        {
            if ((crcBitLength < 8) || (crcBitLength > 0x40))
            {
                throw new ArgumentOutOfRangeException("Only Bit Length of 8 to 64 is supported");
            }
            if (crcBitLength == 8)
            {
                return GenerateChecksum(data, 0x20, CRCPolynomial.CRC_8);
            }
            if (crcBitLength <= 10)
            {
                return GenerateChecksum(data, 0x20, CRCPolynomial.CRC_10);
            }
            if (crcBitLength <= 12)
            {
                return GenerateChecksum(data, 0x20, CRCPolynomial.CRC_12);
            }
            if (crcBitLength <= 0x10)
            {
                return GenerateChecksum(data, 0x20, CRCPolynomial.CRC_CCITT16);
            }
            if (crcBitLength <= 0x18)
            {
                return GenerateChecksum(data, 0x20, CRCPolynomial.CRC_24);
            }
            if (crcBitLength <= 0x20)
            {
                return GenerateChecksum(data, 0x20, CRCPolynomial.CRC_CCITT32);
            }
            return GenerateChecksum(data, 0x20, CRCPolynomial.CRC_64);
        }

        public static ulong GenerateChecksum(byte[] data, int crcBitLength, CRCPolynomial polynomial)
        {
            return GenerateChecksum(data, crcBitLength, (ulong)polynomial, 0L);
        }

        public static ulong GenerateChecksum(byte[] data, int offset, int length)
        {
            return GenerateChecksum(data, offset, length, CRCPolynomial.CRC_CCITT32, 0L);
        }

        public static ulong GenerateChecksum(byte[] data, int crcBitLength, ulong polynomial)
        {
            return GenerateChecksum(data, crcBitLength, polynomial, 0L);
        }

        public static ulong GenerateChecksum(Stream stream, int crcBitLength, ulong polynomial)
        {
            if ((crcBitLength < 8) || (crcBitLength > 0x40))
            {
                throw new ArgumentOutOfRangeException("Only Bit Length of 8 to 64 is supported");
            }
            if (!stream.CanRead)
            {
                throw new ArgumentException("Unreadable stream");
            }
            if (stream.Length == 0L)
            {
                throw new ArgumentException("Stream is empty");
            }
            ulong seedCRC = 0L;
            GenerateCRCTable(crcBitLength, polynomial);
            byte[] buffer = new byte[0x200];
            int length = 0;
            while (stream.Position < stream.Length)
            {
                length = stream.Read(buffer, 0, buffer.Length);
                seedCRC = GenerateChecksum(buffer, 0, length, crcBitLength, polynomial, seedCRC);
            }
            return seedCRC;
        }

        public static ulong GenerateChecksum(byte[] data, int crcBitLength, ulong polynomial, ulong seedCRC)
        {
            return GenerateChecksum(data, 0, data.Length, crcBitLength, polynomial, seedCRC);
        }

        public static ulong GenerateChecksum(byte[] data, int offset, int length, CRCPolynomial polynomial, ulong seedCRC)
        {
            int crcBitLength = 0;
            switch (polynomial)
            {
                case CRCPolynomial.CRC_10:
                    crcBitLength = 10;
                    break;

                case CRCPolynomial.CRC_12:
                    crcBitLength = 12;
                    break;

                case CRCPolynomial.CRC_CCITT16:
                case CRCPolynomial.CRC_16ARC:
                case CRCPolynomial.CRC_16:
                    crcBitLength = 0x10;
                    break;

                case CRCPolynomial.CRC_64:
                    crcBitLength = 0x40;
                    break;

                case CRCPolynomial.CRC_8:
                    crcBitLength = 8;
                    break;

                case CRCPolynomial.CRC_24:
                    crcBitLength = 0x18;
                    break;

                case CRCPolynomial.CRC_32:
                case CRCPolynomial.CRC_CCITT32:
                    crcBitLength = 0x20;
                    break;
            }
            return GenerateChecksum(data, offset, length, crcBitLength, (ulong)polynomial, seedCRC);
        }

        public static ulong GenerateChecksum(byte[] data, int offset, int length, int crcBitLength, ulong polynomial, ulong seedCRC)
        {
            if ((crcBitLength < 8) || (crcBitLength > 0x40))
            {
                throw new ArgumentOutOfRangeException("Only Bit Length of 8 to 64 is supported");
            }
            ulong num = seedCRC;
            ulong num2 = 0L;
            int num3 = crcBitLength - 8;
            ulong num4 = 0L;
            for (int i = 0; i < crcBitLength; i++)
            {
                num4 |= ((ulong)1L) << i;
            }
            GenerateCRCTable(crcBitLength, polynomial);
            for (int j = offset; j < length; j++)
            {
                num2 = num >> num3;
                num2 ^= data[j];
                num = ((num << 8) ^ m_table[(int)((IntPtr)num2)]) & num4;
            }
            return num;
        }

        private static void GenerateCRCTable(int bits, ulong polynomial)
        {
            if (m_polynomial != polynomial)
            {
                ulong num = 0L;
                for (int i = 0; i < bits; i++)
                {
                    num |= ((ulong)1L) << i;
                }
                m_table = new ulong[0x100];
                for (ulong j = 0L; j < 0x100L; j += (ulong)1L)
                {
                    ulong num4 = j;
                    for (int k = 0; k < 8; k++)
                    {
                        if ((num4 & ((ulong)1L)) != 0L)
                        {
                            num4 = num4 >> 1;
                            num4 ^= polynomial;
                        }
                        else
                        {
                            num4 = num4 >> 1;
                        }
                    }
                    num4 &= num;
                    m_table[(int)((IntPtr)j)] = num4;
                }
            }
        }
    }
}
