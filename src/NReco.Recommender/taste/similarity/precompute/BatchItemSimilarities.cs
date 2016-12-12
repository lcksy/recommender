using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Similarity.Precompute
{
    public abstract class BatchItemSimilarities
    {
        private IItemBasedRecommender recommender;
        private int similarItemsPerItem;

        /// @param recommender recommender to use
        /// @param similarItemsPerItem number of similar items to compute per item
        protected BatchItemSimilarities(IItemBasedRecommender recommender, int similarItemsPerItem)
        {
            this.recommender = recommender;
            this.similarItemsPerItem = similarItemsPerItem;
        }

        protected IItemBasedRecommender GetRecommender()
        {
            return recommender;
        }

        protected int GetSimilarItemsPerItem()
        {
            return similarItemsPerItem;
        }

        /// @param degreeOfParallelism number of threads to use for the computation
        /// @param maxDurationInHours  maximum duration of the computation
        /// @param writer  {@link SimilarItemsWriter} used to persist the results
        /// @return  the number of similarities precomputed
        /// @throws IOException
        /// @throws RuntimeException if the computation takes longer than maxDurationInHours
        public abstract int ComputeItemSimilarities(int degreeOfParallelism, int maxDurationInHours, ISimilarItemsWriter writer);
    }
}