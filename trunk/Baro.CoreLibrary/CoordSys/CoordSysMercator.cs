using System;

using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.CoordSys
{
    internal sealed class CoordSysMercator : CoordSys
    {
        private const double WGS84A = 6378137d;
        private const double WGS84E = 0.081819191310871814d;

        private const double QPI = Math.PI / 4d;
        private const double HPI = Math.PI / 2d;

        public override CoordSysType CoordSysType
        {
            get { return CoordSysType.Mercator; }
        }

        public override void LL2Flat(ref double XLong, ref double YLat)
        {
            double lon = XLong * DEG2RAD;
            double lat = YLat * DEG2RAD;

            XLong = WGS84A * lon;
            YLat = WGS84A * Math.Log(Math.Tan(QPI + lat / 2) * Math.Pow(((1 - WGS84E * Math.Sin(lat)) / (1 + WGS84E * Math.Sin(lat))), (WGS84E / 2)));
        }

        private const int IterasyonSayisi = 10;

        public override void Flat2LL(ref double XLong, ref double YLat)
        {
            double lon = XLong / WGS84A;

            double dphi;
            double ts = Math.Exp(-YLat / WGS84A);
            double lat = (HPI) - 2.0 * Math.Atan(ts);
            int i = IterasyonSayisi;

            do
            {
                double con = WGS84E * Math.Sin(lat);
                dphi = (HPI) - 2.0 * Math.Atan(ts * Math.Pow((1.0 - con) / (1.0 + con), WGS84E / 2)) - lat;
                lat += dphi;
            }
            while (Math.Abs(dphi) > 0.0000000001 && --i > 0);

            XLong = lon / DEG2RAD;
            YLat = lat / DEG2RAD;
        }
    }
}
