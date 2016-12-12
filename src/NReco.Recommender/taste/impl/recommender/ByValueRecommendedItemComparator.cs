using System.Collections.Generic;

using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>Defines a natural ordering from most-preferred item (highest value) to least-preferred.</summary>
    public sealed class ByValueRecommendedItemComparator : IComparer<IRecommendedItem>
    {
        private static IComparer<IRecommendedItem> INSTANCE = new ByValueRecommendedItemComparator();

        public static IComparer<IRecommendedItem> GetInstance()
        {
            return INSTANCE;
        }

        public static IComparer<IRecommendedItem> GetReverseInstance()
        {
            return new ReverseComparer<IRecommendedItem>(INSTANCE);
        }

        public int Compare(IRecommendedItem o1, IRecommendedItem o2)
        {
            float value1 = o1.GetValue();
            float value2 = o2.GetValue();
            return value1 > value2 ? -1 : value1 < value2 ? 1 : (o1.GetItemID().CompareTo(o2.GetItemID())); // SortedSet uses IComparer to find identical elements
        }

        internal class ReverseComparer<T> : IComparer<T>
        {
            IComparer<T> comparer;
            internal ReverseComparer(IComparer<T> comparer)
            {
                this.comparer = comparer;
            }
            public int Compare(T obj1, T obj2)
            {
                return -comparer.Compare(obj1, obj2);
            }
        }
    }
}