using NReco.CF.Taste.Common;

namespace NReco.CF.Taste.Similarity
{
    /// <summary>
    /// Implementations of this interface define a notion of similarity between two items. Implementations should
    /// return values in the range -1.0 to 1.0, with 1.0 representing perfect similarity.
    /// </summary>
    /// <see cref="IUserSimilarity"/>
    public interface IItemSimilarity : IRefreshable
    {
        /// <summary>
        /// Returns the degree of similarity, of two items, based on the preferences that users have expressed for
        /// the items.
        /// </summary>
        /// <param name="itemID1">first item ID</param>
        /// <param name="itemID2">second item ID</param>
        /// <returns>similarity between the items, in [-1,1] or {@link Double#NaN} similarity is unknown</returns>
        /// <remarks>
        /// Throws NReco.CF.Taste.Common.NoSuchItemException if either item is known to be non-existent in the data.
        /// Throws TasteException if an error occurs while accessing the data.
        /// </remarks>
        double ItemSimilarity(long itemID1, long itemID2);

        /// <summary>
        /// A bulk-get version of <see cref="ItemSimilarity(long, long)"/>.
        /// </summary>
        /// <param name="itemID1">first item ID</param>
        /// <param name="itemID2s">second item IDs to compute similarity with</param>
        /// <returns>similarity between itemID1 and other items</returns>
        double[] ItemSimilarities(long itemID1, long[] itemID2s);

        /// <summary>
        /// Return all similar item IDs
        /// </summary>
        /// <returns>all IDs of similar items, in no particular order</returns>
        long[] AllSimilarItemIDs(long itemID);
    }
}