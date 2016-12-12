using System.Collections.Generic;

using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Similarity.Precompute
{
    /// <summary>
    /// Compact representation of all similar items for an item
    /// </summary>
    public class SimilarItems
    {
        private long itemID;
        private long[] similarItemIDs;
        private double[] similarities;

        public SimilarItems(long itemID, List<IRecommendedItem> similarItems)
        {
            this.itemID = itemID;

            int numSimilarItems = similarItems.Count;
            similarItemIDs = new long[numSimilarItems];
            similarities = new double[numSimilarItems];

            for (int n = 0; n < numSimilarItems; n++)
            {
                similarItemIDs[n] = similarItems[n].GetItemID();
                similarities[n] = similarItems[n].GetValue();
            }
        }

        public long GetItemID()
        {
            return itemID;
        }

        public int NumSimilarItems()
        {
            return similarItemIDs.Length;
        }

        public IEnumerable<SimilarItem> GetSimilarItems()
        {
            for (int index = 0; index < similarItemIDs.Length; index++)
                yield return new SimilarItem(similarItemIDs[index], similarities[index]);
        }
    }
}