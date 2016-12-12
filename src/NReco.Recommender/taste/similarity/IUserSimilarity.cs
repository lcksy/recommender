using NReco.CF.Taste.Common;

namespace NReco.CF.Taste.Similarity
{
    /// <summary>
    /// Implementations of this interface define a notion of similarity between two users. Implementations should
    /// return values in the range -1.0 to 1.0, with 1.0 representing perfect similarity.
    /// </summary>
    /// <see cref="IItemSimilarity"/>
    public interface IUserSimilarity : IRefreshable
    {
        /// <summary>
        /// Returns the degree of similarity, of two users, based on the their preferences.
        /// </summary>
        /// <param name="userID1">first user ID</param>
        /// <param name="userID2">second user ID</param>
        /// <returns>similarity between the users, in [-1,1] or Double.NaN if similarity is unknown</returns>
        /// <remarks>
        /// Throws NReco.CF.Taste.Common.NoSuchUserException if either user is known to be non-existent in the data.
        /// Throws TasteException if an error occurs while accessing the data.
        /// </remarks>
        double UserSimilarity(long userID1, long userID2);

        // Should we implement userSimilarities() like ItemSimilarity.itemSimilarities()?

        /// <summary>
        /// Attaches a <see cref="IPreferenceInferrer"/> to the <see cref="IUserSimilarity"/> implementation.
        /// </summary>
        /// <param name="inferrer">inferrer to set</param>
        void SetPreferenceInferrer(IPreferenceInferrer inferrer);
    }
}