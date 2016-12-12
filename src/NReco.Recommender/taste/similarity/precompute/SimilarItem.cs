
namespace NReco.CF.Taste.Similarity.Precompute
{
    /// <summary>
    /// Modeling similarity towards another item
    /// </summary>
    public class SimilarItem
    {
        public static int COMPARE_BY_SIMILARITY(SimilarItem x, SimilarItem y)
        {
            return x.similarity.CompareTo(y.similarity);
        }

        private long itemID;
        private double similarity;

        public SimilarItem(long itemID, double similarity)
        {
            set(itemID, similarity);
        }

        public void set(long itemID, double similarity)
        {
            this.itemID = itemID;
            this.similarity = similarity;
        }

        public long getItemID()
        {
            return itemID;
        }

        public double getSimilarity()
        {
            return similarity;
        }
    }
}