using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.Extensions
{
    public static class DateTimeExtensions
    {
        public static int ToIntegerDate(this DateTime dt)
        {
            return (dt.Year * 10000) + (dt.Month * 100) + dt.Day;
        }

        /// <summary>
        /// Mobiliz'in WEB SERVİS içinde kullandığı tarih formatı
        /// </summary>
        /// <param name="d">Date Extension</param>
        /// <returns>dd-MM-yyyy HH:mm:ss</returns>
        public static string ToMobilizDate(this DateTime d)
        {
            return d.ToString("dd-MM-yyyy HH:mm:ss");
        }

        public static string ToMobilizDate2(this DateTime d)
        {
            return d.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime FromHexString(string value)
        {
            byte[] b = new byte[8];

            for (int k = 0; k < 8; k++)
            {
                b[k] = byte.Parse(value.Substring(k * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return DateTime.FromOADate(BitConverter.ToDouble(b, 0));
        }

        public static string ToHexString(this DateTime d)
        {
            byte[] b = BitConverter.GetBytes(d.ToOADate());

            StringBuilder sb = new StringBuilder();

            foreach (byte item in b)
            {
                sb.Append(item.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
