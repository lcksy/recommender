using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Neighborhood;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// A variant on <see cref="GenericUserBasedRecommender"/> which is appropriate for use when no notion of preference
    /// value exists in the data.
    /// </summary>
    public sealed class GenericBooleanPrefUserBasedRecommender : GenericUserBasedRecommender
    {
        public GenericBooleanPrefUserBasedRecommender(IDataModel dataModel, IUserNeighborhood neighborhood, IUserSimilarity similarity)
            : base(dataModel, neighborhood, similarity)
        {
        }

        /// This computation is in a technical sense, wrong, since in the domain of "bool preference users" where
        /// all preference values are 1, this method should only ever return 1.0 or NaN. This isn't terribly useful
        /// however since it means results can't be ranked by preference value (all are 1). So instead this returns a
        /// sum of similarities to any other user in the neighborhood who has also rated the item.
        protected override float DoEstimatePreference(long theUserID, long[] theNeighborhood, long itemID)
        {
            if (theNeighborhood.Length == 0)
            {
                return float.NaN;
            }
            IDataModel dataModel = GetDataModel();
            IUserSimilarity similarity = GetSimilarity();
            float totalSimilarity = 0.0f;
            bool foundAPref = false;
            foreach (long userID in theNeighborhood)
            {
                // See GenericItemBasedRecommender.doEstimatePreference() too
                if (userID != theUserID && dataModel.GetPreferenceValue(userID, itemID) != null)
                {
                    foundAPref = true;
                    totalSimilarity += (float)similarity.UserSimilarity(theUserID, userID);
                }
            }
            return foundAPref ? totalSimilarity : float.NaN;
        }

        protected FastIDSet GetAllOtherItems(long[] theNeighborhood, long theUserID)
        {
            IDataModel dataModel = GetDataModel();
            FastIDSet possibleItemIDs = new FastIDSet();
            foreach (long userID in theNeighborhood)
            {
                possibleItemIDs.AddAll(dataModel.GetItemIDsFromUser(userID));
            }
            possibleItemIDs.RemoveAll(dataModel.GetItemIDsFromUser(theUserID));
            return possibleItemIDs;
        }

        public override string ToString()
        {
            return "GenericBooleanPrefUserBasedRecommender";
        }
    }
}