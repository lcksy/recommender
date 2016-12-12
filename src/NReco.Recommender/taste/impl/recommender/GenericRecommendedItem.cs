using System;

using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// A simple implementation of <see cref="IRecommendedItem"/>.
    /// </summary>
    [Serializable]
    public sealed class GenericRecommendedItem : IRecommendedItem
    {
        private long itemID;
        private float value;

        /// @throws IllegalArgumentException
        ///           if item is null or value is NaN
        public GenericRecommendedItem(long itemID, float value)
        {
            //Preconditions.checkArgument(!Float.isNaN(value), "value is NaN");
            this.itemID = itemID;
            this.value = value;
        }

        public long GetItemID()
        {
            return itemID;
        }

        public float GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            return "RecommendedItem[item:" + itemID + ", value:" + value + ']';
        }

        public override int GetHashCode()
        {
            return (int)itemID ^ RandomUtils.hashFloat(value);
        }

        public override bool Equals(object o)
        {
            if (!(o is GenericRecommendedItem))
            {
                return false;
            }
            IRecommendedItem other = (IRecommendedItem)o;
            return itemID == other.GetItemID() && value == other.GetValue();
        }
    }
}