
namespace NReco.CF.Taste.Impl.Common
{
    /// <summary>
    /// Implementations can retrieve a value for a given key.
    /// </summary>
    public interface IRetriever<TKey, TValue>
    {
        /// @param key key for which a value should be retrieved
        /// @return value for key
        /// @throws TasteException if an error occurs while retrieving the value
        TValue Get(TKey key);
    }
}