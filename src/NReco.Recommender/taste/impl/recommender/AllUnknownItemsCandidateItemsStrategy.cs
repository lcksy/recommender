using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// Return all items the user has not yet seen
    /// </summary>
    public sealed class AllUnknownItemsCandidateItemsStrategy : AbstractCandidateItemsStrategy
    {
        protected override FastIDSet DoGetCandidateItems(long[] preferredItemIDs, IDataModel dataModel)
        {
            FastIDSet possibleItemIDs = new FastIDSet(dataModel.GetNumItems());
            var allItemIDs = dataModel.GetItemIDs();
            while (allItemIDs.MoveNext())
            {
                possibleItemIDs.Add(allItemIDs.Current);
            }
            possibleItemIDs.RemoveAll(preferredItemIDs);
            return possibleItemIDs;
        }
    }
}