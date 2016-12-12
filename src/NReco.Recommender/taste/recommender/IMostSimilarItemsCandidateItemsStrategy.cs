using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Recommender
{
    /// <summary>
    /// Used to retrieve all items that could possibly be similar
    /// </summary>
    public interface IMostSimilarItemsCandidateItemsStrategy : IRefreshable
    {
        FastIDSet GetCandidateItems(long[] itemIDs, IDataModel dataModel);
    }
}