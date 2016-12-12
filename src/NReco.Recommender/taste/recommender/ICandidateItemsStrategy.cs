using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Recommender
{
    /// <summary>Used to retrieve all items that could possibly be recommended to the user</summary>
    public interface ICandidateItemsStrategy : IRefreshable
    {
        /// <summary>
        /// Get IDs of all items that could be recommended to the user
        /// </summary>
        FastIDSet GetCandidateItems(long userID, IPreferenceArray preferencesFromUser, IDataModel dataModel);
    }
}