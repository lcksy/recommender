using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Similarity
{
    /// <summary>
    /// An implementation of a "similarity" based on the <a
    /// href="http://en.wikipedia.org/wiki/Jaccard_index#Tanimoto_coefficient_.28extended_Jaccard_coefficient.29">
    /// Tanimoto coefficient</a>, or extended <a href="http://en.wikipedia.org/wiki/Jaccard_index">Jaccard
    /// coefficient</a>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is intended for "binary" data sets where a user either expresses a generic "yes" preference for an
    /// item or has no preference. The actual preference values do not matter here, only their presence or absence.
    /// </para>
    /// 
    /// <para>
    /// The value returned is in [0,1].
    /// </para>
    /// </remarks>
    public sealed class TanimotoCoefficientSimilarity : AbstractItemSimilarity, IUserSimilarity
    {
        public TanimotoCoefficientSimilarity(IDataModel dataModel)
            : base(dataModel)
        {
        }

        /// @throws NotSupportedException
        public void SetPreferenceInferrer(IPreferenceInferrer inferrer)
        {
            throw new NotSupportedException();
        }

        public double UserSimilarity(long userID1, long userID2)
        {

            IDataModel dataModel = GetDataModel();
            FastIDSet xPrefs = dataModel.GetItemIDsFromUser(userID1);
            FastIDSet yPrefs = dataModel.GetItemIDsFromUser(userID2);

            int xPrefsSize = xPrefs.Count();
            int yPrefsSize = yPrefs.Count();
            if (xPrefsSize == 0 && yPrefsSize == 0)
            {
                return Double.NaN;
            }
            if (xPrefsSize == 0 || yPrefsSize == 0)
            {
                return 0.0;
            }

            int intersectionSize =
                xPrefsSize < yPrefsSize ? yPrefs.IntersectionSize(xPrefs) : xPrefs.IntersectionSize(yPrefs);
            if (intersectionSize == 0)
            {
                return Double.NaN;
            }

            int unionSize = xPrefsSize + yPrefsSize - intersectionSize;

            return (double)intersectionSize / (double)unionSize;
        }

        public override double ItemSimilarity(long itemID1, long itemID2)
        {
            int preferring1 = GetDataModel().GetNumUsersWithPreferenceFor(itemID1);
            return DoItemSimilarity(itemID1, itemID2, preferring1);
        }

        public override double[] ItemSimilarities(long itemID1, long[] itemID2s)
        {
            int preferring1 = GetDataModel().GetNumUsersWithPreferenceFor(itemID1);
            int length = itemID2s.Length;
            double[] result = new double[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = DoItemSimilarity(itemID1, itemID2s[i], preferring1);
            }
            return result;
        }

        private double DoItemSimilarity(long itemID1, long itemID2, int preferring1)
        {
            IDataModel dataModel = GetDataModel();
            int preferring1and2 = dataModel.GetNumUsersWithPreferenceFor(itemID1, itemID2);
            if (preferring1and2 == 0)
            {
                return Double.NaN;
            }
            int preferring2 = dataModel.GetNumUsersWithPreferenceFor(itemID2);
            return (double)preferring1and2 / (double)(preferring1 + preferring2 - preferring1and2);
        }

        public void Refresh(IList<IRefreshable> AlreadyRefreshed)
        {
            AlreadyRefreshed = RefreshHelper.BuildRefreshed(AlreadyRefreshed);
            RefreshHelper.MaybeRefresh(AlreadyRefreshed, GetDataModel());
        }

        public override string ToString()
        {
            return "TanimotoCoefficientSimilarity[dataModel:" + GetDataModel() + ']';
        }
    }
}