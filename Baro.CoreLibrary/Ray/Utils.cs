using Baro.CoreLibrary.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    internal class Utils
    {
        public static string ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new RayInvalidKeyException("Key is null or empty !!!");
            }

            key = key.Trim();

            return key;
        }

        public static IEnumerable<KeyValuePair<string, string>> StartsWith(SortedList<string, string> list, string value)
        {
            // Search
            int result = BinarySearchHelper.BinarySearch<string>(list.Keys, 0, list.Keys.Count, value, (x, y) =>
            {
                return string.Compare(x.Substring(0, Math.Min(x.Length, y.Length)), y);
            });

            // Not found
            if (result < 0)
                return null;

            List<KeyValuePair<string, string>> resultList = new List<KeyValuePair<string, string>>();
            int ResultEksi = result - 1, ResultArtı = result;

            while (ResultEksi >= 0)
            {
                int v = string.Compare(value,
                    list.Keys[ResultEksi].Substring(0, Math.Min(value.Length, list.Keys[ResultEksi].Length)));

                if (v != 0)
                {
                    break;
                }

                resultList.Add(new KeyValuePair<string, string>(list.Keys[ResultEksi], list.Values[ResultEksi]));
                ResultEksi--;
            }

            while (ResultArtı < list.Keys.Count)
            {
                int v = string.Compare(value,
                    list.Keys[ResultArtı].Substring(0, Math.Min(value.Length, list.Keys[ResultArtı].Length)));

                if (v != 0)
                {
                    break;
                }

                resultList.Add(new KeyValuePair<string, string>(list.Keys[ResultArtı], list.Values[ResultArtı]));
                ResultArtı++;
            }

            return resultList;
        }

        public static IEnumerable<TResult> StartsWith<TResult>(SortedList<string, TResult> list, string value)
        {
            // Search
            int result = BinarySearchHelper.BinarySearch<string>(list.Keys, 0, list.Keys.Count, value, (x, y) =>
            {
                return string.Compare(x.Substring(0, Math.Min(x.Length, y.Length)), y);
            });

            // Not found
            if (result < 0)
                return null;

            List<TResult> resultList = new List<TResult>();
            int ResultEksi = result - 1, ResultArtı = result;

            while (ResultEksi >= 0)
            {
                int v = string.Compare(value,
                    list.Keys[ResultEksi].Substring(0, Math.Min(value.Length, list.Keys[ResultEksi].Length)));

                if (v != 0)
                {
                    break;
                }

                resultList.Add(list.Values[ResultEksi]);
                ResultEksi--;
            }

            while (ResultArtı < list.Keys.Count)
            {
                int v = string.Compare(value,
                    list.Keys[ResultArtı].Substring(0, Math.Min(value.Length, list.Keys[ResultArtı].Length)));

                if (v != 0)
                {
                    break;
                }

                resultList.Add(list.Values[ResultArtı]);
                ResultArtı++;
            }

            return resultList;
        }
    }
}
