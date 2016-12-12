using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Similarity
{
    /// <summary>Caches the results from an underlying <see cref="IUserSimilarity"/> implementation.</summary>
    public sealed class CachingUserSimilarity : IUserSimilarity
    {
        private IUserSimilarity similarity;
        private Cache<Tuple<long, long>, Double> similarityCache;
        private RefreshHelper refreshHelper;

        /// Creates this on top of the given {@link UserSimilarity}.
        /// The cache is sized according to properties of the given {@link DataModel}.
        public CachingUserSimilarity(IUserSimilarity similarity, IDataModel dataModel)
            : this(similarity, dataModel.GetNumUsers())
        {
        }

        /// Creates this on top of the given {@link UserSimilarity}.
        /// The cache size is capped by the given size.
        public CachingUserSimilarity(IUserSimilarity similarity, int maxCacheSize)
        {
            //Preconditions.checkArgument(similarity != null, "similarity is null");
            this.similarity = similarity;
            this.similarityCache = new Cache<Tuple<long, long>, Double>(new SimilarityRetriever(similarity), maxCacheSize);
            this.refreshHelper = new RefreshHelper(() =>
            {
                similarityCache.Clear();
            });
            refreshHelper.AddDependency(similarity);
        }

        public double UserSimilarity(long userID1, long userID2)
        {
            Tuple<long, long> key = userID1 < userID2 ? new Tuple<long, long>(userID1, userID2) : new Tuple<long, long>(userID2, userID1);
            return similarityCache.Get(key);
        }

        public void SetPreferenceInferrer(IPreferenceInferrer inferrer)
        {
            similarityCache.Clear();
            similarity.SetPreferenceInferrer(inferrer);
        }

        public void ClearCacheForUser(long userID)
        {
            similarityCache.RemoveKeysMatching(new LongPairMatchPredicate(userID).Matches);
        }

        public void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            refreshHelper.Refresh(alreadyRefreshed);
        }

        private sealed class SimilarityRetriever : IRetriever<Tuple<long, long>, Double>
        {
            private IUserSimilarity similarity;

            internal SimilarityRetriever(IUserSimilarity similarity)
            {
                this.similarity = similarity;
            }

            public Double Get(Tuple<long, long> key)
            {
                return similarity.UserSimilarity(key.Item1, key.Item2);
            }
        }
    }
}