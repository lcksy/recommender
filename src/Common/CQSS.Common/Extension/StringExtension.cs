using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CQSS.Common.Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// 判断字符串是否为空或null
        /// </summary>
        public static bool IsEmpty(this string value)
        {
            return ((value == null) || (value.Length == 0));
        }

        /// <summary>
        /// 判断字符串是否不为空
        /// </summary>
        public static bool IsNotEmpty(this string value)
        {
            return !value.IsEmpty();
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// 与指定的字符串数组连接
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="OutOfMemoryException"/>
        public static string ConcatWith(this string value, params object[] args)
        {
            return string.Concat(value, string.Concat(args));
        }

        /// <summary>
        /// 字符串倒置
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static string Reverse(this string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value == string.Empty) return value;

            char[] chars = value.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        /// 从指定字符串中左截取指定长度
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static string Left(this string value, int length)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (length <= 0) throw new ArgumentException("length");
            if (length > value.Length) return value;

            return value.Substring(0, length);
        }

        /// <summary>
        /// 从指定字符串中右截取指定长度
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static string Right(this string value, int length)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (length <= 0) throw new ArgumentException("length");
            if (length > value.Length) return value;

            return value.Substring(value.Length - length);
        }

        /// <summary>
        /// 从指定字符串中右截取指定下标之间的字符串
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static string Between(this string value, int startIndex, int endIndex)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
            if (startIndex < 0) throw new ArgumentException("startIndex");
            if (endIndex > value.Length - 1) return value.Substring(startIndex);

            return value.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// 将指定日期格式的字符串转换为日期
        /// </summary>
        /// <param name="value">指定日期格式的字符串</param>
        /// <param name="format">日期格式</param>
        /// <returns>日期</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static DateTime ToDateTime(this string value, string format)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");

            return DateTime.ParseExact(value, format, null);
        }

        /// <summary>
        /// 将指定日期格式的字符串转换为日期
        /// </summary>
        /// <param name="value">指定日期格式的字符串</param>
        /// <param name="format">日期格式</param>
        /// <returns>日期</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static DateTime ToDate(this string value)
        {
            return value.ToDateTime("yyyy-MM-dd");
        }

        /// <summary>
        /// 将指定字符串转换为decimal类型
        /// <param name="value">指定decimal类型的字符串</param>
        /// </summary>
        /// <returns>decimal</returns>
        public static decimal ToDecimal(this string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            decimal result = default(decimal);
            decimal.TryParse(value, out result);

            return result;
        }

        /// <summary>
        /// 将指定日期格式的字符串转换为日期
        /// </summary>
        /// <param name="value">指定日期格式的字符串</param>
        /// <param name="format">日期格式</param>
        /// <returns>日期</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static DateTime ToDateTime(this string value)
        {
            return value.ToDateTime("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 将日期格式的字符串转换为新格式
        /// </summary>
        /// <param name="value">日期格式的字符串</param>
        /// <param name="oldFormat">旧格式</param>
        /// <param name="newFormat">新格式</param>
        /// <returns>新格式的日期字符串</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static string ChangeDateTimeFormat(this string value, string oldFormat, string newFormat)
        {
            return value.ToDateTime(oldFormat).ToString(newFormat);
        }

        /// <summary>
        /// 将明文字符串转换为 Hash 字符串
        /// </summary>
        /// <param name="plainText">明文字符串</param>
        /// <param name="hashName">Hash 算法名称，如 MD5、SHA1 等</param>
        /// <param name="encoding">将文明字符串转换为 byte[] 所需的字符集</param>
        /// <returns>Hash 字符串</returns>
        /// <exception cref="ArgumentNullException"/>
        public static string ToHashText(this string plainText, string hashName = "", Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            if (string.IsNullOrEmpty(hashName))
                hashName = "MD5";

            using (var algorithm = HashAlgorithm.Create(hashName))
            {
                if (algorithm == null)
                    throw new ArgumentException("hashName");

                var plainBytes = encoding.GetBytes(plainText);
                var hashBytes = algorithm.ComputeHash(plainBytes);
                var hashText = BitConverter.ToString(hashBytes).Replace("-", "");

                return hashText;
            }
        }

        /// <summary>
        /// 将字符串分隔，返回二元组枚举数
        /// </summary>
        /// <typeparam name="TKey">二元组的 Key 类型</typeparam>
        /// <typeparam name="TValue">二元组的 Value 类型</typeparam>
        /// <param name="joinedString">待分隔的字符串</param>
        /// <param name="columnSeparator">列分隔字符</param>
        /// <param name="rowSeparator">行分隔字符</param>
        /// <param name="keyConverter">将字符串转换为 TKey 类型的转换器</param>
        /// <param name="valueConverter">将字符串转换为 TValue 类型的转换器</param>
        /// <returns>二元组枚举数</returns>
        public static IEnumerable<Tuple<TKey, TValue>> SplitToTuples<TKey, TValue>(this string joinedString, char columnSeparator, char rowSeparator,
            Func<string, TKey> keyConverter, Func<string, TValue> valueConverter)
        {
            var rows = joinedString.Split(new char[] { rowSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                var columns = row.Split(new char[] { columnSeparator }, StringSplitOptions.RemoveEmptyEntries);
                var key = keyConverter(columns[0]);
                var value = valueConverter(columns[1]);

                yield return new Tuple<TKey, TValue>(key, value);
            }
        }

        /// <summary>
        /// 将字符串分隔，返回二元组枚举数
        /// </summary>
        /// <param name="joinedString">待分隔的字符串</param>
        /// <param name="columnSeparator">列分隔字符</param>
        /// <param name="rowSeparator">行分隔字符</param>
        /// <returns>二元组枚举数</returns>
        public static IEnumerable<Tuple<string, string>> SplitToTuples(this string joinedString, char columnSeparator, char rowSeparator)
        {
            var rows = joinedString.Split(new char[] { rowSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                var columns = row.Split(new char[] { columnSeparator }, StringSplitOptions.RemoveEmptyEntries);
                var key = columns[0];
                var value = columns[1];

                yield return new Tuple<string, string>(key, value);
            }
        }

        public static string UrlEncode(this string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        public static string UrlDecode(this string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        public static Dictionary<TKey, TValue> SplitToDictionary<TKey, TValue>(this string joinedString, char columnSeparator, char rowSeparator, Func<string, TKey> keySelector, Func<string, TValue> valueSelector)
        {
            Dictionary<TKey, TValue> keyValues = new Dictionary<TKey, TValue>();
            var rows = joinedString.Split(new char[] { rowSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var row in rows)
            {
                var columns = row.Split(new char[] { columnSeparator }, StringSplitOptions.RemoveEmptyEntries);
                var key = keySelector(columns[0]);
                var value = valueSelector(columns[1]);

                keyValues.Add(key, value);
            }

            return keyValues;
        }

        public static IEnumerable<string> Split(this string source, char separator = ',', bool removeEmpty = true)
        {
            var option = removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            return source.Split(new char[] { separator }, option);
        }

        public static IEnumerable<string> Split(this string source, bool removeEmpty = true, params char[] separators)
        {
            if (separators == null || separators.Length == 0)
                throw new ArgumentNullException("separators");

            var option = removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            return source.Split(separators, option);
        }
    }
}