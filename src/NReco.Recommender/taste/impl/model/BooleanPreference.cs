using System;

using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Model
{
    /// <summary>
    /// Encapsulates a simple bool "preference" for an item whose value does not matter (is fixed at 1.0). This
    /// is appropriate in situations where users conceptually have only a general "yes" preference for items,
    /// rather than a spectrum of preference values.
    /// </summary>
    public sealed class BooleanPreference : IPreference
    {
        private long userID;
        private long itemID;

        public BooleanPreference(long userID, long itemID)
        {
            this.userID = userID;
            this.itemID = itemID;
        }

        public long GetUserID()
        {
            return userID;
        }

        public long GetItemID()
        {
            return itemID;
        }

        public float GetValue()
        {
            return 1.0f;
        }

        public void SetValue(float value)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return "BooleanPreference[userID: " + userID + ", itemID:" + itemID + ']';
        }
    }
}