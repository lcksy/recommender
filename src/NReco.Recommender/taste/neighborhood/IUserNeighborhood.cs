using NReco.CF.Taste.Common;

namespace NReco.CF.Taste.Neighborhood
{
    /// <summary>
    /// Implementations of this interface compute a "neighborhood" of users like a given user. This neighborhood
    /// can be used to compute recommendations then.
    /// </summary>
    public interface IUserNeighborhood : IRefreshable
    {
        /// <summary>
        /// Get IDs of users in the neighborhood to specified user ID
        /// </summary>
        /// <param name="userID">ID of user for which a neighborhood will be computed</param>
        /// <returns>IDs of users in the neighborhood</returns>
        long[] GetUserNeighborhood(long userID);
    }
}