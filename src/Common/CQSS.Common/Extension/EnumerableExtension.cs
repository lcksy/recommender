using System;
using System.Collections.Generic;
using System.Linq;

namespace CQSS.Common.Extension
{
    public static class EnumerableExtension
    {
        public static Dictionary<TKey, List<TValue>> MapKeyToValues<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector, bool forceDistinct = false)
        {
            var map = new Dictionary<TKey, List<TValue>>();

            foreach (var item in source)
            {
                var key = keySelector(item);
                var value = valueSelector(item);

                if (!map.ContainsKey(key))
                    map.Add(key, new List<TValue>());

                if (!forceDistinct || !map[key].Contains(value))
                    map[key].Add(value);
            }

            return map;
        }

        public static string Join<T>(this IEnumerable<T> source, string separator = ",", Func<T, string> selector = null)
        {
            if (selector == null)
                selector = t => t.ToString();

            return string.Join(separator, source.Select(t => selector(t)));
        }

        public static IEnumerable<TElement> SelectRepeat<T, TElement>(this IEnumerable<T> items, Func<T, TElement> elementSelector, Func<T, int> repeatSelector)
        {
            var result = new List<TElement>();
            foreach (var item in items)
            {
                var element = elementSelector(item);
                var repeat = repeatSelector(item);
                result.AddRange(Enumerable.Repeat(element, repeat));
            }

            return result;
        }

        public static IEnumerable<TElement> Right<TElement>(this IEnumerable<TElement> items, int length)
        {
            if (items == null || items.Any() == false) return Enumerable.Empty<TElement>();
            if (length <= 0) return Enumerable.Empty<TElement>();
            if (length > items.Count()) return items;

            return items.Skip(items.Count() - length).Take(length);
        }
    }
}