using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.CoordSys
{
    public sealed class CoordSysOSGB1936 : CoordSys
    {        
        // OSGB1936
        // British Isles (Airy Ellipsoid)
        private const double F0 = 0.9996012717;     // Local scale factor on Central Meridian
        private const double A1 = 6377563.396 * F0; // Major Semi-axis, Airy Ellipsoid
        private const double B1 = 6356256.910 * F0; // Minor Semi-axis, Airy Ellipsoid
        private const double N0 = -100000;          // Grid Northing of True Origin (m)
        private const double E0 = 400000;           // Grid Easting of True Origin (m)
        private const double K0 = 49 * DEG2RAD;     // Lat of True Origin
        private const double Merid = -2;
        private const double L0 = Merid * DEG2RAD;  // Long of True Origin (2W)
        private const double N1 = (A1 - B1) / (A1 + B1); // n
        private const double N2 = N1 * N1;
        private const double N3 = N2 * N1;
        private const double E2 = ((A1 * A1) - (B1 * B1)) / (A1 * A1); // e^2

        /// <summary>
        /// British Grid System
        /// <para>
        /// Datum: OSGB1936,
        /// Map projection: Transverse Mercator,
        /// Latitude of Origin: 49,
        /// Longitude of Origin: -2,
        /// Scale Factor: 0.9996012717,
        /// False Easting: 400000 m,
        /// False Northing: -100000 m
        /// EPSG Code: EPSG:27700
        /// </para>
        /// </summary>
        /// <param name="LongX"></param>
        /// <param name="LatY"></param>
        private void LLtoFlat(ref double LongX, ref double LatY)
        {
            double K = LatY * DEG2RAD; 
            double L = LongX * DEG2RAD;
            double SINK = Math.Sin(K); 
            double COSK = Math.Cos(K); 
            double TANK = SINK / COSK; 
            double TANK2 = TANK * TANK;
            double COSK2 = COSK * COSK; 
            double COSK3 = COSK2 * COSK;
            double K3 = K - K0; 
            double K4 = K + K0;

            // ArcofMeridian
            double J3 = K3 * (1 + N1 + 1.25 * (N2 + N3));
            double J4 = Math.Sin(K3) * Math.Cos(K4) * (3 * (N1 + N2 + 0.875 * N3));
            double J5 = Math.Sin(2 * K3) * Math.Cos(2 * K4) * (1.875 * (N2 + N3));
            double J6 = Math.Sin(3 * K3) * Math.Cos(3 * K4) * 35 / 24 * N3;
            double M = (J3 - J4 + J5 - J6) * B1;

            // VRH2
            double Temp = 1 - E2 * SINK * SINK;
            double V = A1 / Math.Sqrt(Temp);
            double R = V * (1 - E2) / Temp;
            double H2 = V / R - 1.0;

            double P = L - L0; 
            double P2 = P * P; 
            double P4 = P2 * P2;

            J3 = M + N0;
            J4 = V / 2 * SINK * COSK;
            J5 = V / 24 * SINK * (COSK3) * (5 - (TANK2) + 9 * H2);
            J6 = V / 720 * SINK * COSK3 * COSK2 * (61 - 58 * (TANK2) + TANK2 * TANK2);
            // double North = J3 + P2 * J4 + P4 * J5 + P4 * P2 * J6;
            LatY = J3 + P2 * J4 + P4 * J5 + P4 * P2 * J6;

            double J7 = V * COSK;
            double J8 = V / 6 * COSK3 * (V / R - TANK2);
            double J9 = V / 120 * COSK3 * COSK2;
            J9 = J9 * (5 - 18 * TANK2 + TANK2 * TANK2 + 14 * H2 - 58 * TANK2 * H2);
            // double East = E0 + P * J7 + P2 * P * J8 + P4 * P * J9;
            LongX = E0 + P * J7 + P2 * P * J8 + P4 * P * J9;
        }

        public override void LL2Flat(ref double XLong, ref double YLat)
        {
            LLtoFlat(ref XLong, ref YLat);
        }

        public override void Flat2LL(ref double XLong, ref double YLat)
        {
            throw new NotImplementedException();
        }

        public override CoordSysType CoordSysType
        {
            get { return CoordSysType.OSGB; }
        }
    }
}
