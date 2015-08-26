using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace Baro.CoreLibrary.GIS
{
#if WIN64
    using TabHandle = System.Int64;    
#else
    using TabHandle = System.Int32;
#endif

    /// <summary>
    /// MITAB TOOLS
    /// </summary>
    public static class MitabTools
    {
        public unsafe static void CheckProjectionLLWgs84(TabHandle mitabHandle)
        {
            TabHandle prj = Mitab.mitab_c_get_projinfo(mitabHandle);
            byte* p = (byte*)prj;

            if (p[0] != 1 && p[56] != 104)
            {
                Exception eb = new Exception("Invalid TAB projection. Table must be LL/Wgs84 (CoordSys 1,104)");
                throw eb;
            }
        }

        public static int GetBiggestPart(TabHandle feature)
        {
            int partCount = Mitab.mitab_c_get_parts(feature);

            int biggestPart = 0;
            int biggestVCount = 0;

            for (int part = 0; part < partCount; part++)
            {
                int vCount = Mitab.mitab_c_get_vertex_count(feature, part);

                if (vCount >= biggestVCount)
                {
                    biggestPart = part;
                    biggestVCount = vCount;
                }
            }

            return biggestPart;
        }

        public static void GetCurrentCentroid(TabHandle feature, int part, out double cx, out double cy)
        {
            double x = 0, y = 0;
            cx = 0; cy = 0;

            int c = Mitab.mitab_c_get_vertex_count(feature, part);

            cx = Mitab.mitab_c_get_vertex_x(feature, part, 0);
            cy = Mitab.mitab_c_get_vertex_y(feature, part, 0);

            for (int k = 1; k < c; k++)
            {
                x = Mitab.mitab_c_get_vertex_x(feature, part, k);
                y = Mitab.mitab_c_get_vertex_y(feature, part, k);

                cx = (cx + x) / 2;
                cy = (cy + y) / 2;
            }
        }

        public static void GetEdgeNodes(TabHandle feature, int part, out double startLongX, out double startLatY,
            out double endLongX, out double endLatY)
        {
            int vCount = Mitab.mitab_c_get_vertex_count(feature, part);

            startLongX = (double)Mitab.mitab_c_get_vertex_x(feature, part, 0);
            startLatY = (double)Mitab.mitab_c_get_vertex_y(feature, part, 0);

            endLongX = (double)Mitab.mitab_c_get_vertex_x(feature, part, vCount - 1);
            endLatY = (double)Mitab.mitab_c_get_vertex_y(feature, part, vCount - 1);
        }

        public static double[][] GetCoordsD(TabHandle feature, int part)
        {
            int vCount = Mitab.mitab_c_get_vertex_count(feature, part);
            double[][] result = new double[vCount][];

            for (int k = 0; k < vCount; k++)
            {
                double x = (double)Mitab.mitab_c_get_vertex_x(feature, part, k);
                double y = (double)Mitab.mitab_c_get_vertex_y(feature, part, k);

                result[k] = new double[2] { x, y };
            }

            return result;
        }

        public static float[][] GetCoordsF(TabHandle feature, int part)
        {
            int vCount = Mitab.mitab_c_get_vertex_count(feature, part);
            float[][] result = new float[vCount][];

            for (int k = 0; k < vCount; k++)
            {
                float x = (float)Mitab.mitab_c_get_vertex_x(feature, part, k);
                float y = (float)Mitab.mitab_c_get_vertex_y(feature, part, k);

                result[k] = new float[2] { x, y };
            }

            return result;
        }
    }

    /// <summary>
    /// MITABCOL
    /// </summary>
    public sealed class MitabColumns
    {
        public struct Column
        {
            public int type;
            public string name;
            public int index;
        }

        private Dictionary<string, Column> columnlist = new Dictionary<string, Column>();

        public MitabColumns(TabHandle mitabHandle)
        {
            // Contract.Requires<ArgumentException>(mitabHandle != 0);

            for (int k = 0; k < Mitab.mitab_c_get_field_count(mitabHandle); k++)
            {
                Column c;

                c.name = Mitab.mitab_c_get_field_name_csharp(mitabHandle, k);
                c.type = Mitab.mitab_c_get_field_type(mitabHandle, k);
                c.index = k;

                columnlist.Add(c.name, c);
            }
        }

        public bool ColumnExists(string colName)
        {
            return columnlist.ContainsKey(colName);
        }

        public Column[] GetColumns()
        {
            return columnlist.Values.ToArray<Column>();
        }

        public Column GetColumn(string fieldName)
        {
            try
            {
                return columnlist[fieldName];
            }
            catch
            {
                throw new Exception(fieldName + " kolon bulunamadı");
            }            
        }

        public int Get(string fieldName)
        {
            try
            {
                return columnlist[fieldName].index;
            }
            catch
            {
                throw new Exception(fieldName + " kolon bulunamadı");
            }
        }
    }

    /// <summary>
    /// MITAB
    /// </summary>
    public static unsafe class Mitab
    {
#if WIN64
        const string DLL_NAME = "mitab64.dll";
#else
        const string DLL_NAME = "mitab.dll";
#endif

        // update to match mitab.h (app. line 194), when new versions are released
        public const int Libversion = 1007000;

        // feature type values
        public const int TABFC_NoGeom = 0;
        public const int TABFC_Point = 1;
        public const int TABFC_FontPoint = 2;
        public const int TABFC_CustomPoint = 3;
        public const int TABFC_Text = 4;
        public const int TABFC_Polyline = 5;
        public const int TABFC_Arc = 6;
        public const int TABFC_Region = 7;
        public const int TABFC_Rectangle = 8;
        public const int TABFC_Ellipse = 9;
        public const int TABFC_Multipoint = 10; // 1.2.0
        public const int TABFC_Collection = 11; // 1.7.0

        // field types
        public const int TABFT_Char = 1;
        public const int TABFT_Integer = 2;
        public const int TABFT_SmallInt = 3;
        public const int TABFT_Decimal = 4;
        public const int TABFT_Float = 5;
        public const int TABFT_Date = 6;
        public const int TABFT_Logical = 7;
        public const int TABFT_Time = 8;  // 1.7.0
        public const int TABFT_DateTime = 9;  // 1.7.0

        // text justification
        public const int TABTJ_Left = 0;
        public const int TABTJ_Center = 1;
        public const int TABTJ_Right = 2;

        // text spacing
        public const int TABTS_Single = 0;
        public const int TABTS_1_5 = 1;
        public const int TABTS_Double = 2;

        // test linetype
        public const int TABTL_NoLine = 0;
        public const int TABTL_Simple = 1;
        public const int TABTL_Arrow = 2;

        // No feature
        public const int NO_FEATURE = -1;

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_add_field(TabHandle handle, 
            string field_name, int field_type, int width, int precision, int indexed, int unique);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern void mitab_c_close(TabHandle handle);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern TabHandle mitab_c_create(string filename, 
            string mif_or_tab, string mif_projectiondef, double north, double south, double east, double west);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern TabHandle mitab_c_create_feature(TabHandle handle, int feature_type);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern void mitab_c_destroy_feature(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_affine_params(TabHandle projinfo, 
            ref int nAffineUnits, ref double[] adAffineParams);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_brush_bgcolor(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_brush_fgcolor(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_brush_pattern(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_brush_transparent(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern string mitab_c_get_charset(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern TabHandle mitab_c_get_collection_multipoint_ref(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern TabHandle mitab_c_get_collection_polyline_ref(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern TabHandle mitab_c_get_collection_region_ref(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern TabHandle mitab_c_get_datum_info(TabHandle projinfo, 
            ref double dDatumShiftX, ref double dDatumShiftY, ref double dDatumShiftZ, ref double[] adDatumParams);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern string mitab_c_get_extended_mif_coordsys(TabHandle handle);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_feature_count(TabHandle handle);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern double mitab_c_get_field_as_double(TabHandle feature, int field);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr mitab_c_get_field_as_string(TabHandle feature, int field);

        public static string mitab_c_get_field_as_string_csharp(TabHandle feature, int field)
        {
            var i = mitab_c_get_field_as_string(feature, field);
            var s = Marshal.PtrToStringAnsi(i);
            // Marshal.FreeHGlobal(i);

            return s;
        }

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_field_count(TabHandle handle);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr mitab_c_get_field_name(TabHandle handle, int field);

        public static string mitab_c_get_field_name_csharp(TabHandle handle, int field)
        {
            var i = mitab_c_get_field_name(handle, field);
            var s = Marshal.PtrToStringAnsi(i);
            // Marshal.FreeCoTaskMem(i);

            return s;
        }

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_field_precision(TabHandle handle, int field);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_field_type(TabHandle handle, int field);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_field_width(TabHandle handle, int field);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern string mitab_c_get_font(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern string mitab_c_get_mif_coordsys(TabHandle dataset);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_parts(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_pen_color(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_pen_pattern(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_pen_width(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern void mitab_c_get_projection_info(TabHandle projInfo,
            ref int nProjId, ref int nEllipsoidId, ref int nUnitsId, ref double[] adProjParams);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern TabHandle mitab_c_get_projinfo(TabHandle dataset);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_symbol_color(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_symbol_no(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_symbol_size(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern string mitab_c_get_text(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern double mitab_c_get_text_angle(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_text_bgcolor(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_text_fgcolor(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern double mitab_c_get_text_height(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_text_justification(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_text_linetype(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_text_spacing(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern double mitab_c_get_text_width(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_type(TabHandle feature);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_get_vertex_count(TabHandle feature, int part);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern double mitab_c_get_vertex_x(TabHandle feature, int part, int vertex);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern double mitab_c_get_vertex_y(TabHandle feature, int part, int vertex);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern string mitab_c_getlasterrormsg();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_getlasterrorno();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_getlibversion();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_is_field_indexed(TabHandle handle, int field);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_is_field_unique(TabHandle handle, int field);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_is_interior_ring(TabHandle feature, int ringindex);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_load_coordsys_table(string filename);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_next_feature_id(TabHandle handle, int last_feature_id);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern TabHandle mitab_c_open(string filename);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern TabHandle mitab_c_read_feature(TabHandle handle, int feature_id);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_set_affine_params(TabHandle projInfo, 
            int nAffineUnits, ref double[] adAffineParams);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern void mitab_c_set_arc(TabHandle feature, 
            double center_x, double center_y, double x_radius, double y_radius, double start_angle, double end_angle);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern TabHandle mitab_c_set_brush(TabHandle feature, 
            int fg_color, int bg_color, int pattern, int transparent);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_set_charset(TabHandle handle, string charset);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_set_collection_multipoint(TabHandle feature, TabHandle multipoint, int make_copy);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_set_collection_polyline(TabHandle feature, TabHandle polyline, int make_copy);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_set_collection_region(TabHandle feature, TabHandle region, int make_copy);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_set_datum_info(TabHandle projInfo, 
            double dDatumShiftX, double dDatumShiftY, double dDatumShiftZ, ref double[] adDatumParams);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern void mitab_c_set_field(TabHandle feature, int field, string value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern void mitab_c_set_font(TabHandle feature, string font_name);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern void mitab_c_set_pen(TabHandle feature, int width, int pattern, int color);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern void mitab_c_set_points(TabHandle feature, 
            int part, int vertex_count, double[] x, double[] y);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern void mitab_c_set_projection_info(TabHandle projInfo, 
            int nProjId, int nEllipsoidId, int nUnitsId, ref double[] adProjParams);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_set_projinfo(TabHandle dataset, TabHandle projInfo);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_set_quick_spatial_index_mode(TabHandle dataset, int value);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_set_symbol(TabHandle feature, 
            int symbol_no, int symbol_size, int symbol_color);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_set_text(TabHandle feature, string text);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_set_text_display(TabHandle feature, 
            double angle, double height, double width, int fg_color, int bg_color, int justification, int spacing, int linetype);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int mitab_c_write_feature(TabHandle handle, TabHandle feature);
    }
}
