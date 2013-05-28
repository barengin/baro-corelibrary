using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.CoordSys
{
    /// <summary>
    /// Kullanılabilecek koordinat sistemleri
    /// </summary>
    public enum CoordSysType
    {
        /// <summary>
        /// Undefined
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Mercator
        /// </summary>
        Mercator = 1,
        /// <summary>
        /// Turkish Lambert WGS84
        /// </summary>
        Lambert = 2,
        /// <summary>
        /// British National Grid EPSG: 27700
        /// </summary>
        OSGB = 3,
        /// <summary>
        /// Google Spherical Mercator
        /// </summary>
        Google = 4
    }

    /// <summary>
    /// Düzlemden enlem-boylam'a veya tam tersi şekilde dönüşüm
    /// yapabilecek sanal sınıf. Bu sınıfı yaratmak için <see cref="CoordSysFactory">CoordSysFactory</see>'yi
    /// kullanın.
    /// </summary>
    public abstract class CoordSys
    {
        public const double DEG2RAD = Math.PI / 180.0;
        public const double RAD2DEG = 180.0 / Math.PI;

        /// <summary>
        /// Long-Lat -> Flat projeksiyon dönüşüm methodu
        /// </summary>
        /// <param name="XLong">X veya Longitude</param>
        /// <param name="YLat">Y veya Latitude</param>
        public abstract void LL2Flat(ref double XLong, ref double YLat);

        /// <summary>
        /// Flat -> Long-Lat projeksiyon dönüşümü
        /// </summary>
        /// <param name="XLong">X veya Longitude</param>
        /// <param name="YLat">Y veya Latitude</param>
        public abstract void Flat2LL(ref double XLong, ref double YLat);

        /// <summary>
        /// Dönüşüm için kullanılan projeksiyon sistemi
        /// </summary>
        public abstract CoordSysType CoordSysType { get; }
    }
}
