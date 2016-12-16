using System;
using System.Collections.Concurrent;

namespace CQSS.Common.Infrastructure
{
    public class Singleton
    {
        private static readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

        public static ConcurrentDictionary<Type, object> Cache
        {
            get { return _cache; }
        }
    }

    public class Singleton<T> : Singleton
        where T : class
    {
        private static T _instance;

        public static T Instance
        {
            get 
            { 
                return _instance; 
            }
            set
            {
                if (_instance == null && Cache.TryAdd(typeof(T), value))
                    _instance = value;
            }
        }

        public static void Replace(T other)
        {
            if (Cache.TryUpdate(typeof(T), other, _instance))
                _instance = other;
        }
    }
}