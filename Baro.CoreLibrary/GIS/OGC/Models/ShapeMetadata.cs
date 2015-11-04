using System.Collections.Generic;
using MongoDB.Bson;

namespace Baro.CoreLibrary.GIS.OGC.Models
{
    /// <summary>
    /// A class used to store metadata of a spatial object.
    /// </summary>
    public class ShapeMetadata
    {
        #region Private Properties

        private Dictionary<string, object> _properties;

        #endregion

        #region Constructor

        /// <summary>
        /// A class used to store metadata of a spatial object.
        /// </summary>
        public ShapeMetadata()
        {
            _properties = new Dictionary<string, object>();
            //ID = string.Empty;
            //Title = string.Empty;
            //Description = string.Empty;
        }

        #endregion

        #region Public Properties

        ///// <summary>
        ///// A unique identifier for the Geometry
        ///// </summary>
        //public string ID { get; set; }

        ///// <summary>
        ///// Main title or name.
        ///// </summary>
        //public string Title { get; set; }

        ///// <summary>
        ///// Main descriotion.
        ///// </summary>
        //public string Description { get; set; }

        /// <summary>
        /// A dictionary of all additional metadata objects by name.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get
            {
                return _properties;
            }
        }

        /// <summary>
        /// This method converts only properties
        /// </summary>
        /// <returns></returns>
        public BsonDocument ToGeoJsonData()
        {
            return _properties.ToBsonDocument();
        }
        #endregion
    }
}
