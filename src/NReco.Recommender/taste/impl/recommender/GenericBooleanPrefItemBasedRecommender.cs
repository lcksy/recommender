using System;

using NReco.CF.Taste.Model;
using NReco.CF.Taste.Recommender;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// A variant on <see cref="GenericItemBasedRecommender"/> which is appropriate for use when no notion of preference
    /// value exists in the data.
    /// </summary>
    /// <seealso cref="NReco.CF.Taste.Impl.Recommender.GenericBooleanPrefUserBasedRecommender"/>
    public sealed class GenericBooleanPrefItemBasedRecommender : GenericItemBasedRecommender
    {
        public GenericBooleanPrefItemBasedRecommender(IDataModel dataModel, IItemSimilarity similarity)
            : base(dataModel, similarity)
        {
        }

        public GenericBooleanPrefItemBasedRecommender(IDataModel dataModel, IItemSimilarity similarity,
            ICandidateItemsStrategy candidateItemsStrategy, IMostSimilarItemsCandidateItemsStrategy
            mostSimilarItemsCandidateItemsStrategy)
            : base(dataModel, similarity, candidateItemsStrategy, mostSimilarItemsCandidateItemsStrategy)
        {

        }

        /// This computation is in a technical sense, wrong, since in the domain of "bool preference users" where
        /// all preference values are 1, this method should only ever return 1.0 or NaN. This isn't terribly useful
        /// however since it means results can't be ranked by preference value (all are 1). So instead this returns a
        /// sum of similarities.
        protected override float DoEstimatePreference(long userID, IPreferenceArray preferencesFromUser, long itemID)
        {
            double[] similarities = GetSimilarity().ItemSimilarities(itemID, preferencesFromUser.GetIDs());
            bool foundAPref = false;
            double totalSimilarity = 0.0;
            foreach (double theSimilarity in similarities)
            {
                if (!Double.IsNaN(theSimilarity))
                {
                    foundAPref = true;
                    totalSimilarity += theSimilarity;
                }
            }
            return foundAPref ? (float)totalSimilarity : float.NaN;
        }

        public override string ToString()
        {
            return "GenericBooleanPrefItemBasedRecommender";
        }
    }
}