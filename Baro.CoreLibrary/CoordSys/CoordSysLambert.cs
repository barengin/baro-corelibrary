using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.CoordSys
{
    public sealed class CoordSysLambert : CoordSys
    {
        //CONSTANTS WGS84
        private const double WGS84_A = 6378137d;
        private const double WGS84_F = 1d / 298.25722356d;
        private const double TurkLamb1stPar = 37.5d;
        private const double TurkLamb2ndPar = 40.5d;
        private const double TurkLongOrigin = 36d;
        private const double TurkLatOrigin = 25d;
        private const double TurkFalseEast = 1003827.11d;
        private const double TurkFalseNorth = -1183453.08d;
        private const double QPI = Math.PI / 4d;

        //
        private const double phi1 = TurkLamb1stPar * DEG2RAD;
        private const double phi2 = TurkLamb2ndPar * DEG2RAD;
        private const double phiF = TurkLatOrigin * DEG2RAD;
        private const double lambdaF = TurkLongOrigin * DEG2RAD;

        //
        private const double phi1Cos = 0.79335334028994076d;
        private const double phi1Sin = 0.60876142901040764d;

        private const double phi2Cos = 0.76040596559853946d;
        private const double phi2Sin = 0.64944804833193004d;

        private const double phiFSin = 0.42261826174198425d;

        public override CoordSysType CoordSysType
        {
            get { return CoordSysType.Lambert; }
        }

        public override void LL2Flat(ref double XLong, ref double YLat)
        {
            //convert degrees to rads
            double phi = YLat * DEG2RAD;
            double lambda = XLong * DEG2RAD;

            double e = Math.Pow((2 * WGS84_F - Math.Pow(WGS84_F, 2)), 0.5); //eccentricity

            double m1 = phi1Cos / Math.Pow((1 - ((Math.Pow(e, 2)) * Math.Pow(phi1Sin, 2))), 0.5);
            double m2 = phi2Cos / Math.Pow((1 - ((Math.Pow(e, 2)) * Math.Pow(phi2Sin, 2))), 0.5);

            double t = Math.Tan((QPI) - phi / 2) / Math.Pow(((1 - e * Math.Sin(phi)) / (1 + e * Math.Sin(phi))), e / 2);
            double t1 = Math.Tan((QPI) - phi1 / 2) / Math.Pow(((1 - e * phi1Sin) / (1 + e * phi1Sin)), e / 2);
            double t2 = Math.Tan((QPI) - phi2 / 2) / Math.Pow(((1 - e * phi2Sin) / (1 + e * phi2Sin)), e / 2);
            double tF = Math.Tan((QPI) - phiF / 2) / Math.Pow(((1 - e * phiFSin) / (1 + e * phiFSin)), e / 2);

            double n = (Math.Log(m1) - Math.Log(m2)) / (Math.Log(t1) - Math.Log(t2));
            double F = m1 / (n * Math.Pow(t1, n));

            double r = WGS84_A * F * Math.Pow(t, n);
            double rF = WGS84_A * F * Math.Pow(tF, n);

            double teta = n * (lambda - lambdaF);

            XLong = (TurkFalseEast + r * Math.Sin(teta));
            YLat = (TurkFalseNorth + rF - r * Math.Cos(teta));
        }

        public override void Flat2LL(ref double XLong, ref double YLat)
        {
            double e = Math.Pow((2 * WGS84_F - Math.Pow(WGS84_F, 2)), 0.5); //eccentricity

            double m1 = Math.Cos(phi1) / Math.Pow((1 - ((Math.Pow(e, 2)) * Math.Pow(Math.Sin(phi1), 2))), 0.5);
            double m2 = Math.Cos(phi2) / Math.Pow((1 - ((Math.Pow(e, 2)) * Math.Pow(Math.Sin(phi2), 2))), 0.5);

            double t1 = Math.Tan((Math.PI / 4) - phi1 / 2) / Math.Pow(((1 - e * Math.Sin(phi1)) / (1 + e * Math.Sin(phi1))), e / 2);
            double t2 = Math.Tan((Math.PI / 4) - phi2 / 2) / Math.Pow(((1 - e * Math.Sin(phi2)) / (1 + e * Math.Sin(phi2))), e / 2);
            double tF = Math.Tan((Math.PI / 4) - phiF / 2) / Math.Pow(((1 - e * Math.Sin(phiF)) / (1 + e * Math.Sin(phiF))), e / 2);

            double h1 = Math.Log(m1);
            double h2 = Math.Log(m2);
            double h3 = Math.Log(t1);
            double h4 = Math.Log(t2);

            double n = (h1 - h2) / (h3 - h4);

            double F = m1 / (n * Math.Pow(t1, n));

            double rF = WGS84_A * F * Math.Pow(tF, n);

            double rn = Math.Abs(Math.Pow((Math.Pow((XLong - TurkFalseEast), 2) + Math.Pow(rF - (YLat - TurkFalseNorth), 2)), 0.5));
            double tn = Math.Pow((rn / (WGS84_A * F)), 1 / n);
            double tetan = Math.Atan((XLong - TurkFalseEast) / (rF - (YLat - TurkFalseNorth)));

            double lambda = tetan / n + lambdaF;

            double phi = Math.PI / 2 - 2 * Math.Atan(tn);

            for (int i = 0; i < 10; i++)
            {
                phi = Math.PI / 2 - 2 * Math.Atan(tn * Math.Pow((1 - e * Math.Sin(phi)) / (1 + e * Math.Sin(phi)), e / 2));
            }

            YLat = phi * RAD2DEG;
            XLong = lambda * RAD2DEG;
        }
    }
}
