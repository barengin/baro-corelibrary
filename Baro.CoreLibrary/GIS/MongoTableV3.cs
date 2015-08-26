using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.GIS
{
    public sealed class MongoTableV3 : IDisposable
    {
        private const string CLASS_TAG = "gtLib2.Db.MongoTableV2";

        private bool disposed = false;

        private MongoClient _mongoClient;
        private MongoServer _mongoServer;
        private MongoDatabase _mongoDatabase;

        private void CheckDisposed()
        {
            if (disposed) throw new ObjectDisposedException(CLASS_TAG);
        }

        public MongoTableV3(string cfg)
        {
            string[] parameters = cfg.Split('|');

            _mongoClient = new MongoClient(parameters[0]);
            _mongoServer = _mongoClient.GetServer();
            _mongoDatabase = _mongoServer.GetDatabase(parameters[1]);
        }

        public int UpdateRecord(string collectionName, string tabFilename)
        {
            var mapCatalog = _mongoDatabase.GetCollection<BsonDocument>("MAPCATALOG");

            if (!mapCatalog.Exists())
            {
                return (int)ErrorCodes.NoMapCatalogExists;
            }

            // The map
            var collection = _mongoDatabase.GetCollection<BsonDocument>(collectionName);

            if (!collection.Exists())
            {
                return (int)ErrorCodes.CollectionNotFound;
            }

            // MapCatalog Definition
            var collectionDef = mapCatalog.FindOne(Query.EQ("name", collectionName));

            if (collectionDef == null)
            {
                return (int)ErrorCodes.TableNotRegistered;
            }

            // Columns
            var columns = collectionDef["columns"].AsBsonArray;

            // Open tab
            var table = Mitab.mitab_c_open(tabFilename);

            if (table == 0)
            {
                return (int)ErrorCodes.TabFileNotFound;
            }

            MitabColumns miColumns = new MitabColumns(table);

            try
            {
                miColumns.GetColumn("DB_PRINX");
            }
            catch
            {
                Mitab.mitab_c_close(table);
                return (int)ErrorCodes.DbPrinxNotFound;
            }

            // First record
            int id = Mitab.mitab_c_next_feature_id(table, -1);

            // if EOT
            if (id == -1)
            {
                Mitab.mitab_c_close(table);
                return (int)ErrorCodes.TableIsEmpty;
            }

            // Read feature
            var feature = Mitab.mitab_c_read_feature(table, id);
            int featureType = Mitab.mitab_c_get_type(feature);

            // Objects: Point & Region
            var points = new BsonArray();

            // Collect points if featureType is Point or Region
            if ((featureType == Mitab.TABFC_Point) ||
                (featureType == Mitab.TABFC_CustomPoint) ||
                (featureType == Mitab.TABFC_FontPoint) ||
                (featureType == Mitab.TABFC_Region))
            {
                var partCount = Mitab.mitab_c_get_parts(feature);

                for (int part = 0; part < partCount; part++)
                {
                    var vertexCount = Mitab.mitab_c_get_vertex_count(feature, part);

                    BsonArray p = new BsonArray();

                    for (int i = 0; i < vertexCount; i++)
                    {
                        var fx = Mitab.mitab_c_get_vertex_x(feature, part, i);
                        var fy = Mitab.mitab_c_get_vertex_y(feature, part, i);

                        p.Add(fx);
                        p.Add(fy);
                    }

                    points.Add(p);
                }
            }

            //// Collect points if featureType is Point or Region
            //if ((featureType == Mitab.TABFC_Point) ||
            //    (featureType == Mitab.TABFC_CustomPoint) ||
            //    (featureType == Mitab.TABFC_FontPoint) ||
            //    (featureType == Mitab.TABFC_Region))
            //{
            //    var vertexCount = Mitab.mitab_c_get_vertex_count(feature, 0);

            //    for (int i = 0; i < vertexCount; i++)
            //    {
            //        var fx = Mitab.mitab_c_get_vertex_x(feature, 0, i);
            //        var fy = Mitab.mitab_c_get_vertex_y(feature, 0, i);

            //        points.Add(fx);
            //        points.Add(fy);
            //    }
            //}

            // Create Map Record
            var obj = new BsonDocument()
                {
                    { "Obj", new BsonDocument()
                        {
                            { "type", featureType },
                            { "coordinates", points }
                        }
                    }
                };

            // Add _id field to OBJ
            {
                var oid = Mitab.mitab_c_get_field_as_string_csharp(feature, miColumns.GetColumn("DB_PRINX").index);
                obj.Add(new BsonElement("_id", new BsonObjectId(ObjectId.Parse(oid))));
            }
            
            // Columns from MapCatalog
            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i].AsBsonArray;

                var columnName = column[0].AsString;
                var columnType = column[1].AsInt32;

                switch (columnType)
                {
                    case Mitab.TABFT_Char:
                        obj.Add(new BsonElement(columnName, Mitab.mitab_c_get_field_as_string_csharp(feature, miColumns.GetColumn(columnName).index)));
                        break;

                    case Mitab.TABFT_SmallInt:
                    case Mitab.TABFT_Integer:
                        obj.Add(new BsonElement(columnName, (int)Mitab.mitab_c_get_field_as_double(feature, miColumns.GetColumn(columnName).index)));
                        break;

                    case Mitab.TABFT_Decimal:
                    case Mitab.TABFT_Float:
                        obj.Add(new BsonElement(columnName, Mitab.mitab_c_get_field_as_double(feature, miColumns.GetColumn(columnName).index)));
                        break;
                }
            }

            Mitab.mitab_c_destroy_feature(feature);
            Mitab.mitab_c_close(table);

            collection.Save(obj);

            return (int)ErrorCodes.OK;
        }
        
        public int InsertRecord(string collectionName, string tabFilename)
        {
            var mapCatalog = _mongoDatabase.GetCollection<BsonDocument>("MAPCATALOG");

            if (!mapCatalog.Exists())
            {
                return (int)ErrorCodes.NoMapCatalogExists;
            }

            // The map
            var collection = _mongoDatabase.GetCollection<BsonDocument>(collectionName);

            if (!collection.Exists())
            {
                return (int)ErrorCodes.CollectionNotFound;
            }

            // MapCatalog Definition
            var collectionDef = mapCatalog.FindOne(Query.EQ("name", collectionName));

            if (collectionDef == null)
            {
                return (int)ErrorCodes.TableNotRegistered;
            }

            // Columns
            var columns = collectionDef["columns"].AsBsonArray;

            // Open tab
            var table = Mitab.mitab_c_open(tabFilename);

            if (table == 0)
            {
                return (int)ErrorCodes.TabFileNotFound;
            }

            MitabColumns miColumns = new MitabColumns(table);

            // First record
            int id = Mitab.mitab_c_next_feature_id(table, -1);

            // if EOT
            if (id == -1)
            {
                Mitab.mitab_c_close(table);
                return (int)ErrorCodes.TableIsEmpty;
            }

            // Read feature
            var feature = Mitab.mitab_c_read_feature(table, id);
            int featureType = Mitab.mitab_c_get_type(feature);

            // Objects: Point & Region
            var points = new BsonArray();

            //// Collect points if featureType is Point or Region
            //if ((featureType == Mitab.TABFC_Point) ||
            //    (featureType == Mitab.TABFC_CustomPoint) ||
            //    (featureType == Mitab.TABFC_FontPoint) ||
            //    (featureType == Mitab.TABFC_Region))
            //{
            //    var vertexCount = Mitab.mitab_c_get_vertex_count(feature, 0);

            //    for (int i = 0; i < vertexCount; i++)
            //    {
            //        var fx = Mitab.mitab_c_get_vertex_x(feature, 0, i);
            //        var fy = Mitab.mitab_c_get_vertex_y(feature, 0, i);

            //        points.Add(fx);
            //        points.Add(fy);
            //    }
            //}

            // Collect points if featureType is Point or Region
            if ((featureType == Mitab.TABFC_Point) ||
                (featureType == Mitab.TABFC_CustomPoint) ||
                (featureType == Mitab.TABFC_FontPoint) ||
                (featureType == Mitab.TABFC_Region))
            {
                var partCount = Mitab.mitab_c_get_parts(feature);

                for (int part = 0; part < partCount; part++)
                {
                    var vertexCount = Mitab.mitab_c_get_vertex_count(feature, part);

                    BsonArray p = new BsonArray();

                    for (int i = 0; i < vertexCount; i++)
                    {
                        var fx = Mitab.mitab_c_get_vertex_x(feature, part, i);
                        var fy = Mitab.mitab_c_get_vertex_y(feature, part, i);

                        p.Add(fx);
                        p.Add(fy);
                    }

                    points.Add(p);
                }
            }

            // Create Map Record
            var obj = new BsonDocument()
                {
                    { "Obj", new BsonDocument()
                        {
                            { "type", featureType },
                            { "coordinates", points }
                        }
                    }
                };

            {
                if (miColumns.ColumnExists("DB_PRINX"))
                {
                    var v = Mitab.mitab_c_get_field_as_string_csharp(feature, miColumns.GetColumn("DB_PRINX").index);
                    obj.Add(new BsonElement("_id", new BsonObjectId(new ObjectId(v))));
                }
                else
                {
                    obj.Add(new BsonElement("_id", new BsonObjectId(ObjectId.GenerateNewId())));
                }
            }

            // Columns
            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i].AsBsonArray;

                var columnName = column[0].AsString;
                var columnType = column[1].AsInt32;

                switch (columnType)
                {
                    case Mitab.TABFT_Char:
                        obj.Add(new BsonElement(columnName, Mitab.mitab_c_get_field_as_string_csharp(feature, miColumns.GetColumn(columnName).index)));
                        break;

                    case Mitab.TABFT_SmallInt:
                    case Mitab.TABFT_Integer:
                        obj.Add(new BsonElement(columnName, (int)Mitab.mitab_c_get_field_as_double(feature, miColumns.GetColumn(columnName).index)));
                        break;

                    case Mitab.TABFT_Decimal:
                    case Mitab.TABFT_Float:
                        obj.Add(new BsonElement(columnName, Mitab.mitab_c_get_field_as_double(feature, miColumns.GetColumn(columnName).index)));
                        break;
                }
            }

            Mitab.mitab_c_destroy_feature(feature);
            Mitab.mitab_c_close(table);

            // TODO: Burada duplicate ID durumu için exception yaz !!!!
            collection.Insert(obj);

            return (int)ErrorCodes.OK;
        }
            
        public int DeleteRecord(string collectionName, string objectID)
        {
            // Map Collection
            var map = _mongoDatabase.GetCollection<BsonDocument>(collectionName);

            if (!map.Exists())
            {
                return (int)ErrorCodes.CollectionNotFound;
            }

            map.Remove(Query.EQ("_id", new ObjectId(objectID)), RemoveFlags.Single);

            return (int)ErrorCodes.OK;
        }
            
        public int UploadTable(string tabFilename, string collectionName)
        {
            // Open tab
            var table = Mitab.mitab_c_open(tabFilename);

            if (table == 0)
            {
                return (int)ErrorCodes.TabFileNotFound;
            }

            // Field count
            var fieldCount = Mitab.mitab_c_get_field_count(table);

            // MAPCATALOG Record
            var columns = new BsonArray();

            var catalogRecord = new BsonDocument()
            {
                { "name", collectionName },
                { "columns", columns }
            };

            // TAB Columns
            for (int i = 0; i < fieldCount; i++)
            {
                var fieldName = Mitab.mitab_c_get_field_name_csharp(table, i);
                var fieldType = Mitab.mitab_c_get_field_type(table, i);

                // ignore DB_PRINX
                if (string.Compare(fieldName, "DB_PRINX", true) == 0)
                {
                    continue;
                }

                var column = new BsonArray();

                switch (fieldType)
                {
                    case Mitab.TABFT_Char:
                        var fieldWidth = Mitab.mitab_c_get_field_width(table, i);

                        column.Add(fieldName);
                        column.Add(Mitab.TABFT_Char);
                        column.Add(fieldWidth);
                        columns.Add(column);
                        break;

                    case Mitab.TABFT_Decimal:
                    case Mitab.TABFT_Float:

                        column.Add(fieldName);
                        column.Add(Mitab.TABFT_Float);
                        columns.Add(column);
                        break;

                    case Mitab.TABFT_Integer:
                    case Mitab.TABFT_SmallInt:

                        column.Add(fieldName);
                        column.Add(Mitab.TABFT_Integer);
                        columns.Add(column);
                        break;

                    default:
                        break;
                }
            }

            // Update MAPCATALOG
            var mapCatalog = _mongoDatabase.GetCollection<BsonDocument>("MAPCATALOG");
            mapCatalog.Update(Query.EQ("name", collectionName), Update.Replace<BsonDocument>(catalogRecord), UpdateFlags.Upsert);

            // Map Collection
            var map = _mongoDatabase.GetCollection<BsonDocument>(collectionName);

            // If it's exists, drop it!
            if (map.Exists())
            {
                map.Drop();
            }

            // First record
            int id = Mitab.mitab_c_next_feature_id(table, -1);

            // Until EOT
            while (id != -1)
            {
                // Read feature
                var feature = Mitab.mitab_c_read_feature(table, id);

                // Objects: Point & Region
                var points = new BsonArray();
                var featureType = Mitab.mitab_c_get_type(feature);

                // Collect points if featureType is Point or Region
                if ((featureType == Mitab.TABFC_Point) ||
                    (featureType == Mitab.TABFC_CustomPoint) ||
                    (featureType == Mitab.TABFC_FontPoint) ||
                    (featureType == Mitab.TABFC_Region))
                {
                    var partCount = Mitab.mitab_c_get_parts(feature);

                    for (int part = 0; part < partCount; part++)
                    {
                        var vertexCount = Mitab.mitab_c_get_vertex_count(feature, part);

                        BsonArray p = new BsonArray();

                        for (int i = 0; i < vertexCount; i++)
                        {
                            var fx = Mitab.mitab_c_get_vertex_x(feature, part, i);
                            var fy = Mitab.mitab_c_get_vertex_y(feature, part, i);

                            p.Add(fx);
                            p.Add(fy);
                        }

                        points.Add(p);
                    }
                }

                // Create Map Record
                var obj = new BsonDocument()
                {
                    { "Obj", new BsonDocument()
                        {
                            { "type", featureType },
                            { "coordinates", points }
                        }
                    }
                };

                // TAB Columns
                for (int i = 0; i < fieldCount; i++)
                {
                    var fieldType = Mitab.mitab_c_get_field_type(table, i);
                    var fieldName = Mitab.mitab_c_get_field_name_csharp(table, i);

                    // Handle DB_PRINX
                    if (string.Compare(fieldName, "DB_PRINX", true) == 0)
                    {
                        var v = Mitab.mitab_c_get_field_as_string_csharp(feature, i);
                        obj.Add("_id", new BsonObjectId(new ObjectId(v)));
                        continue;
                    }

                    switch (fieldType)
                    {
                        case Mitab.TABFT_Char:
                            obj.Add(new BsonElement(fieldName, Mitab.mitab_c_get_field_as_string_csharp(feature, i)));
                            break;

                        case Mitab.TABFT_Decimal:
                        case Mitab.TABFT_Float:
                            obj.Add(new BsonElement(fieldName, Mitab.mitab_c_get_field_as_double(feature, i)));
                            break;

                        case Mitab.TABFT_Integer:
                        case Mitab.TABFT_SmallInt:
                            obj.Add(new BsonElement(fieldName, (int)Mitab.mitab_c_get_field_as_double(feature, i)));
                            break;

                        default:
                            break;
                    }
                }

                // Destroy feature
                Mitab.mitab_c_destroy_feature(feature);

                // Insert map record into DB
                map.Insert<BsonDocument>(obj);

                // Next record
                id = Mitab.mitab_c_next_feature_id(table, id);
            }

            // Close table
            Mitab.mitab_c_close(table);

            // Everything is ok
            return (int)ErrorCodes.OK;
        }

        public int DownloadTable(string collectionName, string tabFilename)
        {
            var mapCatalog = _mongoDatabase.GetCollection<BsonDocument>("MAPCATALOG");

            if (!mapCatalog.Exists())
            {
                return (int)ErrorCodes.NoMapCatalogExists;
            }

            var collection = _mongoDatabase.GetCollection<BsonDocument>(collectionName);

            if (!collection.Exists())
            {
                return (int)ErrorCodes.CollectionNotFound;
            }

            var collectionDef = mapCatalog.FindOne(Query.EQ("name", collectionName));

            if (collectionDef == null)
            {
                return (int)ErrorCodes.TableNotRegistered;
            }

            var columns = collectionDef["columns"].AsBsonArray;

            var table = Mitab.mitab_c_create(tabFilename, "tab", "Earth Projection 1, 104", 0, 0, 0, 0);

            if (table == 0)
            {
                return (int)ErrorCodes.CantCreateTab;
            }

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i].AsBsonArray;

                var columnName = column[0].AsString;
                var columnType = column[1].AsInt32;

                switch (columnType)
                {
                    case Mitab.TABFT_Char:
                        var columnWidth = column[2].AsInt32;

                        Mitab.mitab_c_add_field(table, columnName, Mitab.TABFT_Char, columnWidth, 0, 0, 0);
                        break;

                    case Mitab.TABFT_Integer:
                        Mitab.mitab_c_add_field(table, columnName, Mitab.TABFT_Integer, 0, 0, 0, 0);
                        break;

                    case Mitab.TABFT_Float:
                        Mitab.mitab_c_add_field(table, columnName, Mitab.TABFT_Float, 0, 0, 0, 0);
                        break;
                }
            }

            // DB_PRINX
            Mitab.mitab_c_add_field(table, "DB_PRINX", Mitab.TABFT_Char, 24, 0, 0, 0);

            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalDigits = 12;
            nfi.NumberDecimalSeparator = ".";
            nfi.NumberGroupSeparator = "";

            var records = collection.FindAll();

            foreach (var item in records)
            {
                var obj = item["Obj"].AsBsonDocument;
                var objType = obj["type"].AsInt32;
#if WIN64
                long feature;
#else
                int feature;
#endif

                if ((objType == Mitab.TABFC_Region) ||
                    (objType == Mitab.TABFC_CustomPoint) ||
                    (objType == Mitab.TABFC_FontPoint) ||
                    (objType == Mitab.TABFC_Point))
                {
                    feature = Mitab.mitab_c_create_feature(table, objType);
                    
                    var points = obj["coordinates"].AsBsonArray;

                    for (int part = 0; part < points.Count; part++)
                    {
                        BsonArray p = points[part].AsBsonArray;

                        double[] fx = new double[p.Count / 2];
                        double[] fy = new double[p.Count / 2];

                        for (int i = 0; i < p.Count; i++)
                        {
                            if (i % 2 == 0)
                            {
                                fx[i / 2] = p[i].AsDouble;
                            }
                            else
                            {
                                fy[i / 2] = p[i].AsDouble;
                            }
                        }

                        Mitab.mitab_c_set_points(feature, part, fx.Length, fx, fy);
                    }
                }
                else
                {
                    feature = Mitab.mitab_c_create_feature(table, Mitab.TABFC_NoGeom);
                }

                for (int i = 0; i < columns.Count; i++)
                {
                    var column = columns[i].AsBsonArray;

                    var columnName = column[0].AsString;
                    var columnType = column[1].AsInt32;

                    switch (columnType)
                    {
                        case Mitab.TABFT_Char:
                            Mitab.mitab_c_set_field(feature, i, item[columnName].AsString);
                            break;

                        case Mitab.TABFT_Integer:
                            Mitab.mitab_c_set_field(feature, i, item[columnName].AsInt32.ToString());
                            break;

                        case Mitab.TABFT_Float:
                            Mitab.mitab_c_set_field(feature, i, item[columnName].AsDouble.ToString(nfi));
                            break;
                    }
                }

                // Last column is DB_PRINX
                Mitab.mitab_c_set_field(feature, columns.Count, item["_id"].AsObjectId.ToString());

                // Write and commit
                Mitab.mitab_c_write_feature(table, feature);
                Mitab.mitab_c_destroy_feature(feature);
            }

            Mitab.mitab_c_close(table);

            return (int)ErrorCodes.OK;
        }

        #region Dispose
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            disposed = true;

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            // base.Dispose(disposing);
        }

        ~MongoTableV3()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
