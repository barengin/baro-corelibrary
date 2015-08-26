using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.HAC.Metrics
{
    public class EDistance : IDistanceMetric
    {
        private const double NauticalMile = 1852;
        public const double DEG2RAD = Math.PI / 180.0;
        public const double RAD2DEG = 180.0 / Math.PI;

        /// <summary>
        /// Distance in meters, using the Harversine Formula (Great circle).
        /// </summary>
        /// <returns>Distance in meters</returns>
        /// <remarks>
        /// See:
        /// http://en.wikipedia.org/wiki/Great-circle_distance
        /// http://williams.best.vwh.net/avform.htm (Aviation Formulary)
        /// </remarks>
        public static double Distance(double longX1, double latY1, double longX2, double latY2)
        {
            double lon1 = DEG2RAD * -longX1;
            double lat1 = DEG2RAD * latY1;
            double lon2 = DEG2RAD * -longX2;
            double lat2 = DEG2RAD * latY2;

            double d = 2 * Math.Asin(Math.Sqrt(
                Math.Pow(Math.Sin((lat1 - lat2) / 2), 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Pow(Math.Sin((lon1 - lon2) / 2), 2)
            ));

            return (double)(NauticalMile * 60d * d / DEG2RAD);
        }

        public float GetDistance(object[] set1, object[] set2)
        {
            return (float)Distance((double)set1[0], (double)set1[1], (double)set2[0], (double)set2[1]);
        }
    }
}
