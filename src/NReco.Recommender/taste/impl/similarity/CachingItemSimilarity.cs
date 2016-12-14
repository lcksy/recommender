using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Similarity
{
    /// <summary>Caches the results from an underlying <see cref="IItemSimilarity"/> implementation.</summary>
    public sealed class CachingItemSimilarity : IItemSimilarity
    {
        private IItemSimilarity similarity;
        private Cache<Tuple<long, long>, Double> similarityCache;
        private RefreshHelper refreshHelper;

        /// Creates this on top of the given {@link ItemSimilarity}.
        /// The cache is sized according to properties of the given {@link DataModel}.
        public CachingItemSimilarity(IItemSimilarity similarity, IDataModel dataModel)
            : this(similarity, dataModel.GetNumItems())
        {
            ;
        }

        /// Creates this on top of the given {@link ItemSimilarity}.
        /// The cache size is capped by the given size.
        public CachingItemSimilarity(IItemSimilarity similarity, int maxCacheSize)
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

        public double ItemSimilarity(long itemID1, long itemID2)
        {
            Tuple<long, long> key = itemID1 < itemID2 ? new Tuple<long, long>(itemID1, itemID2) : new Tuple<long, long>(itemID2, itemID1);
            return similarityCache.Get(key);
        }

        public double[] ItemSimilarities(long itemID1, long[] itemID2s)
        {
            int length = itemID2s.Length;
            double[] result = new double[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ItemSimilarity(itemID1, itemID2s[i]);
            }
            return result;
        }

        public long[] AllSimilarItemIDs(long itemID)
        {
            return similarity.AllSimilarItemIDs(itemID);
        }

        public void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            refreshHelper.Refresh(alreadyRefreshed);
        }

        public void ClearCacheForItem(long itemID)
        {
            similarityCache.RemoveKeysMatching(new LongPairMatchPredicate(itemID).Matches);
        }

        private sealed class SimilarityRetriever : IRetriever<Tuple<long, long>, Double>
        {
            private IItemSimilarity similarity;

            internal SimilarityRetriever(IItemSimilarity similarity)
            {
                this.similarity = similarity;
            }

            public Double Get(Tuple<long, long> key)
            {
                return similarity.ItemSimilarity(key.Item1, key.Item2);
            }
        }
    }
}