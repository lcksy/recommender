
namespace NReco.CF.Taste.Similarity.Precompute
{
    /// <summary>
    /// Used to persist the results of a batch item similarity computation
    /// conducted with a <see cref="BatchItemSimilarities"/> implementation
    /// </summary>
    public interface ISimilarItemsWriter /*: Closeable*/ 
    {
        void Open();

        void Add(SimilarItems similarItems);
    }
}