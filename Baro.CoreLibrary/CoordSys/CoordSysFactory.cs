using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.CoordSys
{
    public class CoordSysException : Exception
    {
        public CoordSysException()
        {
        }
        public CoordSysException(string message)
            : base(message)
        {
        }
        public CoordSysException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// Dönüşüm için CoordSys nesnesinden yaratır.
    /// </summary>
    public static class CoordSysFactory
    {
        /// <summary>
        /// Dönüşüm için bir CoordSys nesnesi yaratır.
        /// </summary>
        /// <param name="type">Kullanmak istediğiniz projeksiyon sistemi</param>
        /// <returns>CoordSys nesnesi</returns>
        public static CoordSys CreateCoordSys(CoordSysType type)
        {
            switch (type)
            {
                case CoordSysType.Mercator:
                    return new CoordSysMercator();

                case CoordSysType.Lambert:
                    return new CoordSysLambert();

                case CoordSysType.OSGB:
                    return new CoordSysOSGB1936();

                case CoordSysType.Google:
                    return new CoordSysGoogle();

                default:
                    throw new CoordSysException("Invalid coordsys. internal error");
            }
        }
    }
}
