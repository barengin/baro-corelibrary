using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.CoordSys
{
    internal sealed class CoordSysGoogle : CoordSys
    {
        private const double radius = 6378137d;

        public override void LL2Flat(ref double XLong, ref double YLat)
        {
            var longitude = XLong * DEG2RAD;
            XLong = (radius * longitude);

            var latitude = DEG2RAD * YLat;
            YLat = radius / 2.0d * Math.Log((1.0d + Math.Sin(latitude)) / (1.0d - Math.Sin(latitude)));
        }

        public override void Flat2LL(ref double XLong, ref double YLat)
        {
            var longRadians = XLong / radius;
            var longDegrees = RAD2DEG * longRadians;

            /* The user could have panned around the world a lot of times.
            Lat long goes from -180 to 180.  So every time a user gets 
            to 181 we want to subtract 360 degrees.  Every time a user
            gets to -181 we want to add 360 degrees. */

            var rotations = Math.Floor((longDegrees + 180d) / 360d);
            XLong = longDegrees - (rotations * 360);

            YLat = RAD2DEG * ((Math.PI / 2d) - (2d * Math.Atan(Math.Exp(-1.0d * YLat / radius))));
        }

        public override CoordSysType CoordSysType
        {
            get { return CoordSysType.Google; }
        }
    }
}
