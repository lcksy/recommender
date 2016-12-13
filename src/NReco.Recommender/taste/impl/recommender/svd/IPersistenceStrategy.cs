
namespace NReco.CF.Taste.Impl.Recommender.SVD
{
    /// <summary>
    /// Provides storage for <see cref="Factorization"/>s
    /// </summary>
    public interface IPersistenceStrategy
    {
        /// <summary>Load a factorization from a persistent store.</summary>
        /// <returns>a Factorization or null if the persistent store is empty.</returns>
        Factorization Load();

        /// <summary>Write a factorization to a persistent store unless it already
        /// contains an identical factorization.</summary>
        void MaybePersist(Factorization factorization);
    }
}