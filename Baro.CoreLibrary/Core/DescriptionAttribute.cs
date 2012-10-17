using System;

using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Baro.CoreLibrary
{
    /// <summary>
    /// Obfuscate edilmiş nesneler için açıklama, ama her yerde kullanılabilir.
    /// </summary>
    public class DescriptionAttribute : Attribute
    {
        private string _longDesc, _shortDesc;
        private bool _isDateTime;

        public DescriptionAttribute(string longDesc)
        {
            _longDesc = longDesc;
        }

        public DescriptionAttribute(string longDesc, string shortDesc)
        {
            _longDesc = longDesc;
            _shortDesc = shortDesc;
        }

        public DescriptionAttribute(string longDesc, string shortDesc, bool isDateTime)
        {
            _longDesc = longDesc;
            _shortDesc = shortDesc;
            _isDateTime = isDateTime;
        }

        public string Desc { get { return _longDesc; } }
        public string ShortDesc { get { return _shortDesc; } }
        public bool isDateTime { get { return _isDateTime; } }

        public static string GetShortDescriptionOf(Type type)
        {
            object[] atts = type.GetCustomAttributes(typeof(DescriptionAttribute), false);

            foreach (DescriptionAttribute da in atts)
            {
                return da.ShortDesc;
            }

            return null;
        }

        public static string GetShortDescriptionOf(FieldInfo fi)
        {
            object[] atts = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            foreach (DescriptionAttribute da in atts)
            {
                return da.ShortDesc;
            }

            return null;
        }

        public static string GetShortDescriptionOf(FieldInfo fi, out bool isDateTime)
        {
            object[] atts = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            foreach (DescriptionAttribute da in atts)
            {
                isDateTime = da.isDateTime;
                return da.ShortDesc;
            }

            isDateTime = false;
            return null;
        }

        public static string GetDescriptionOf(Type type)
        {
            object[] atts = type.GetCustomAttributes(typeof(DescriptionAttribute), false);

            foreach (DescriptionAttribute da in atts)
            {
                return da.Desc;
            }

            return null;
        }

        public static string ObjDebugString(object obj)
        {
            Type type = obj.GetType();
            string objDesc = GetShortDescriptionOf(type);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(objDesc);

            FieldInfo[] fi = type.GetFields();

            foreach (var item in fi)
            {
                object fieldValue = item.GetValue(obj);
                
                bool isDateTime;
                string sd = GetShortDescriptionOf(item, out isDateTime);

                if (isDateTime && item.FieldType == typeof(double))
                {
                    DateTime dt = DateTime.FromOADate((double)fieldValue);
                    sb.Append("\t"); sb.AppendLine(string.Format(sd, dt.ToString()));
                }
                else
                {
                    sb.Append("\t"); sb.AppendLine(string.Format(sd, fieldValue));
                }
            }

            return sb.ToString();
        }
    }
}
