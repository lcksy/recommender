using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Model
{
    /// <summary>
    /// Contains some features common to all implementations.
    /// </summary>
    [Serializable]
    public abstract class AbstractDataModel : IDataModel
    {
        private float maxPreference;
        private float minPreference;

        protected AbstractDataModel()
        {
            maxPreference = float.NaN;
            minPreference = float.NaN;
        }

        public abstract IEnumerator<long> GetUserIDs();

        public abstract IPreferenceArray GetPreferencesFromUser(long userID);

        public abstract FastIDSet GetItemIDsFromUser(long userID);

        public abstract IEnumerator<long> GetItemIDs();

        public abstract void Refresh(IList<IRefreshable> alreadyRefreshed);

        public abstract IPreferenceArray GetPreferencesForItem(long itemID);

        public abstract int GetNumItems();

        public abstract int GetNumUsers();

        public abstract int GetNumUsersWithPreferenceFor(long itemID);

        public abstract int GetNumUsersWithPreferenceFor(long itemID1, long itemID2);

        public abstract float? GetPreferenceValue(long userID, long itemID);

        public abstract DateTime? GetPreferenceTime(long userID, long itemID);

        public abstract bool HasPreferenceValues();

        public abstract void SetPreference(long userID, long itemID, float value);

        public abstract void RemovePreference(long userID, long itemID);

        public virtual float GetMaxPreference()
        {
            return maxPreference;
        }

        protected virtual void SetMaxPreference(float maxPreference)
        {
            this.maxPreference = maxPreference;
        }

        public virtual float GetMinPreference()
        {
            return minPreference;
        }

        protected virtual void SetMinPreference(float minPreference)
        {
            this.minPreference = minPreference;
        }
    }
}