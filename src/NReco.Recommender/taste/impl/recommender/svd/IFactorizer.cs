using NReco.CF.Taste.Common;

namespace NReco.CF.Taste.Impl.Recommender.SVD
{
    /// <summary>Implementation must be able to create a factorization of a rating matrix</summary>
    public interface IFactorizer : IRefreshable
    {
        Factorization Factorize();
    }
}