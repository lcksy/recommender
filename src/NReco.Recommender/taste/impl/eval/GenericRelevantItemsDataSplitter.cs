using System.Collections.Generic;

using NReco.CF.Taste.Eval;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Impl.Model;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Eval
{
    /// <summary>
    /// Picks relevant items to be those with the strongest preference, and
    /// includes the other users' preferences in full.
    /// </summary>
    public sealed class GenericRelevantItemsDataSplitter : IRelevantItemsDataSplitter
    {

        public FastIDSet GetRelevantItemsIDs(long userID,
                                             int at,
                                             double relevanceThreshold,
                                             IDataModel dataModel)
        {
            IPreferenceArray prefs = dataModel.GetPreferencesFromUser(userID);
            FastIDSet relevantItemIDs = new FastIDSet(at);
            prefs.SortByValueReversed();
            for (int i = 0; i < prefs.Length() && relevantItemIDs.Count() < at; i++)
            {
                if (prefs.GetValue(i) >= relevanceThreshold)
                {
                    relevantItemIDs.Add(prefs.GetItemID(i));
                }
            }
            return relevantItemIDs;
        }

        public void ProcessOtherUser(long userID,
                                     FastIDSet relevantItemIDs,
                                     FastByIDMap<IPreferenceArray> trainingUsers,
                                     long otherUserID,
                                     IDataModel dataModel)
        {
            IPreferenceArray prefs2Array = dataModel.GetPreferencesFromUser(otherUserID);
            // If we're dealing with the very user that we're evaluating for precision/recall,
            if (userID == otherUserID)
            {
                // then must remove all the test IDs, the "relevant" item IDs
                List<IPreference> prefs2 = new List<IPreference>(prefs2Array.Length());
                foreach (IPreference pref in prefs2Array)
                {
                    if (!relevantItemIDs.Contains(pref.GetItemID()))
                    {
                        prefs2.Add(pref);
                    }
                }

                if (prefs2.Count > 0)
                {
                    trainingUsers.Put(otherUserID, new GenericUserPreferenceArray(prefs2));
                }
            }
            else
            {
                // otherwise just add all those other user's prefs
                trainingUsers.Put(otherUserID, prefs2Array);
            }
        }
    }
}