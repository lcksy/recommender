using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// Returns all items that have not been rated by the user and that were preferred by another user
    /// that has preferred at least one item that the current user has preferred too.
    /// </summary>
    public sealed class PreferredItemsNeighborhoodCandidateItemsStrategy : AbstractCandidateItemsStrategy
    {
        protected override FastIDSet DoGetCandidateItems(long[] preferredItemIDs, IDataModel dataModel)
        {
            FastIDSet possibleItemsIDs = new FastIDSet();
            foreach (long itemID in preferredItemIDs)
            {
                IPreferenceArray itemPreferences = dataModel.GetPreferencesForItem(itemID);
                int numUsersPreferringItem = itemPreferences.Length();
                for (int index = 0; index < numUsersPreferringItem; index++)
                {
                    possibleItemsIDs.AddAll(dataModel.GetItemIDsFromUser(itemPreferences.GetUserID(index)));
                }
            }
            possibleItemsIDs.RemoveAll(preferredItemIDs);
            return possibleItemsIDs;
        }
    }
}