﻿using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.G3
{
    static class SinCos11
    {
        /// <summary>
        /// 11 bit sola kaydırılmış COS tablosu
        /// </summary>
        public static readonly int[] cos = new int[]
            {
                2048,2047,2046,2045,2043,2040,2036,2032,2028,2022,2016,2010,2003,1995,1987,1978,1968,1958,1947,
                1936,1924,1911,1898,1885,1870,1856,1840,1824,1808,1791,1773,1755,1736,1717,1697,1677,1656,1635,
                1613,1591,1568,1545,1521,1497,1473,1448,1422,1396,1370,1343,1316,1288,1260,1232,1203,1174,1145,
                1115,1085,1054,1024,992,961,929,897,865,832,800,767,733,700,666,632,598,564,530,495,460,425,390,
                355,320,285,249,214,178,142,107,71,35,0,-36,-72,-108,-143,-179,-215,-250,-286,-321,-356,-391,-426,
                -461,-496,-531,-565,-599,-633,-667,-701,-734,-768,-801,-833,-866,-898,-930,-962,-993,-1024,-1055,
                -1086,-1116,-1146,-1175,-1204,-1233,-1261,-1289,-1317,-1344,-1371,-1397,-1423,-1449,-1474,-1498,
                -1522,-1546,-1569,-1592,-1614,-1636,-1657,-1678,-1698,-1718,-1737,-1756,-1774,-1792,-1809,-1825,
                -1841,-1857,-1871,-1886,-1899,-1912,-1925,-1937,-1948,-1959,-1969,-1979,-1988,-1996,-2004,-2011,
                -2017,-2023,-2029,-2033,-2037,-2041,-2044,-2046,-2047,-2048,-2048,-2048,-2047,-2046,-2044,-2041,
                -2037,-2033,-2029,-2023,-2017,-2011,-2004,-1996,-1988,-1979,-1969,-1959,-1948,-1937,-1925,-1912,
                -1899,-1886,-1871,-1857,-1841,-1825,-1809,-1792,-1774,-1756,-1737,-1718,-1698,-1678,-1657,-1636,
                -1614,-1592,-1569,-1546,-1522,-1498,-1474,-1449,-1423,-1397,-1371,-1344,-1317,-1289,-1261,-1233,
                -1204,-1175,-1146,-1116,-1086,-1055,-1025,-993,-962,-930,-898,-866,-833,-801,-768,-734,-701,-667,
                -633,-599,-565,-531,-496,-461,-426,-391,-356,-321,-286,-250,-215,-179,-143,-108,-72,-36,-1,35,71,
                107,142,178,214,249,285,320,355,390,425,460,495,530,564,598,632,666,700,733,767,800,832,865,897,
                929,961,992,1023,1054,1085,1115,1145,1174,1203,1232,1260,1288,1316,1343,1370,1396,1422,1448,1473,
                1497,1521,1545,1568,1591,1613,1635,1656,1677,1697,1717,1736,1755,1773,1791,1808,1824,1840,1856,
                1870,1885,1898,1911,1924,1936,1947,1958,1968,1978,1987,1995,2003,2010,2016,2022,2028,2032,2036,
                2040,2043,2045,2046,2047
            };

        /// <summary>
        /// 11 bit sağa kaydırılmış SIN tablosu
        /// </summary>
        public static readonly int[] sin = new int[]
        {
            0,35,71,107,142,178,214,249,285,320,355,390,425,460,495,530,564,598,632,666,700,733,767,800,832,
            865,897,929,961,992,1024,1054,1085,1115,1145,1174,1203,1232,1260,1288,1316,1343,1370,1396,1422,
            1448,1473,1497,1521,1545,1568,1591,1613,1635,1656,1677,1697,1717,1736,1755,1773,1791,1808,1824,
            1840,1856,1870,1885,1898,1911,1924,1936,1947,1958,1968,1978,1987,1995,2003,2010,2016,2022,2028,2032,
            2036,2040,2043,2045,2046,2047,2048,2047,2046,2045,2043,2040,2036,2032,2028,2022,2016,2010,2003,1995,
            1987,1978,1968,1958,1947,1936,1924,1911,1898,1885,1870,1856,1840,1824,1808,1791,1773,1755,1736,1717,
            1697,1677,1656,1635,1613,1591,1568,1545,1521,1497,1473,1448,1422,1396,1370,1343,1316,1288,1260,1232,
            1203,1174,1145,1115,1085,1054,1024,992,961,929,897,865,832,800,767,733,700,666,632,598,564,530,495,
            460,425,390,355,320,285,249,214,178,142,107,71,35,0,-36,-72,-108,-143,-179,-215,-250,-286,-321,-356,
            -391,-426,-461,-496,-531,-565,-599,-633,-667,-701,-734,-768,-801,-833,-866,-898,-930,-962,-993,-1024,
            -1055,-1086,-1116,-1146,-1175,-1204,-1233,-1261,-1289,-1317,-1344,-1371,-1397,-1423,-1449,-1474,-1498,
            -1522,-1546,-1569,-1592,-1614,-1636,-1657,-1678,-1698,-1718,-1737,-1756,-1774,-1792,-1809,-1825,-1841,
            -1857,-1871,-1886,-1899,-1912,-1925,-1937,-1948,-1959,-1969,-1979,-1988,-1996,-2004,-2011,-2017,-2023,
            -2029,-2033,-2037,-2041,-2044,-2046,-2047,-2048,-2048,-2048,-2047,-2046,-2044,-2041,-2037,-2033,-2029,
            -2023,-2017,-2011,-2004,-1996,-1988,-1979,-1969,-1959,-1948,-1937,-1925,-1912,-1899,-1886,-1871,-1857,
            -1841,-1825,-1809,-1792,-1774,-1756,-1737,-1718,-1698,-1678,-1657,-1636,-1614,-1592,-1569,-1546,-1522,
            -1498,-1474,-1449,-1423,-1397,-1371,-1344,-1317,-1289,-1261,-1233,-1204,-1175,-1146,-1116,-1086,-1055,
            -1025,-993,-962,-930,-898,-866,-833,-801,-768,-734,-701,-667,-633,-599,-565,-531,-496,-461,-426,-391,
            -356,-321,-286,-250,-215,-179,-143,-108,-72,-36
        };
    }
}
