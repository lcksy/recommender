using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Similarity;
using NReco.Math3.Stats;

namespace NReco.CF.Taste.Impl.Similarity
{
    /// <summary>
    /// Similarity test is based on the likelihood ratio, which expresses how many times more likely the data are under one model than the other.
    /// </summary>
    /// <remarks>
    /// See <a href="http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.14.5962">
    /// http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.14.5962</a> and
    /// <a href="http://tdunning.blogspot.com/2008/03/surprise-and-coincidence.html">
    /// http://tdunning.blogspot.com/2008/03/surprise-and-coincidence.html</a>.
    /// </remarks>
    public sealed class LogLikelihoodSimilarity : AbstractItemSimilarity, IUserSimilarity
    {
        public LogLikelihoodSimilarity(IDataModel dataModel)
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
            FastIDSet prefs1 = dataModel.GetItemIDsFromUser(userID1);
            FastIDSet prefs2 = dataModel.GetItemIDsFromUser(userID2);

            long prefs1Size = prefs1.Count();
            long prefs2Size = prefs2.Count();
            long intersectionSize = prefs1Size < prefs2Size ? prefs2.IntersectionSize(prefs1) : prefs1.IntersectionSize(prefs2);
            if (intersectionSize == 0)
            {
                return Double.NaN;
            }
            long numItems = dataModel.GetNumItems();
            double logLikelihood =
                LogLikelihood.LogLikelihoodRatio(intersectionSize,
                                                 prefs2Size - intersectionSize,
                                                 prefs1Size - intersectionSize,
                                                 numItems - prefs1Size - prefs2Size + intersectionSize);
            return 1.0 - 1.0 / (1.0 + logLikelihood);
        }

        public override double ItemSimilarity(long itemID1, long itemID2)
        {
            IDataModel dataModel = GetDataModel();
            long preferring1 = dataModel.GetNumUsersWithPreferenceFor(itemID1);
            long numUsers = dataModel.GetNumUsers();
            return DoItemSimilarity(itemID1, itemID2, preferring1, numUsers);
        }

        public override double[] ItemSimilarities(long itemID1, long[] itemID2s)
        {
            IDataModel dataModel = GetDataModel();
            long preferring1 = dataModel.GetNumUsersWithPreferenceFor(itemID1);
            long numUsers = dataModel.GetNumUsers();
            int length = itemID2s.Length;
            double[] result = new double[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = DoItemSimilarity(itemID1, itemID2s[i], preferring1, numUsers);
            }
            return result;
        }

        private double DoItemSimilarity(long itemID1, long itemID2, long preferring1, long numUsers)
        {
            IDataModel dataModel = GetDataModel();
            long preferring1and2 = dataModel.GetNumUsersWithPreferenceFor(itemID1, itemID2);
            if (preferring1and2 == 0)
            {
                return Double.NaN;
            }
            long preferring2 = dataModel.GetNumUsersWithPreferenceFor(itemID2);
            double logLikelihood =
                LogLikelihood.LogLikelihoodRatio(preferring1and2,
                                                 preferring2 - preferring1and2,
                                                 preferring1 - preferring1and2,
                                                 numUsers - preferring1 - preferring2 + preferring1and2);
            return 1.0 - 1.0 / (1.0 + logLikelihood);
        }

        public void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            alreadyRefreshed = RefreshHelper.BuildRefreshed(alreadyRefreshed);
            RefreshHelper.MaybeRefresh(alreadyRefreshed, GetDataModel());
        }

        public override string ToString()
        {
            return "LogLikelihoodSimilarity[dataModel:" + GetDataModel() + ']';
        }
    }
}