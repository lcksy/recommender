using System;

using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Model
{
    /// <summary>
    /// A simple {@link Preference} encapsulating an item and preference value.
    /// </summary>
    [Serializable]
    public class GenericPreference : IPreference
    {
        private long userID;
        private long itemID;
        private float value;

        public GenericPreference(long userID, long itemID, float value)
        {
            //Preconditions.checkArgument(!Float.isNaN(value), "NaN value");
            this.userID = userID;
            this.itemID = itemID;
            this.value = value;
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
            return value;
        }

        public void SetValue(float value)
        {
            //Preconditions.checkArgument(!Float.isNaN(value), "NaN value");
            this.value = value;
        }

        public override string ToString()
        {
            return "GenericPreference[userID: " + userID + ", itemID:" + itemID + ", value:" + value + ']';
        }
    }
}