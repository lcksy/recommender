using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Impl.Model;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// A <see cref="IRecommender"/> which caches the results from another <see cref="IRecommender"/> in memory.
    /// </summary>
    public sealed class CachingRecommender : IRecommender
    {
        private static Logger log = LoggerFactory.GetLogger(typeof(CachingRecommender));

        private IRecommender recommender;
        private int[] maxHowMany;
        private IRetriever<long, Recommendations> recommendationsRetriever;
        private Cache<long, Recommendations> recommendationCache;
        private Cache<Tuple<long, long>, float> estimatedPrefCache;
        private RefreshHelper refreshHelper;
        private IDRescorer currentRescorer;

        public CachingRecommender(IRecommender recommender)
        {
            //Preconditions.checkArgument(recommender != null, "recommender is null");
            this.recommender = recommender;
            maxHowMany = new int[] { 1 };
            // Use "num users" as an upper limit on cache size. Rough guess.
            int numUsers = recommender.GetDataModel().GetNumUsers();
            recommendationsRetriever = new RecommendationRetriever(this);
            recommendationCache = new Cache<long, Recommendations>(recommendationsRetriever, numUsers);
            estimatedPrefCache = new Cache<Tuple<long, long>, float>(new EstimatedPrefRetriever(this), numUsers);
            refreshHelper = new RefreshHelper(() =>
            {
                Clear();
            });
            refreshHelper.AddDependency(recommender);
        }

        private void SetCurrentRescorer(IDRescorer rescorer)
        {
            if (rescorer == null)
            {
                if (currentRescorer != null)
                {
                    currentRescorer = null;
                    Clear();
                }
            }
            else
            {
                if (!rescorer.Equals(currentRescorer))
                {
                    currentRescorer = rescorer;
                    Clear();
                }
            }
        }

        public IList<IRecommendedItem> Recommend(long userID, int howMany)
        {
            return Recommend(userID, howMany, null);
        }

        public IList<IRecommendedItem> Recommend(long userID, int howMany, IDRescorer rescorer)
        {
            //Preconditions.checkArgument(howMany >= 1, "howMany must be at least 1");
            lock (maxHowMany)
            {
                if (howMany > maxHowMany[0])
                {
                    maxHowMany[0] = howMany;
                }
            }

            // Special case, avoid caching an anonymous user
            if (userID == PlusAnonymousUserDataModel.TEMP_USER_ID)
            {
                return recommendationsRetriever.Get(PlusAnonymousUserDataModel.TEMP_USER_ID).GetItems();
            }

            SetCurrentRescorer(rescorer);

            Recommendations recommendations = recommendationCache.Get(userID);
            if (recommendations.GetItems().Count < howMany && !recommendations.IsNoMoreRecommendableItems())
            {
                Clear(userID);
                recommendations = recommendationCache.Get(userID);
                if (recommendations.GetItems().Count < howMany)
                {
                    recommendations.SetNoMoreRecommendableItems(true);
                }
            }

            List<IRecommendedItem> recommendedItems = recommendations.GetItems();
            return recommendedItems.Count > howMany ? recommendedItems.GetRange(0, howMany) : recommendedItems;
        }

        public float EstimatePreference(long userID, long itemID)
        {
            return estimatedPrefCache.Get(new Tuple<long, long>(userID, itemID));
        }

        public void SetPreference(long userID, long itemID, float value)
        {
            recommender.SetPreference(userID, itemID, value);
            Clear(userID);
        }

        public void RemovePreference(long userID, long itemID)
        {
            recommender.RemovePreference(userID, itemID);
            Clear(userID);
        }

        public IDataModel GetDataModel()
        {
            return recommender.GetDataModel();
        }

        public void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            refreshHelper.Refresh(alreadyRefreshed);
        }

        /// <p>
        /// Clears cached recommendations for the given user.
        /// </p>
        /// 
        /// @param userID
        ///          clear cached data associated with this user ID
        public void Clear(long userID)
        {
            log.Debug("Clearing recommendations for user ID '{}'", userID);
            recommendationCache.Remove(userID);
            estimatedPrefCache.RemoveKeysMatching((Tuple<long, long> userItemPair) =>
            {
                return userItemPair.Item1 == userID;
            });
        }

        /// <p>
        /// Clears all cached recommendations.
        /// </p>
        public void Clear()
        {
            log.Debug("Clearing all recommendations...");
            recommendationCache.Clear();
            estimatedPrefCache.Clear();
        }

        public override string ToString()
        {
            return "CachingRecommender[recommender:" + recommender + ']';
        }

        private sealed class RecommendationRetriever : IRetriever<long, Recommendations>
        {
            CachingRecommender p;

            internal RecommendationRetriever(CachingRecommender parent)
            {
                p = parent;
            }

            public Recommendations Get(long key)
            {
                log.Debug("Retrieving new recommendations for user ID '{}'", key);
                int howMany = p.maxHowMany[0];
                IDRescorer rescorer = p.currentRescorer;
                var recommendations =
                    rescorer == null ? p.recommender.Recommend(key, howMany) : p.recommender.Recommend(key, howMany, rescorer);
                return new Recommendations(new List<IRecommendedItem>(recommendations));
            }
        }

        private sealed class EstimatedPrefRetriever : IRetriever<Tuple<long, long>, float>
        {
            CachingRecommender p;
            internal EstimatedPrefRetriever(CachingRecommender parent)
            {
                p = parent;
            }

            public float Get(Tuple<long, long> key)
            {
                long userID = key.Item1;
                long itemID = key.Item2;
                log.Debug("Retrieving estimated preference for user ID '{}' and item ID '{}'", userID, itemID);
                return p.recommender.EstimatePreference(userID, itemID);
            }
        }

        private sealed class Recommendations
        {
            private List<IRecommendedItem> items;
            private bool noMoreRecommendableItems;

            internal Recommendations(List<IRecommendedItem> items)
            {
                this.items = items;
            }

            public List<IRecommendedItem> GetItems()
            {
                return items;
            }

            public bool IsNoMoreRecommendableItems()
            {
                return noMoreRecommendableItems;
            }

            public void SetNoMoreRecommendableItems(bool noMoreRecommendableItems)
            {
                this.noMoreRecommendableItems = noMoreRecommendableItems;
            }
        }
    }
}