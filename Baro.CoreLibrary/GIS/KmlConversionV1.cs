using Baro.CoreLibrary.Core;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;

namespace Baro.CoreLibrary.GIS
{
    /// <summary>
    /// This class converts TAB file into KML
    /// </summary>
    public static class KmlConversionV1
    {
        public static ErrorCodes Tab2Kml(string tabFilename, string tabNameColumn, string kmlFilename)
        {
            // Open tab
            var table = Mitab.mitab_c_open(tabFilename);

            // table not found
            if (table == 0)
            {
                return ErrorCodes.TabFileNotFound;
            }

            // Tab columns list
            MitabColumns tabColums = new MitabColumns(table);

            // First record
            int id = Mitab.mitab_c_next_feature_id(table, -1);

            // Create KML Folder
            var folder = new Folder();
            folder.Id = "Nesneler";
            folder.Name = "Nesneler";

            // Until EOT
            while (id != -1)
            {
                // Read feature
                var feature = Mitab.mitab_c_read_feature(table, id);

                int objType = Mitab.mitab_c_get_type(feature);

                switch (objType)
                {
                    case Mitab.TABFC_FontPoint:
                    case Mitab.TABFC_CustomPoint:
                    case Mitab.TABFC_Point:
                        {
                            var fx = Mitab.mitab_c_get_vertex_x(feature, 0, 0);
                            var fy = Mitab.mitab_c_get_vertex_y(feature, 0, 0);

                            var placemark = new Placemark();
                            placemark.Name = Mitab.mitab_c_get_field_as_string_csharp(feature, tabColums.GetColumn(tabNameColumn).index);
                            placemark.Geometry = new Point() { Coordinate = new SharpKml.Base.Vector(fy, fx) };
                            folder.AddFeature(placemark);
                        }

                        break;

                    case Mitab.TABFC_Polyline:
                        {
                            var nodeCount = Mitab.mitab_c_get_vertex_count(feature, 0);

                            CoordinateCollection col = new CoordinateCollection();

                            for (int i = 0; i < nodeCount; i++)
                            {
                                col.Add(new Vector(Mitab.mitab_c_get_vertex_y(feature, 0, i), Mitab.mitab_c_get_vertex_x(feature, 0, i)));
                            }

                            LineString polygon = new LineString();
                            polygon.AltitudeMode = AltitudeMode.ClampToGround;
                            polygon.Coordinates = col;

                            Placemark placemark = new Placemark();
                            placemark.Name = Mitab.mitab_c_get_field_as_string_csharp(feature, tabColums.GetColumn(tabNameColumn).index);
                            placemark.Geometry = polygon;

                            folder.AddFeature(placemark);
                        }

                        break;

                    case Mitab.TABFC_Region:
                        {
                            var partCount = Mitab.mitab_c_get_parts(feature);

                            Polygon polygon = new Polygon();
                            polygon.AltitudeMode = AltitudeMode.ClampToGround;
                            polygon.Extrude = true;
                            
                            for (int part = 0; part < partCount; part++)
                            {
                                var nodeCount = Mitab.mitab_c_get_vertex_count(feature, part);

                                CoordinateCollection col = new CoordinateCollection();

                                for (int i = 0; i < nodeCount; i++)
                                {
                                    col.Add(new Vector(Mitab.mitab_c_get_vertex_y(feature, part, i), Mitab.mitab_c_get_vertex_x(feature, part, i)));
                                }


                                if (part == 0)
                                {
                                    OuterBoundary outerBoundary = new OuterBoundary();
                                    outerBoundary.LinearRing = new LinearRing();
                                    outerBoundary.LinearRing.Coordinates = col;

                                    polygon.OuterBoundary = outerBoundary;
                                }
                                else
                                {
                                    InnerBoundary inner = new InnerBoundary();
                                    inner.LinearRing = new LinearRing();
                                    inner.LinearRing.Coordinates = col;

                                    polygon.AddInnerBoundary(inner);
                                }
                            }

                            Placemark placemark = new Placemark();
                            placemark.Name = Mitab.mitab_c_get_field_as_string_csharp(feature, tabColums.GetColumn(tabNameColumn).index);
                            placemark.Geometry = polygon;

                            folder.AddFeature(placemark);
                        }

                        break;

                    default:
                        break;
                }

                // Destroy feature
                Mitab.mitab_c_destroy_feature(feature);

                // Next id
                id = Mitab.mitab_c_next_feature_id(table, id);
            }

            Mitab.mitab_c_close(table);

            // Create KML DOM
            var kml = new Kml();
            kml.Feature = folder;

            // Serialize the KML
            var serializer = new Serializer();
            serializer.Serialize(kml);

            try
            {
                System.IO.File.WriteAllText(kmlFilename, serializer.Xml, System.Text.Encoding.UTF8);
            }
            catch
            {
                return ErrorCodes.CantCreateKml;
            }

            return ErrorCodes.OK;
        }
    }
}
