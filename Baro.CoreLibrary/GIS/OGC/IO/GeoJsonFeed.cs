
//http://bingmapsv7modules.codeplex.com/SourceControl/latest#BMv7Plugins/BMv7.GeoJSON/scripts/GeoJSONModule.js
//http://geojson.org/geojson-spec.html

//Uses JSON.Net library: http://www.nuget.org/packages/Newtonsoft.Json

using Baro.CoreLibrary.GIS.OGC.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Baro.CoreLibrary.GIS.OGC.IO
{
    /// <summary>
    /// A class for reading and writing GeoJSON objects.
    /// </summary>
    public class GeoJsonFeed : BaseTextFeed
    {
        //TODO Add support for styles

        #region Constructor

        public GeoJsonFeed()
        {
        }

        public GeoJsonFeed(bool stripHtml)
            : base(stripHtml)
        {
        }

        public GeoJsonFeed(double tolerance)
            : base(tolerance)
        {            
        }

        public GeoJsonFeed(bool stripHtml, double tolerance)
            : base(stripHtml, tolerance)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads a GeoJSON feed
        /// </summary>
        /// <param name="feed">GeoJSON feed as a string.</param>
        /// <returns>A SpatialDataSet containing the spatial data from the GeoJSON feed.</returns>
        public override Task<SpatialDataSet> ReadAsync(string feed)
        {
            return Task.Run<SpatialDataSet>(async () =>
            {
                try
                {
                    using (var reader = new StringReader(feed))
                    {
                        var geoms = await ParseGeoJSON(reader);
                        return new SpatialDataSet()
                        {
                            Geometries = geoms,
                            BoundingBox = geoms.Envelope()
                        };
                    }
                }
                catch { }

                return null;
            });
        }

        /// <summary>
        /// Reads a GPX feed.
        /// </summary>
        /// <param name="xmlUri">Uri to GPX feed.</param>
        /// <returns>A SpatialDataSet containing the spatial data from the GPX feed.</returns>
        public override Task<SpatialDataSet> ReadAsync(Uri feedUri)
        {
            return Task.Run<SpatialDataSet>(async () =>
            {
                try
                {
                    var json = await ServiceHelper.GetStreamAsync(feedUri);
                    using (var reader = new StreamReader(json))
                    {
                        var geoms = await ParseGeoJSON(reader);

                        return new SpatialDataSet()
                        {
                            Geometries = geoms,
                            BoundingBox = geoms.Envelope()
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new SpatialDataSet()
                    {
                        Error = ex.Message
                    };
                }
            });
        }

        /// <summary>
        /// Reads a GeoJSON feed
        /// </summary>
        /// <param name="feedStream">GeoJSON feed as a Stream.</param>
        /// <returns>A SpatialDataSet containing the spatial data from the GeoJSON feed.</returns>
        public override Task<SpatialDataSet> ReadAsync(Stream feedStream)
        {
            return Task.Run<SpatialDataSet>(async () =>
            {
                try
                {
                    using (var reader = new StreamReader(feedStream))
                    {
                        var g = await ParseGeoJSON(reader);

                        return new SpatialDataSet()
                        {
                            Geometries = g,
                            BoundingBox = g.Envelope()
                        };
                    }
                }
                catch { }

                return null;
            });
        }

        /// <summary>
        /// Writes a list of geometries to a string.
        /// </summary>
        /// <param name="data">Data set to write</param>
        /// <returns>A GeoJSON string of the geometries</returns>
        public static Task<string> WriteAsync(Geometry geometry)
        {
            return Task.Run<string>(() =>
            {
                var geoms = new List<Geometry>();
                geoms.Add(geometry);
               // return WriteGeometries(geoms);

                return string.Empty;
            });
        }

        /// <summary>
        /// Writes a list of geometries to a string.
        /// </summary>
        /// <param name="data">Data set to write</param>
        /// <returns>A GeoJSON string of the geometries</returns>
        public override Task<string> WriteAsync(SpatialDataSet data)
        {
            return Task.Run<string>(() =>
            {
               // return Write(data);

                return string.Empty;
            });
        }

        /// <summary>
        /// Writes a spatial data set object as a string.
        /// </summary>
        /// <param name="data">Data set to write</param>
        /// <returns>A GeoJSON string</returns>
        public override Task WriteAsync(SpatialDataSet data, Stream stream)
        {
            return Task.Run(() =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    //var sGeoms = Write(data);
                    //writer.Write(sGeoms);
                }
            });
        }

        #endregion

        #region Private Methods

        #region GeoJSON Read Methods

        private async Task<List<Geometry>> ParseGeoJSON(TextReader geoJSON)
        {
            List<Geometry> geoms = null;

            {
                var root = BsonSerializer.Deserialize<BsonDocument>(geoJSON);

                string type = root["type"].AsString;
                
                switch (type)
                {
                    case "FeatureCollection":
                        geoms = await ParseFeatureCollection(root);
                        break;
                    case "Feature":
                        var geom = await ParseFeature(root);
                        if (geom != null)
                        {
                            geoms = new List<Geometry>();
                            geoms.Add(geom);
                        }
                        break;
                    case "Point":
                    case "LineString":
                    case "Polygon":
                    case "MultiPoint":
                    case "MultiLineString":
                    case "MultiPolygon":
                    case "GeometryCollection":
                        var g = await ParseGeometry(root);
                        if (g != null)
                        {
                            geoms = new List<Geometry>();
                            geoms.Add(g);
                        }
                        break;
                    default:
                        break;
                }
            }

            return geoms;
        }

        private async Task<Geometry> ParseGeometry(BsonDocument json)
        {
            var type = json["type"].AsString;

            switch (type)
            {
                case "Point":
                    return ParsePoint(json);
                case "LineString":
                    return await ParseLineString(json);
                case "Polygon":
                    return await ParsePolygon(json);
                case "MultiPoint":
                    return await ParseMultiPoint(json);
                case "MultiLineString":
                    return await ParseMultiLineString(json);
                case "MultiPolygon":
                    return await ParseMultiPolygon(json);
                case "GeometryCollection":
                    return await ParseGeometryCollection(json);
                default:
                    break;
            }

            return null;
        }

        private Coordinate? ParseCoordinate(BsonArray jsonArray)
        {
            if (jsonArray.Count >= 3)
            {
                return new Coordinate(jsonArray[1].AsDouble, jsonArray[0].AsDouble, jsonArray[2].AsDouble);
            }
            else if (jsonArray.Count >= 2)
            {
                return new Coordinate(jsonArray[1].AsDouble, jsonArray[0].AsDouble);
            }

            return null;
        }

        private async Task<CoordinateCollection> ParseCoordinates(BsonArray jsonArray)
        {
            var coords = new CoordinateCollection();

            foreach (BsonArray c in jsonArray)
            {
                var coord = ParseCoordinate(c);
                if (coord.HasValue)
                {
                    coords.Add(coord.Value);
                }
            }

            if (optimize)
            {
                coords = await SpatialTools.VertexReductionAsync(coords, tolerance);
            }

            return coords;
        }

        private Geometry ParsePoint(BsonDocument json)
        {
            var coords = ParseCoordinate(json["coordinates"].AsBsonArray);
            if (coords.HasValue)
            {
                return new Point(coords.Value);
            }

            return null;
        }

        private async Task<Geometry> ParseLineString(BsonDocument json)
        {
            var coords = await ParseCoordinates(json["coordinates"].AsBsonArray);
            if (coords != null && coords.Count >= 2)
            {
                return new LineString(coords);
            }

            return null;
        }

        private async Task<Geometry> ParsePolygon(BsonDocument json)
        {
            var rings = json["coordinates"].AsBsonArray;

            if (rings.Count > 0)
            {
                var coords = await ParseCoordinates(rings[0].AsBsonArray);
                var polygon = new Polygon(coords);

                for (int i = 1; i < rings.Count; i++)
                {
                    coords = await ParseCoordinates(rings[i].AsBsonArray);
                    if (coords != null && coords.Count > 0)
                    {
                        polygon.InteriorRings.Add(coords);
                    }
                }

                return polygon;
            }

            return null;
        }

        private async Task<Geometry> ParseMultiPoint(BsonDocument json)
        {
            var coords = await ParseCoordinates(json["coordinates"].AsBsonArray);
            if (coords != null && coords.Count > 0)
            {
                var mp = new MultiPoint(coords.Count);
                foreach (var c in coords)
                {
                    mp.Geometries.Add(new Point(c));
                }
                return mp;
            }

            return null;
        }

        private async Task<Geometry> ParseMultiLineString(BsonDocument json)
        {
            var lines = json["coordinates"].AsBsonArray;

            var mp = new MultiLineString();

            foreach (BsonArray l in lines)
            {
                var coords = await ParseCoordinates(l);
                if (coords != null && coords.Count > 0)
                {
                    mp.Geometries.Add(new LineString(coords));
                }
            }

            if (mp.Geometries.Count > 0)
            {
                return mp;
            }

            return null;
        }

        private async Task<Geometry> ParseMultiPolygon(BsonDocument json)
        {
            var polygons = json["coordinates"].AsBsonArray;

            var mp = new MultiPolygon();

            foreach (BsonArray p in polygons)
            {
                var rings = p;
                if (rings.Count > 0)
                {
                    var coords = await ParseCoordinates(rings[0].AsBsonArray);
                    var polygon = new Polygon(coords);

                    for (int i = 1; i < rings.Count; i++)
                    {
                        coords = await ParseCoordinates(rings[i].AsBsonArray);
                        if (coords != null && coords.Count > 0)
                        {
                            polygon.InteriorRings.Add(coords);
                        }
                    }
                    mp.Geometries.Add(polygon);
                }
            }

            if (mp.Geometries.Count > 0)
            {
                return mp;
            }

            return null;
        }

        private async Task<Geometry> ParseGeometryCollection(BsonDocument json)
        {
            var jsonGeom = json["geometries"].AsBsonArray;

            if (jsonGeom != null)
            {
                var gc = new GeometryCollection();

                foreach (BsonDocument g in jsonGeom)
                {
                    var geom = await ParseGeometry(g);
                    if (geom != null)
                    {
                        gc.Geometries.Add(geom);
                    }
                }

                if (gc.Geometries.Count > 0)
                {
                    return gc;
                }
            }           

            return null;
        }

        private async Task<List<Geometry>> ParseFeatureCollection(BsonDocument json)
        {
            var geoms = new List<Geometry>();
            var features = json["features"].AsBsonArray;

            foreach (BsonDocument f in features)
            {
                var geom = await ParseFeature(f);
                if (geom != null)
                {
                    geoms.Add(geom);
                }
            }

            return geoms;
        }

        private async Task<Geometry> ParseFeature(BsonDocument json)
        {
            var jsonGeom = json["geometry"].AsBsonDocument;
            var geom = await ParseGeometry(jsonGeom);

            if (geom != null)
            {
                geom.Metadata = ParseProperties(json);

                if (geom.Metadata.Properties.ContainsKey("id"))
                {
                    geom.Metadata.ID = geom.Metadata.Properties["id"] as string;
                }

                return geom;
            }

            return null;
        }

        private ShapeMetadata ParseProperties(BsonDocument json)
        {
            var metadata = new ShapeMetadata();

            var propJson = json["properties"].AsBsonDocument;
            var idJson = json["id"].AsString;

            if (!string.IsNullOrWhiteSpace(idJson))
            {
                metadata.Properties.Add("id", idJson);
            }

            if (propJson != null)
            {
                foreach (BsonElement child in propJson.Elements)
                {
                    var p = child;

                    try
                    {
                        switch (p.Value.BsonType)
                        {
                            case BsonType.String:
                                metadata.Properties.Add(p.Name, p.Value.AsString);
                                break;
                            case BsonType.Double:
                                metadata.Properties.Add(p.Name, p.Value.AsDouble);
                                break;
                            case BsonType.Int32:
                            case BsonType.Int64:
                                metadata.Properties.Add(p.Name, p.Value.AsInt64);
                                break;
                            case BsonType.Boolean:
                                metadata.Properties.Add(p.Name, p.Value.AsBoolean);
                                break;
                            default:
                                break;
                        }
                    }
                    catch { }
                }
            }

            if (metadata.Properties.Count > 0)
            {
                return metadata;
            }

            return null;
        }

        #endregion

        #region GeoJSON Write Methods

        private static string Write(SpatialDataSet data)
        {
            return WriteGeometries(data.Geometries);
        }

        private static string WriteGeometries(List<Geometry> geometries)
        {
            if (geometries != null)
            {
                BsonDocument root;

                if (geometries.Count == 1 && (geometries[0].Metadata == null || geometries[0].Metadata.Properties.Count == 0))
                {
                    root = CreateGeometry(geometries[0]);
                }
                else
                {
                    root = new BsonDocument();
                    root.Add("type", "FeatureCollection");
                    root.Add("features", CreateFeatures(geometries));
                }

                return root.ToString();
            }

            return string.Empty;
        }

        private static BsonArray CreateFeatures(List<Geometry> geometries)
        {
            var features = new BsonArray();

            foreach (var g in geometries)
            {
                features.Add(CreateFeature(g));
            }

            return features;
        }

        private static BsonDocument CreateFeature(Geometry geometry)
        {
            var feature = new BsonDocument();

            feature.Add("type", "Feature");
            feature.Add("geometry", CreateGeometry(geometry));

            if (geometry.Metadata != null && geometry.Metadata.Properties.Count > 0)
            {
                feature.Add("properties", CreateProperties(geometry.Metadata));
            }

            return feature;
        }

        private static BsonDocument CreateProperties(ShapeMetadata metadata)
        {
            var properties = new BsonDocument();
            bool tempBool;
            double tempDouble;

            if (!string.IsNullOrEmpty(metadata.Title))
            {
                properties.Add("title", metadata.Title);
            }

            if (!string.IsNullOrEmpty(metadata.Description))
            {
                properties.Add("description", metadata.Description);
            }

            foreach (var t in metadata.Properties)
            {
                object o = t.Value;
                if (o is string)
                {
                    properties.Add(t.Key, o.ToString());
                }
                else if (o is bool && bool.TryParse(o.ToString(), out tempBool))
                {
                    properties.Add(t.Key, tempBool);
                }
                else if (double.TryParse(o.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out tempDouble))
                {
                    properties.Add(t.Key, tempDouble);
                }
            }

            return properties;
        }

        private static BsonDocument CreateGeometry(Geometry geometry)
        {
            if (geometry is Point)
            {
                return CreatePoint(geometry as Point);
            }
            else if (geometry is LineString)
            {
                return CreateLineString(geometry as LineString);
            }
            else if (geometry is Polygon)
            {
                return CreatePolygon(geometry as Polygon);
            }
            else if (geometry is MultiPoint)
            {
                return CreateMultiPoint(geometry as MultiPoint);
            }
            else if (geometry is MultiLineString)
            {
                return CreateMultiLineString(geometry as MultiLineString);
            }
            else if (geometry is MultiPolygon)
            {
                return CreateMultiPolygon(geometry as MultiPolygon);
            }
            else if (geometry is GeometryCollection)
            {
                return CreateGeometryCollection(geometry as GeometryCollection);
            }

            return null;
        }

        private static BsonArray CreateCoordinate(Coordinate coordinate)
        {
            var coord = new BsonArray();

            coord.Add(coordinate.Longitude);
            coord.Add(coordinate.Latitude);

            if (coordinate.Altitude.HasValue)
            {
                coord.Add(coordinate.Altitude.Value);
            }

            return coord;
        }

        private static BsonArray CreateCoordinates(CoordinateCollection coordinates)
        {
            var coords = new BsonArray();

            foreach (var c in coordinates)
            {
                coords.Add(CreateCoordinate(c));
            }

            return coords;
        }

        private static BsonArray CreateRings(List<CoordinateCollection> rings)
        {
            var coords = new BsonArray();

            foreach (var r in rings)
            {
                coords.Add(CreateCoordinates(r));
            }

            return coords;
        }

        private static BsonDocument CreatePoint(Point point)
        {
            var json = new BsonDocument();

            json.Add("type", "Feature");
            json.Add("coordinates", CreateCoordinate(point.Coordinate));

            return json;
        }

        private static BsonDocument CreateLineString(LineString line)
        {
            var json = new BsonDocument();

            json.Add("type", "LineString");
            json.Add("coordinates", CreateCoordinates(line.Vertices));

            return json;
        }

        private static BsonDocument CreatePolygon(Polygon polygon)
        {
            var json = new BsonDocument();

            json.Add("type", "Polygon");

            var exRing = CreateCoordinates(polygon.ExteriorRing);
            var inRings = CreateRings(polygon.InteriorRings);

            inRings.Insert(0, exRing);

            json.Add("coordinates", inRings);

            return json;
        }

        private static BsonDocument CreateMultiPoint(MultiPoint points)
        {
            var json = new BsonDocument();

            json.Add("type", "MultiPoint");

            var coords = new CoordinateCollection();
            foreach (var p in points)
            {
                coords.Add(p.Coordinate);
            }

            json.Add("coordinates", CreateCoordinates(coords));

            return json;
        }

        private static BsonDocument CreateMultiLineString(MultiLineString lines)
        {
            var json = new BsonDocument();

            json.Add("type", "MultiLineString");

            var coords = new List<CoordinateCollection>();
            foreach (var l in lines)
            {
                coords.Add(l.Vertices);
            }

            json.Add("coordinates", CreateRings(coords));

            return json;
        }

        private static BsonDocument CreateMultiPolygon(MultiPolygon polygons)
        {
            var json = new BsonDocument();

            json.Add("type", "MultiPolygon");

            var coords = new BsonArray();

            foreach (var p in polygons)
            {
                var exRing = CreateCoordinates(p.ExteriorRing);
                var inRings = CreateRings(p.InteriorRings);

                inRings.Insert(0, exRing);

                coords.Add(inRings);
            }

            json.Add("coordinates", coords);

            return json;
        }

        private static BsonDocument CreateGeometryCollection(GeometryCollection geometries)
        {
            var json = new BsonDocument();

            json.Add("type", "GeometryCollection");

            var geoms = new BsonArray();

            foreach (var g in geometries)
            {
                geoms.Add(CreateGeometry(g));
            }

            json.Add("geometries", geoms);

            return json;
        }

        #endregion

        #endregion
    }
}
