using System.Collections.Generic;

using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// Defines ordering on <see cref="IRecommendedItem"/> by the rescored value of the recommendations' estimated
    /// preference value, from high to low.
    /// </summary>
    public sealed class ByRescoreComparator : IComparer<IRecommendedItem>
    {
        private IDRescorer rescorer;

        public ByRescoreComparator(IDRescorer rescorer)
        {
            this.rescorer = rescorer;
        }

        public int Compare(IRecommendedItem o1, IRecommendedItem o2)
        {
            double rescored1;
            double rescored2;
            if (rescorer == null)
            {
                rescored1 = o1.GetValue();
                rescored2 = o2.GetValue();
            }
            else
            {
                rescored1 = rescorer.Rescore(o1.GetItemID(), o1.GetValue());
                rescored2 = rescorer.Rescore(o2.GetItemID(), o2.GetValue());
            }
            if (rescored1 < rescored2)
            {
                return 1;
            }
            else if (rescored1 > rescored2)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public override string ToString()
        {
            return "ByRescoreComparator[rescorer:" + rescorer + ']';
        }
    }
}