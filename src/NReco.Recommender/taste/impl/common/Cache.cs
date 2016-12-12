using System;
using System.Collections.Generic;
using System.Linq;

namespace NReco.CF.Taste.Impl.Common
{
    /// <summary>
    /// An efficient Map-like class which caches values for keys. Values are not "put" into a <see cref="Cache"/>;
    /// instead the caller supplies the instance with an implementation of <see cref="IRetriever"/> which can load the
    /// value for a given key.
    /// <para>
    /// The cache does not support <code>null</code> keys.
    /// </para>
    /// </summary>
    public sealed class Cache<K, V> : IRetriever<K, V>
    {
        private static Object NULL = new Object();

        private IDictionary<K, V> cache;
        private IRetriever<K, V> retriever; //Retriever<? super K,? extends V>

        /// <summary>
        /// Creates a new cache based on the given <see cref="IRetriever"/>.
        /// </summary>
        /// <param name="retriever">object which can retrieve values for keys</param>       
        public Cache(IRetriever<K, V> retriever)
            : this(retriever, Int32.MaxValue)
        {
        }

        /// <summary>
        /// Creates a new cache based on the given {@link Retriever} and with given maximum size.
        /// </summary>
        /// <param name="retriever">object which can retrieve values for keys</param>
        /// <param name="maxEntries">maximum number of entries the cache will store before evicting some</param>     
        public Cache(IRetriever<K, V> retriever, int maxEntries)
        {
            //Preconditions.checkArgument(retriever != null, "retriever is null");
            //Preconditions.checkArgument(maxEntries >= 1, "maxEntries must be at least 1");
            cache = new Dictionary<K, V>(11);
            this.retriever = retriever;
        }

        /// <summary>
        /// Returns cached value for a key. If it does not exist, it is loaded using a {@link Retriever}.
        /// </summary>
        /// <param name="key">cache key</param>
        /// <returns>value for that key</returns>
        public V Get(K key)
        {
            V value;
            bool tryGetVal;
            lock (cache)
            {
                tryGetVal = cache.TryGetValue(key, out value);
            }
            if (!tryGetVal)
            {
                return GetAndCacheValue(key);
            }
            return NULL.Equals(value) ? default(V) : value;
        }

        /// <summary>
        /// Uncaches any existing value for a given key.
        /// </summary>
        /// <param name="key">cache key</param>     
        public void Remove(K key)
        {
            lock (cache)
            {
                cache.Remove(key);
            }
        }

        /// <summary>Clears all cache entries whose key matches the given predicate.</summary>
        public void RemoveKeysMatching(Func<K, bool> predicate)
        {
            lock (cache)
            {
                var it = cache.Keys.ToArray();
                foreach (var key in it)
                {
                    if (predicate(key))
                    {
                        cache.Remove(key);
                    }
                }
            }
        }

        /// <summary>Clears all cache entries whose value matches the given predicate.</summary>
        public void RemoveValueMatching(Func<V, bool> predicate)
        {
            lock (cache)
            {
                var it = cache.ToArray();
                foreach (var entry in it)
                {
                    if (predicate(entry.Value))
                    {
                        cache.Remove(entry);
                    }
                }
            }
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            lock (cache)
            {
                cache.Clear();
            }
        }

        private V GetAndCacheValue(K key)
        {
            V value = retriever.Get(key);
            if (value == null)
            {
                value = (V)NULL;
            }
            lock (cache)
            {
                cache[key] = value;
            }
            return value;
        }

        public override string ToString()
        {
            return "Cache[retriever:" + retriever.ToString() + ']';
        }
    }
}