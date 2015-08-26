using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Core
{
    public class WildcardFilter : Regex
    {
        public WildcardFilter(string pattern)
            : base(WildcardToRegex(pattern))
        {
        }

        public WildcardFilter(string pattern, RegexOptions options)
            : base(WildcardToRegex(pattern), options)
        {
        }

        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).
             Replace("\\*", ".*").
             Replace("\\?", ".") + "$";
        }
    }

    public class WildcardFilters
    {
        private WildcardFilter[] _filters;

        public WildcardFilters(string patterns)
            : this(patterns, RegexOptions.IgnoreCase)
        {
        }

        public WildcardFilters(string patterns, RegexOptions options)
        {
            var list = patterns.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            _filters = new WildcardFilter[list.Length];

            for (int i = 0; i < list.Length; i++)
            {
                _filters[i] = new WildcardFilter(list[i], options);
            }
        }

        public bool IsMatch(string value)
        {
            foreach (var item in _filters)
            {
                if (item.IsMatch(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
