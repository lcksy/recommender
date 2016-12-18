using System;

namespace NReco.Recommender.Extension.Extension
{
    public static class StringExtension
    {
        public static int ToInt(this string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source is null");

            int value = default(int);

            int.TryParse(source, out value);

            return value;
        }

        public static TOut Default<TInput, TOut>(this TInput source, Func<TInput, TOut> func)
        {
            if(source.Equals(default(TInput)))
            { 
                if (func != null)
                    return func(source);
            }

            return (TOut)(object)source;
        }
    }
}