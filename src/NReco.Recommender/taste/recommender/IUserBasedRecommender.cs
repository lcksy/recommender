using System;

namespace NReco.CF.Taste.Recommender
{
    /// <summary>
    /// Interface implemented by "user-based" recommenders.
    /// </summary>
    public interface IUserBasedRecommender : IRecommender
    {
        /// <summary>
        /// Get most similar user IDs for specified user ID
        /// </summary>
        /// <param name="userID">ID of user for which to find most similar other users</param>
        /// <param name="howMany">desired number of most similar users to find</param>
        /// <returns>users most similar to the given user</returns>
        long[] MostSimilarUserIDs(long userID, int howMany);

        /// <summary>
        /// Get most similar user IDs for specified user ID and rescorer
        /// </summary>
        /// <param name="userID">ID of user for which to find most similar other users</param>
        /// <param name="howMany">desired number of most similar users to find</param>
        /// <param name="rescorer"><see cref="IRescorer"/> which can adjust user-user similarity estimates used to determine most similar users</param>
        /// <returns>IDs of users most similar to the given user</returns>
        long[] MostSimilarUserIDs(long userID, int howMany, IRescorer<Tuple<long, long>> rescorer);
    }
}