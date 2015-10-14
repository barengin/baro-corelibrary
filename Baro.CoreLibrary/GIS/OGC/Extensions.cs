using Baro.CoreLibrary.GIS.OGC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.GIS.OGC
{
    public static class Extensions
    {
        /// <summary>
        /// Calculates the Envelope (BoundingBox) of a list of geometries.
        /// </summary>
        /// <param name="geometries">List of geometries to calculate envelope for.</param>
        /// <returns>Envolpe the contains all geometries.</returns>
        public static BoundingBox Envelope(this List<Geometry> geometries)
        {
            BoundingBox bounds = null;
            foreach (var g in geometries)
            {
                var b = g.Envelope();

                if (bounds == null)
                {
                    bounds = b;
                }
                else
                {
                    bounds = bounds.Join(b);
                }
            }

            return bounds;
        }
    }
}
