
namespace NReco.CF.Taste.Impl.Recommender.SVD
{
    /// <summary>A <see cref="IPersistenceStrategy"/> which does nothing.</summary>
    public class NoPersistenceStrategy : IPersistenceStrategy
    {
        public Factorization Load()
        {
            return null;
        }

        public void MaybePersist(Factorization factorization)
        {
            // do nothing.
        }
    }
}