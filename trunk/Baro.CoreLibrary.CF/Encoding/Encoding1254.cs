using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Encoding
{
    /// <summary>
    /// Tamamen bize özel bir Türkçe encoding kütüphanesi
    /// </summary>
    internal sealed unsafe class Encoding1254 : Encoding
    {
        #region Chars 1254
        private byte[][] bytes1254 = new byte[256][];
        private char[] chars1254 = new char[] {
            '\u0000','\u0001','\u0002','\u0003','\u0004','\u0005','\u0006','\u0007',
            '\u0008','\u0009','\u000A','\u000B','\u000C','\u000D','\u000E','\u000F',
            '\u0010','\u0011','\u0012','\u0013','\u0014','\u0015','\u0016','\u0017',
            '\u0018','\u0019','\u001A','\u001B','\u001C','\u001D','\u001E','\u001F',
            '\u0020','\u0021','\u0022','\u0023','\u0024','\u0025','\u0026','\u0027',
            '\u0028','\u0029','\u002A','\u002B','\u002C','\u002D','\u002E','\u002F',
            '\u0030','\u0031','\u0032','\u0033','\u0034','\u0035','\u0036','\u0037',
            '\u0038','\u0039','\u003A','\u003B','\u003C','\u003D','\u003E','\u003F',
            '\u0040','\u0041','\u0042','\u0043','\u0044','\u0045','\u0046','\u0047',
            '\u0048','\u0049','\u004A','\u004B','\u004C','\u004D','\u004E','\u004F',
            '\u0050','\u0051','\u0052','\u0053','\u0054','\u0055','\u0056','\u0057',
            '\u0058','\u0059','\u005A','\u005B','\u005C','\u005D','\u005E','\u005F',
            '\u0060','\u0061','\u0062','\u0063','\u0064','\u0065','\u0066','\u0067',
            '\u0068','\u0069','\u006A','\u006B','\u006C','\u006D','\u006E','\u006F',
            '\u0070','\u0071','\u0072','\u0073','\u0074','\u0075','\u0076','\u0077',
            '\u0078','\u0079','\u007A','\u007B','\u007C','\u007D','\u007E','\u007F',
            '\u0080','\u0081','\u201A','\u0192','\u201E','\u2026','\u2020','\u2021',
            '\u02C6','\u2030','\u0160','\u2039','\u0152','\u008D','\u008E','\u008F',
            '\u0090','\u2018','\u2019','\u201C','\u201D','\u2022','\u2013','\u2014',
            '\u02DC','\u2122','\u0161','\u203A','\u0153','\u009D','\u009E','\u0178',
            '\u00A0','\u00A1','\u00A2','\u00A3','\u00A4','\u00A5','\u00A6','\u00A7',
            '\u00A8','\u00A9','\u00AA','\u00AB','\u00AC','\u00AD','\u00AE','\u00AF',
            '\u00B0','\u00B1','\u00B2','\u00B3','\u00B4','\u00B5','\u00B6','\u00B7',
            '\u00B8','\u00B9','\u00BA','\u00BB','\u00BC','\u00BD','\u00BE','\u00BF',
            '\u00C0','\u00C1','\u00C2','\u00C3','\u00C4','\u00C5','\u00C6','\u00C7',
            '\u00C8','\u00C9','\u00CA','\u00CB','\u00CC','\u00CD','\u00CE','\u00CF',
            '\u011E','\u00D1','\u00D2','\u00D3','\u00D4','\u00D5','\u00D6','\u00D7',
            '\u00D8','\u00D9','\u00DA','\u00DB','\u00DC','\u0130','\u015E','\u00DF',
            '\u00E0','\u00E1','\u00E2','\u00E3','\u00E4','\u00E5','\u00E6','\u00E7',
            '\u00E8','\u00E9','\u0119','\u00EB','\u0117','\u00ED','\u00EE','\u012B',
            '\u011F','\u00F1','\u00F2','\u00F3','\u00F4','\u00F5','\u00F6','\u00F7',
            '\u00F8','\u00F9','\u00FA','\u00FB','\u00FC','\u0131','\u015F','\u00FF'
        };

        #endregion

        #region Order bytes
        private byte[] order1254 = new byte[256];
        #endregion

        public override char this[byte index]
        {
            get
            {
                return chars1254[index];
            }
        }

        #region Unrecognized Char
        public const char UnrecognizedChar = '?';
        private const byte UnrecognizedCharValue = (byte)UnrecognizedChar;

        #endregion

        public override byte this[char index]
        {
            get
            {
                int h = (int)index;
                byte[] m = bytes1254[h >> 8];

                if (m == null)
                    return UnrecognizedCharValue;

                byte k = m[h & 0xff];

                if (k == 0)
                    return UnrecognizedCharValue;

                return k;
            }
        }

        public override string GetString(byte[] t, int size)
        {
            StringBuilder s = new StringBuilder(size);

            for (int x = 0; x < size; x++)
                s.Append(chars1254[t[x]]);

            return s.ToString();
        }

        public override string GetString(byte[] t)
        {
            StringBuilder s = new StringBuilder(t.Length);

            for (int x = 0; x < t.Length; x++)
                s.Append(chars1254[t[x]]);

            return s.ToString();
        }

        public override string GetString(byte* t, int size)
        {
            StringBuilder s = new StringBuilder(size);

            while (size-- != 0)
                s.Append(chars1254[*t++]);

            return s.ToString();
        }

        public Encoding1254()
        {
            #region Generate 1254 Charset
            for (int x = 0; x < 256; x++)
            {
                int c = (int)chars1254[x];

                if (bytes1254[c >> 8] == null)
                    bytes1254[c >> 8] = new byte[256];

                bytes1254[c >> 8][c & 0xff] = (byte)x;
            }

            #endregion

            UpdateByteOrders();
        }

        private void UpdateByteOrders()
        {
            UpdateByteOrder(0, 0);
            UpdateByteOrder(1, 1);
            UpdateByteOrder(2, 2);
            UpdateByteOrder(3, 3);
            UpdateByteOrder(4, 4);
            UpdateByteOrder(5, 5);
            UpdateByteOrder(6, 6);
            UpdateByteOrder(7, 7);
            UpdateByteOrder(8, 8);
            UpdateByteOrder(10, 9);
            UpdateByteOrder(11, 10);
            UpdateByteOrder(12, 11);
            UpdateByteOrder(13, 12);
            UpdateByteOrder(14, 13);
            UpdateByteOrder(15, 14);
            UpdateByteOrder(16, 15);
            UpdateByteOrder(17, 16);
            UpdateByteOrder(18, 17);
            UpdateByteOrder(19, 18);
            UpdateByteOrder(20, 19);
            UpdateByteOrder(21, 20);
            UpdateByteOrder(22, 21);
            UpdateByteOrder(23, 22);
            UpdateByteOrder(24, 23);
            UpdateByteOrder(25, 24);
            UpdateByteOrder(26, 25);
            UpdateByteOrder(27, 26);
            UpdateByteOrder(28, 27);
            UpdateByteOrder(29, 28);
            UpdateByteOrder(30, 29);
            UpdateByteOrder(31, 30);
            UpdateByteOrder(33, 31);
            UpdateByteOrder(34, 32);
            UpdateByteOrder(35, 33);
            UpdateByteOrder(36, 34);
            UpdateByteOrder(37, 35);
            UpdateByteOrder(38, 36);
            UpdateByteOrder(39, 37);
            UpdateByteOrder(40, 38);
            UpdateByteOrder(41, 39);
            UpdateByteOrder(42, 40);
            UpdateByteOrder(43, 41);
            UpdateByteOrder(44, 42);
            UpdateByteOrder(45, 43);
            UpdateByteOrder(46, 44);
            UpdateByteOrder(47, 45);
            UpdateByteOrder(58, 46);
            UpdateByteOrder(59, 47);
            UpdateByteOrder(60, 48);
            UpdateByteOrder(61, 49);
            UpdateByteOrder(62, 50);
            UpdateByteOrder(63, 51);
            UpdateByteOrder(64, 52);
            UpdateByteOrder(91, 53);
            UpdateByteOrder(92, 54);
            UpdateByteOrder(93, 55);
            UpdateByteOrder(94, 56);
            UpdateByteOrder(95, 57);
            UpdateByteOrder(96, 58);
            UpdateByteOrder(123, 59);
            UpdateByteOrder(124, 60);
            UpdateByteOrder(125, 61);
            UpdateByteOrder(126, 62);
            UpdateByteOrder(127, 63);
            UpdateByteOrder(160, 64);
            UpdateByteOrder(161, 65);
            UpdateByteOrder(162, 66);
            UpdateByteOrder(163, 67);
            UpdateByteOrder(164, 68);
            UpdateByteOrder(165, 69);
            UpdateByteOrder(166, 70);
            UpdateByteOrder(167, 71);
            UpdateByteOrder(168, 72);
            UpdateByteOrder(169, 73);
            UpdateByteOrder(170, 74);
            UpdateByteOrder(171, 75);
            UpdateByteOrder(172, 76);
            UpdateByteOrder(173, 77);
            UpdateByteOrder(174, 78);
            UpdateByteOrder(175, 79);
            UpdateByteOrder(176, 80);
            UpdateByteOrder(177, 81);
            UpdateByteOrder(178, 82);
            UpdateByteOrder(179, 83);
            UpdateByteOrder(180, 84);
            UpdateByteOrder(181, 85);
            UpdateByteOrder(182, 86);
            UpdateByteOrder(183, 87);
            UpdateByteOrder(184, 88);
            UpdateByteOrder(185, 89);
            UpdateByteOrder(186, 90);
            UpdateByteOrder(187, 91);
            UpdateByteOrder(188, 92);
            UpdateByteOrder(189, 93);
            UpdateByteOrder(190, 94);
            UpdateByteOrder(191, 95);
            UpdateByteOrder(215, 96);
            UpdateByteOrder(247, 97);
            UpdateByteOrder(140, 98);
            UpdateByteOrder(156, 99);
            UpdateByteOrder(138, 100);
            UpdateByteOrder(154, 101);
            UpdateByteOrder(159, 102);
            UpdateByteOrder(131, 103);
            UpdateByteOrder(136, 104);
            UpdateByteOrder(152, 105);
            UpdateByteOrder(150, 106);
            UpdateByteOrder(151, 107);
            UpdateByteOrder(145, 108);
            UpdateByteOrder(146, 109);
            UpdateByteOrder(130, 110);
            UpdateByteOrder(147, 111);
            UpdateByteOrder(148, 112);
            UpdateByteOrder(132, 113);
            UpdateByteOrder(134, 114);
            UpdateByteOrder(135, 115);
            UpdateByteOrder(149, 116);
            UpdateByteOrder(133, 117);
            UpdateByteOrder(137, 118);
            UpdateByteOrder(139, 119);
            UpdateByteOrder(155, 120);
            UpdateByteOrder(128, 121);
            UpdateByteOrder(153, 122);
            UpdateByteOrder(9, 123);
            UpdateByteOrder(32, 124);
            UpdateByteOrder(65, 125);
            UpdateByteOrder(97, 125);
            UpdateByteOrder(193, 125);
            UpdateByteOrder(225, 125);
            UpdateByteOrder(192, 125);
            UpdateByteOrder(224, 125);
            UpdateByteOrder(194, 125);
            UpdateByteOrder(226, 125);
            UpdateByteOrder(196, 125);
            UpdateByteOrder(228, 125);
            UpdateByteOrder(195, 125);
            UpdateByteOrder(227, 125);
            UpdateByteOrder(197, 125);
            UpdateByteOrder(229, 125);
            UpdateByteOrder(198, 125);
            UpdateByteOrder(230, 125);
            UpdateByteOrder(66, 126);
            UpdateByteOrder(98, 126);
            UpdateByteOrder(67, 127);
            UpdateByteOrder(99, 127);
            UpdateByteOrder(199, 128);
            UpdateByteOrder(231, 128);
            UpdateByteOrder(68, 129);
            UpdateByteOrder(100, 129);
            UpdateByteOrder(69, 130);
            UpdateByteOrder(101, 130);
            UpdateByteOrder(201, 130);
            UpdateByteOrder(233, 130);
            UpdateByteOrder(200, 130);
            UpdateByteOrder(232, 130);
            UpdateByteOrder(202, 130);
            UpdateByteOrder(234, 130);
            UpdateByteOrder(203, 130);
            UpdateByteOrder(235, 130);
            UpdateByteOrder(70, 131);
            UpdateByteOrder(102, 131);
            UpdateByteOrder(71, 132);
            UpdateByteOrder(103, 132);
            UpdateByteOrder(208, 133);
            UpdateByteOrder(240, 133);
            UpdateByteOrder(72, 134);
            UpdateByteOrder(104, 134);
            UpdateByteOrder(73, 135);
            UpdateByteOrder(253, 135);
            UpdateByteOrder(205, 135);
            UpdateByteOrder(237, 135);
            UpdateByteOrder(204, 135);
            UpdateByteOrder(236, 135);
            UpdateByteOrder(206, 135);
            UpdateByteOrder(238, 135);
            UpdateByteOrder(207, 135);
            UpdateByteOrder(239, 135);
            UpdateByteOrder(221, 136);
            UpdateByteOrder(105, 136);
            UpdateByteOrder(74, 137);
            UpdateByteOrder(106, 137);
            UpdateByteOrder(75, 138);
            UpdateByteOrder(107, 138);
            UpdateByteOrder(76, 139);
            UpdateByteOrder(108, 139);
            UpdateByteOrder(77, 140);
            UpdateByteOrder(109, 140);
            UpdateByteOrder(78, 141);
            UpdateByteOrder(110, 141);
            UpdateByteOrder(209, 141);
            UpdateByteOrder(241, 141);
            UpdateByteOrder(79, 142);
            UpdateByteOrder(111, 142);
            UpdateByteOrder(211, 142);
            UpdateByteOrder(243, 142);
            UpdateByteOrder(210, 142);
            UpdateByteOrder(242, 142);
            UpdateByteOrder(212, 142);
            UpdateByteOrder(244, 142);
            UpdateByteOrder(213, 142);
            UpdateByteOrder(245, 142);
            UpdateByteOrder(216, 142);
            UpdateByteOrder(248, 142);
            UpdateByteOrder(214, 143);
            UpdateByteOrder(246, 143);
            UpdateByteOrder(80, 144);
            UpdateByteOrder(112, 144);
            UpdateByteOrder(81, 145);
            UpdateByteOrder(113, 145);
            UpdateByteOrder(82, 146);
            UpdateByteOrder(114, 146);
            UpdateByteOrder(83, 147);
            UpdateByteOrder(115, 147);
            UpdateByteOrder(223, 147);
            UpdateByteOrder(222, 148);
            UpdateByteOrder(254, 148);
            UpdateByteOrder(84, 149);
            UpdateByteOrder(116, 149);
            UpdateByteOrder(85, 150);
            UpdateByteOrder(117, 150);
            UpdateByteOrder(218, 150);
            UpdateByteOrder(250, 150);
            UpdateByteOrder(217, 150);
            UpdateByteOrder(249, 150);
            UpdateByteOrder(219, 150);
            UpdateByteOrder(251, 150);
            UpdateByteOrder(220, 151);
            UpdateByteOrder(252, 151);
            UpdateByteOrder(86, 152);
            UpdateByteOrder(118, 152);
            UpdateByteOrder(87, 153);
            UpdateByteOrder(119, 153);
            UpdateByteOrder(88, 154);
            UpdateByteOrder(120, 154);
            UpdateByteOrder(89, 155);
            UpdateByteOrder(121, 155);
            UpdateByteOrder(255, 155);
            UpdateByteOrder(90, 156);
            UpdateByteOrder(122, 156);
            UpdateByteOrder(48, 157);
            UpdateByteOrder(49, 158);
            UpdateByteOrder(50, 159);
            UpdateByteOrder(51, 160);
            UpdateByteOrder(52, 161);
            UpdateByteOrder(53, 162);
            UpdateByteOrder(54, 163);
            UpdateByteOrder(55, 164);
            UpdateByteOrder(56, 165);
            UpdateByteOrder(57, 166);
            UpdateByteOrder(129, 167);
            UpdateByteOrder(141, 167);
            UpdateByteOrder(142, 167);
            UpdateByteOrder(143, 167);
            UpdateByteOrder(144, 167);
            UpdateByteOrder(157, 167);
            UpdateByteOrder(158, 167);
        }

        private void UpdateByteOrder(byte char1254, byte orderValue)
        {
            order1254[char1254] = orderValue;
        }

        public override void GetBytesInto(string t, byte* buffer, int bufferSize)
        {
            int len = Math.Min(bufferSize, t.Length);

            for (int x = 0; x < len; x++)
                buffer[x] = this[t[x]];
        }

        public override void GetBytesInto(string t, byte[] buffer, int bufferSize)
        {
            int len = Math.Min(Math.Min(bufferSize, t.Length), buffer.Length);

            for (int x = 0; x < len; x++)
                buffer[x] = this[t[x]];
        }

        public override byte[] GetBytes(string t)
        {
            byte[] b = new byte[t.Length];

            for (int x = 0; x < t.Length; x++)
                b[x] = this[t[x]];

            return b;
        }

        private int GetByteOrder(byte p)
        {
            return order1254[p];
        }

        private int GetByteOrder(char p)
        {
            return order1254[this[p]];
        }

        public override int Compare(byte* x, int sizex, byte* y, int sizey)
        {
            int minlen = (sizex < sizey) ? sizex : sizey;

            for (int k = 0; k < minlen; k++)
            {
                int xa = GetByteOrder(x[k]) - GetByteOrder(y[k]);

                if (xa < 0)
                {
                    return -1;
                }
                else if (xa > 0)
                {
                    return 1;
                }
            }

            if (sizex == sizey)
            {
                return 0;
            }
            else
            {
                if (sizex < sizey)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        public override int Compare(string a, string b)
        {
            int minlen = (a.Length < b.Length) ? a.Length : b.Length;

            for (int k = 0; k < minlen; k++)
            {
                int xa = GetByteOrder(a[k]) - GetByteOrder(b[k]);

                if (xa < 0)
                {
                    return -1;
                }
                else if (xa > 0)
                {
                    return 1;
                }
            }

            if (a.Length == b.Length)
            {
                return 0;
            }
            else
            {
                if (a.Length < b.Length)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        public override int CodePage
        {
            get { return 1254; }
        }
    }
}
