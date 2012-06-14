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

        public DescriptionAttribute(string longDesc)
        {
            _longDesc = longDesc;
        }

        public DescriptionAttribute(string longDesc, string shortDesc)
        {
            _longDesc = longDesc;
            _shortDesc = shortDesc;
        }

        public string Desc { get { return _longDesc; } }
        public string ShortDesc { get { return _shortDesc; } }

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
                sb.Append("\t"); sb.AppendLine(string.Format(GetShortDescriptionOf(item), fieldValue));
            }

            return sb.ToString();
        }
    }
}
