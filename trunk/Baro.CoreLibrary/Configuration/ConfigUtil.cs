using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Configuration
{
    internal static class ConfigUtil
    {
        public static string ConvertToConfigValue(string str)
        {
            return str.Replace("\u000D\u000A", "[CR]").Replace("=", "[--]");
        }

        public static string ConvertToString(string configValue)
        {
            return configValue.Replace("[--]", "=").Replace("[CR]", "\u000D\u000A");
        }

        public static Dictionary<string, string> CreateKVPDictionary()
        {
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public static Dictionary<string, IConfigSection> CreateSectionDictionary()
        {
            return new Dictionary<string, IConfigSection>(StringComparer.InvariantCultureIgnoreCase);
        }

        public static string GetSectionTitle(string s)
        {
            s = s.Trim();
            
            if (s.StartsWith("[") && s.EndsWith("]"))
                return s.Trim('[', ']');
            else return null;
        }

        public static bool isKeyValuePair(string s)
        {
            return s.IndexOf('=') > 0;
        }
    }
}
