using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Similarity
{
    /// <summary>
    /// Implementation of City Block distance (also known as Manhattan distance) - the absolute value of the difference of
    /// each direction is summed.  The resulting unbounded distance is then mapped between 0 and 1.
    /// </summary>
    public sealed class CityBlockSimilarity : AbstractItemSimilarity, IUserSimilarity
    {
        public CityBlockSimilarity(IDataModel dataModel)
            : base(dataModel)
        {
        }

        /// @throws NotSupportedException
        public void SetPreferenceInferrer(IPreferenceInferrer inferrer)
        {
            throw new NotSupportedException();
        }

        public void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            var refreshed = RefreshHelper.BuildRefreshed(alreadyRefreshed);
            RefreshHelper.MaybeRefresh(refreshed, GetDataModel());
        }

        public override double ItemSimilarity(long itemID1, long itemID2)
        {
            IDataModel dataModel = GetDataModel();
            int preferring1 = dataModel.GetNumUsersWithPreferenceFor(itemID1);
            int preferring2 = dataModel.GetNumUsersWithPreferenceFor(itemID2);
            int intersection = dataModel.GetNumUsersWithPreferenceFor(itemID1, itemID2);
            return DoSimilarity(preferring1, preferring2, intersection);
        }

        public override double[] ItemSimilarities(long itemID1, long[] itemID2s)
        {
            IDataModel dataModel = GetDataModel();
            int preferring1 = dataModel.GetNumUsersWithPreferenceFor(itemID1);
            double[] distance = new double[itemID2s.Length];
            for (int i = 0; i < itemID2s.Length; ++i)
            {
                int preferring2 = dataModel.GetNumUsersWithPreferenceFor(itemID2s[i]);
                int intersection = dataModel.GetNumUsersWithPreferenceFor(itemID1, itemID2s[i]);
                distance[i] = DoSimilarity(preferring1, preferring2, intersection);
            }
            return distance;
        }

        public double UserSimilarity(long userID1, long userID2)
        {
            IDataModel dataModel = GetDataModel();
            FastIDSet prefs1 = dataModel.GetItemIDsFromUser(userID1);
            FastIDSet prefs2 = dataModel.GetItemIDsFromUser(userID2);
            int prefs1Size = prefs1.Count();
            int prefs2Size = prefs2.Count();
            int intersectionSize = prefs1Size < prefs2Size ? prefs2.IntersectionSize(prefs1) : prefs1.IntersectionSize(prefs2);
            return DoSimilarity(prefs1Size, prefs2Size, intersectionSize);
        }

        /// Calculate City Block Distance from total non-zero values and intersections and map to a similarity value.
        ///
        /// @param pref1        number of non-zero values in left vector
        /// @param pref2        number of non-zero values in right vector
        /// @param intersection number of overlapping non-zero values
        private static double DoSimilarity(int pref1, int pref2, int intersection)
        {
            int distance = pref1 + pref2 - 2 * intersection;
            return 1.0 / (1.0 + distance);
        }
    }
}