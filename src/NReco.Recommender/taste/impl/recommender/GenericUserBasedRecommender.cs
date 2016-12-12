using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Neighborhood;
using NReco.CF.Taste.Recommender;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// A simple <see cref="NReco.CF.Taste.Recommender.IRecommender"/>
    /// which uses a given <see cref="IDataModel"/> and <see cref="IUserNeighborhood"/> to produce recommendations.
    /// </summary>
    public class GenericUserBasedRecommender : AbstractRecommender, IUserBasedRecommender
    {
        private static Logger log = LoggerFactory.GetLogger(typeof(GenericUserBasedRecommender));

        private IUserNeighborhood neighborhood;
        private IUserSimilarity similarity;
        private RefreshHelper refreshHelper;
        private EstimatedPreferenceCapper capper;

        public GenericUserBasedRecommender(IDataModel dataModel, IUserNeighborhood neighborhood, IUserSimilarity similarity)
            : base(dataModel)
        {
            //Preconditions.checkArgument(neighborhood != null, "neighborhood is null");
            this.neighborhood = neighborhood;
            this.similarity = similarity;
            this.refreshHelper = new RefreshHelper(() =>
            {
                capper = BuildCapper();
            });
            refreshHelper.AddDependency(dataModel);
            refreshHelper.AddDependency(similarity);
            refreshHelper.AddDependency(neighborhood);
            capper = BuildCapper();
        }

        public IUserSimilarity GetSimilarity()
        {
            return similarity;
        }

        public override IList<IRecommendedItem> Recommend(long userID, int howMany, IDRescorer rescorer)
        {
            //Preconditions.checkArgument(howMany >= 1, "howMany must be at least 1");

            log.Debug("Recommending items for user ID '{}'", userID);

            long[] theNeighborhood = neighborhood.GetUserNeighborhood(userID);

            if (theNeighborhood.Length == 0)
            {
                return new List<IRecommendedItem>();
            }

            FastIDSet allItemIDs = GetAllOtherItems(theNeighborhood, userID);
            TopItems.IEstimator<long> estimator = new Estimator(this, userID, theNeighborhood);

            List<IRecommendedItem> topItems = TopItems
                .GetTopItems(howMany, allItemIDs.GetEnumerator(), rescorer, estimator);

            log.Debug("Recommendations are: {}", topItems);
            return topItems;
        }

        public override float EstimatePreference(long userID, long itemID)
        {
            IDataModel model = GetDataModel();
            float? actualPref = model.GetPreferenceValue(userID, itemID);
            if (actualPref.HasValue)
            {
                return actualPref.Value;
            }
            long[] theNeighborhood = neighborhood.GetUserNeighborhood(userID);
            return DoEstimatePreference(userID, theNeighborhood, itemID);
        }

        public virtual long[] MostSimilarUserIDs(long userID, int howMany)
        {
            return MostSimilarUserIDs(userID, howMany, null);
        }

        public virtual long[] MostSimilarUserIDs(long userID, int howMany, IRescorer<Tuple<long, long>> rescorer)
        {
            TopItems.IEstimator<long> estimator = new MostSimilarEstimator(userID, similarity, rescorer);
            return DoMostSimilarUsers(howMany, estimator);
        }

        private long[] DoMostSimilarUsers(int howMany, TopItems.IEstimator<long> estimator)
        {
            IDataModel model = GetDataModel();
            return TopItems.GetTopUsers(howMany, model.GetUserIDs(), null, estimator);
        }

        protected virtual float DoEstimatePreference(long theUserID, long[] theNeighborhood, long itemID)
        {
            if (theNeighborhood.Length == 0)
            {
                return float.NaN;
            }
            IDataModel dataModel = GetDataModel();
            double preference = 0.0;
            double totalSimilarity = 0.0;
            int count = 0;
            foreach (long userID in theNeighborhood)
            {
                if (userID != theUserID)
                {
                    // See GenericItemBasedRecommender.doEstimatePreference() too
                    float? pref = dataModel.GetPreferenceValue(userID, itemID);
                    if (pref.HasValue)
                    {
                        double theSimilarity = similarity.UserSimilarity(theUserID, userID);
                        if (!Double.IsNaN(theSimilarity))
                        {
                            preference += theSimilarity * pref.Value;
                            totalSimilarity += theSimilarity;
                            count++;
                        }
                    }
                }
            }
            // Throw out the estimate if it was based on no data points, of course, but also if based on
            // just one. This is a bit of a band-aid on the 'stock' item-based algorithm for the moment.
            // The reason is that in this case the estimate is, simply, the user's rating for one item
            // that happened to have a defined similarity. The similarity score doesn't matter, and that
            // seems like a bad situation.
            if (count <= 1)
            {
                return float.NaN;
            }
            float estimate = (float)(preference / totalSimilarity);
            if (capper != null)
            {
                estimate = capper.CapEstimate(estimate);
            }
            return estimate;
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

        public override void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            refreshHelper.Refresh(alreadyRefreshed);
        }

        public override string ToString()
        {
            return "GenericUserBasedRecommender[neighborhood:" + neighborhood + ']';
        }

        private EstimatedPreferenceCapper BuildCapper()
        {
            IDataModel dataModel = GetDataModel();
            if (float.IsNaN(dataModel.GetMinPreference()) && float.IsNaN(dataModel.GetMaxPreference()))
            {
                return null;
            }
            else
            {
                return new EstimatedPreferenceCapper(dataModel);
            }
        }

        private sealed class MostSimilarEstimator : TopItems.IEstimator<long>
        {
            private long toUserID;
            private IUserSimilarity similarity;
            private IRescorer<Tuple<long, long>> rescorer;

            internal MostSimilarEstimator(long toUserID, IUserSimilarity similarity, IRescorer<Tuple<long, long>> rescorer)
            {
                this.toUserID = toUserID;
                this.similarity = similarity;
                this.rescorer = rescorer;
            }

            public double Estimate(long userID)
            {
                // Don't consider the user itself as a possible most similar user
                if (userID == toUserID)
                {
                    return Double.NaN;
                }
                if (rescorer == null)
                {
                    return similarity.UserSimilarity(toUserID, userID);
                }
                else
                {
                    Tuple<long, long> pair = new Tuple<long, long>(toUserID, userID);
                    if (rescorer.IsFiltered(pair))
                    {
                        return Double.NaN;
                    }
                    double originalEstimate = similarity.UserSimilarity(toUserID, userID);
                    return rescorer.Rescore(pair, originalEstimate);
                }
            }
        }

        private sealed class Estimator : TopItems.IEstimator<long>
        {

            private long theUserID;
            private long[] theNeighborhood;
            GenericUserBasedRecommender r;

            internal Estimator(GenericUserBasedRecommender r, long theUserID, long[] theNeighborhood)
            {
                this.r = r;
                this.theUserID = theUserID;
                this.theNeighborhood = theNeighborhood;
            }

            public double Estimate(long itemID)
            {
                return r.DoEstimatePreference(theUserID, theNeighborhood, itemID);
            }
        }
    }
}