using NReco.CF.Taste.Common;

namespace NReco.CF.Taste.Similarity
{
    /// <summary>
    /// Implementations of this interface compute an inferred preference for a user and an item that the user has
    /// not expressed any preference for. This might be an average of other preferences scores from that user, for
    /// example. This technique is sometimes called "default voting".
    /// </summary>
    public interface IPreferenceInferrer : IRefreshable
    {
        /// <summary>
        /// Infers the given user's preference value for an item.
        /// </summary>
        /// <param name="userID">ID of user to infer preference for</param>
        /// <param name="itemID">item ID to infer preference for</param>
        /// <returns>inferred preference</returns>
        float InferPreference(long userID, long itemID);
    }
}