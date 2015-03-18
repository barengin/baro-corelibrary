using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Baro.CoreLibrary.Google
{
    public static class GeocodeAPI
    {
        public const string ADDRESS_PATTERN = "https://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor={1}&key={2}";

        public static string Geocode(string key, string address, bool sensor)
        {
            string result;
            address = string.Format(ADDRESS_PATTERN, address, sensor.ToString().ToLower(), key);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
                result = sr.ReadToEnd();
            }

            return result;
        }
    }
}
