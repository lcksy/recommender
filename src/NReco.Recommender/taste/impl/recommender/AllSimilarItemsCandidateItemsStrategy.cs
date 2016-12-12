using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// Returns the result of <see cref="IItemSimilarity.AllSimilarItemIDs"/> as candidate items
    /// </summary>
    public class AllSimilarItemsCandidateItemsStrategy : AbstractCandidateItemsStrategy
    {
        private IItemSimilarity similarity;

        public AllSimilarItemsCandidateItemsStrategy(IItemSimilarity similarity)
        {
            //Preconditions.checkArgument(similarity != null, "similarity is null");
            this.similarity = similarity;
        }

        protected override FastIDSet DoGetCandidateItems(long[] preferredItemIDs, IDataModel dataModel)
        {
            FastIDSet candidateItemIDs = new FastIDSet();
            foreach (long itemID in preferredItemIDs)
            {
                candidateItemIDs.AddAll(similarity.AllSimilarItemIDs(itemID));
            }
            candidateItemIDs.RemoveAll(preferredItemIDs);
            return candidateItemIDs;
        }
    }
}