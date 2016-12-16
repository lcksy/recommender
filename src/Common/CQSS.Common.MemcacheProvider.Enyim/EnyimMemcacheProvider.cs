using CQSS.Common.Infrastructure.MemcacheProvider;
using CQSS.Common.Infrastructure.Serializing;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CQSS.Common.MemcacheProvider.Enyim
{
    public class EnyimMemcacheProvider : IMemcacheProvider
    {
        #region Fields

        protected MemcachedClient _client = null;
        protected IJsonSerializer _serializer = null;

        #endregion

        #region Ctor

        public EnyimMemcacheProvider(IJsonSerializer serializer = null)
        {
            _serializer = serializer;
            _client = MemcachedClient.CacheClient;
        }

        #endregion

        #region Methods

        protected virtual object Serialize(object obj)
        {
            if (_serializer != null && obj.GetType().IsClass)
                return _serializer.Serialize(obj);
            else
                return obj;
        }

        protected virtual object Deserialize(object storeObject, Type type)
        {
            if (_serializer != null && type.IsClass)
                return _serializer.Deserialize(storeObject.ToString(), type);
            else
                return storeObject;
        }

        #endregion

        #region IMemcacheProvider implement

        public T Get<T>(string key)
        {
            var storeObject = _client.Get(key);
            return (T)this.Deserialize(storeObject, typeof(T));
        }

        public IEnumerable<T> GetMulti<T>(IEnumerable<string> keys)
        {
            var keyValueMap = _client.Get_Multi(keys);
            foreach (var kvp in keyValueMap)
                yield return (T)this.Deserialize(kvp.Value, typeof(T));
        }

        public Dictionary<string, T> GetDictionary<T>(IEnumerable<string> keys)
        {
            var keyValueMap = _client.Get_Multi(keys);
            return keyValueMap.ToDictionary(kv => kv.Key, kv => (T)this.Deserialize(kv.Value, typeof(T)));
        }

        public bool Set<T>(string key, T value)
        {
            var storeObject = this.Serialize(value);
            return _client.Store(StoreMode.Set, key, storeObject);
        }

        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            var storeObject = this.Serialize(value);
            return _client.Store(StoreMode.Set, key, storeObject, expiresAt);
        }

        public bool Set<T>(string key, T value, TimeSpan validFor)
        {
            var storeObject = this.Serialize(value);
            return _client.Store(StoreMode.Set, key, storeObject, validFor);
        }

        public bool Remove(string key)
        {
            return _client.Remove(key);
        }

        public bool Exists(string key)
        {
            return _client.KeyExists(key);
        }

        #endregion
    }
}