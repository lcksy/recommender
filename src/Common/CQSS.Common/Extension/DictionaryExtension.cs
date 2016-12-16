using System.Collections.Generic;
using System.Linq;

namespace CQSS.Common.Extension
{
    public static class DictionaryExtension
    {
        public static string Join<TKey, TValue>(this Dictionary<TKey, TValue> source, string columnSeprator, string rowSeparator)
        {
            if (source == null || source.Count == 0)
                return string.Empty;

            var array = source.Select(kv => "{0}{1}{2}".FormatWith(kv.Key, columnSeprator, kv.Value)).ToArray();
            return string.Join(rowSeparator, array);
        }
    }
}