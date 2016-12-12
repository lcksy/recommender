using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Eval
{
    /// <summary>
    /// Evaluate recommender by comparing order of all raw prefs with order in 
    /// recommender's output for that user. Can also compare data models.
    /// </summary>
    public sealed class OrderBasedRecommenderEvaluator
    {
        private static Logger log = LoggerFactory.GetLogger(typeof(OrderBasedRecommenderEvaluator));

        private OrderBasedRecommenderEvaluator()
        {
        }

        public static void Evaluate(IRecommender recommender1,
                                    IRecommender recommender2,
                                    int samples,
                                    IRunningAverage tracker,
                                    String tag)
        {
            PrintHeader();
            var users = recommender1.GetDataModel().GetUserIDs();

            while (users.MoveNext())
            {
                long userID = users.Current;
                var recs1 = recommender1.Recommend(userID, samples);
                var recs2 = recommender2.Recommend(userID, samples);
                FastIDSet commonSet = new FastIDSet();
                long maxItemID = SetBits(commonSet, recs1, samples);
                FastIDSet otherSet = new FastIDSet();
                maxItemID = Math.Max(maxItemID, SetBits(otherSet, recs2, samples));
                int max = Mask(commonSet, otherSet, maxItemID);
                max = Math.Min(max, samples);
                if (max < 2)
                {
                    continue;
                }
                long[] items1 = GetCommonItems(commonSet, recs1, max);
                long[] items2 = GetCommonItems(commonSet, recs2, max);
                double variance = ScoreCommonSubset(tag, userID, samples, max, items1, items2);
                tracker.AddDatum(variance);
            }
        }

        public static void Evaluate(IRecommender recommender,
                                    IDataModel model,
                                    int samples,
                                    IRunningAverage tracker,
                                    String tag)
        {
            PrintHeader();
            var users = recommender.GetDataModel().GetUserIDs();
            while (users.MoveNext())
            {
                long userID = users.Current;
                var recs1 = recommender.Recommend(userID, model.GetNumItems());
                IPreferenceArray prefs2 = model.GetPreferencesFromUser(userID);
                prefs2.SortByValueReversed();
                FastIDSet commonSet = new FastIDSet();
                long maxItemID = SetBits(commonSet, recs1, samples);
                FastIDSet otherSet = new FastIDSet();
                maxItemID = Math.Max(maxItemID, SetBits(otherSet, prefs2, samples));
                int max = Mask(commonSet, otherSet, maxItemID);
                max = Math.Min(max, samples);
                if (max < 2)
                {
                    continue;
                }
                long[] items1 = GetCommonItems(commonSet, recs1, max);
                long[] items2 = GetCommonItems(commonSet, prefs2, max);
                double variance = ScoreCommonSubset(tag, userID, samples, max, items1, items2);
                tracker.AddDatum(variance);
            }
        }

        public static void Evaluate(IDataModel model1,
                                    IDataModel model2,
                                    int samples,
                                    IRunningAverage tracker,
                                    String tag)
        {
            PrintHeader();
            var users = model1.GetUserIDs();
            while (users.MoveNext())
            {
                long userID = users.Current;
                IPreferenceArray prefs1 = model1.GetPreferencesFromUser(userID);
                IPreferenceArray prefs2 = model2.GetPreferencesFromUser(userID);
                prefs1.SortByValueReversed();
                prefs2.SortByValueReversed();
                FastIDSet commonSet = new FastIDSet();
                long maxItemID = SetBits(commonSet, prefs1, samples);
                FastIDSet otherSet = new FastIDSet();
                maxItemID = Math.Max(maxItemID, SetBits(otherSet, prefs2, samples));
                int max = Mask(commonSet, otherSet, maxItemID);
                max = Math.Min(max, samples);
                if (max < 2)
                {
                    continue;
                }
                long[] items1 = GetCommonItems(commonSet, prefs1, max);
                long[] items2 = GetCommonItems(commonSet, prefs2, max);
                double variance = ScoreCommonSubset(tag, userID, samples, max, items1, items2);
                tracker.AddDatum(variance);
            }
        }

        /// This exists because FastIDSet has 'retainAll' as MASK, but there is 
        /// no count of the number of items in the set. size() is supposed to do 
        /// this but does not work.
        private static int Mask(FastIDSet commonSet, FastIDSet otherSet, long maxItemID)
        {
            int count = 0;
            for (int i = 0; i <= maxItemID; i++)
            {
                if (commonSet.Contains(i))
                {
                    if (otherSet.Contains(i))
                    {
                        count++;
                    }
                    else
                    {
                        commonSet.Remove(i);
                    }
                }
            }
            return count;
        }

        private static long[] GetCommonItems(FastIDSet commonSet, IEnumerable<IRecommendedItem> recs, int max)
        {
            long[] commonItems = new long[max];
            int index = 0;
            foreach (IRecommendedItem rec in recs)
            {
                long item = rec.GetItemID();
                if (commonSet.Contains(item))
                {
                    commonItems[index++] = item;
                }
                if (index == max)
                {
                    break;
                }
            }
            return commonItems;
        }

        private static long[] GetCommonItems(FastIDSet commonSet, IPreferenceArray prefs1, int max)
        {
            long[] commonItems = new long[max];
            int index = 0;
            for (int i = 0; i < prefs1.Length(); i++)
            {
                long item = prefs1.GetItemID(i);
                if (commonSet.Contains(item))
                {
                    commonItems[index++] = item;
                }
                if (index == max)
                {
                    break;
                }
            }
            return commonItems;
        }

        private static long SetBits(FastIDSet modelSet, IList<IRecommendedItem> items, int max)
        {
            long maxItem = -1;
            for (int i = 0; i < items.Count && i < max; i++)
            {
                long itemID = items[i].GetItemID();
                modelSet.Add(itemID);
                if (itemID > maxItem)
                {
                    maxItem = itemID;
                }
            }
            return maxItem;
        }

        private static long SetBits(FastIDSet modelSet, IPreferenceArray prefs, int max)
        {
            long maxItem = -1;
            for (int i = 0; i < prefs.Length() && i < max; i++)
            {
                long itemID = prefs.GetItemID(i);
                modelSet.Add(itemID);
                if (itemID > maxItem)
                {
                    maxItem = itemID;
                }
            }
            return maxItem;
        }

        private static void PrintHeader()
        {
            log.Info("tag,user,samples,common,hamming,bubble,rank,normal,score");
        }

        /// Common Subset Scoring
        ///
        /// These measurements are given the set of results that are common to both
        /// recommendation lists. They only get ordered lists.
        ///
        /// These measures all return raw numbers do not correlate among the tests.
        /// The numbers are not corrected against the total number of samples or the
        /// number of common items.
        /// The one contract is that all measures are 0 for an exact match and an
        /// increasing positive number as differences increase.
        private static double ScoreCommonSubset(String tag,
                                                long userID,
                                                int samples,
                                                int subset,
                                                long[] itemsL,
                                                long[] itemsR)
        {
            int[] vectorZ = new int[subset];
            int[] vectorZabs = new int[subset];

            long bubble = Sort(itemsL, itemsR);
            int hamming = SlidingWindowHamming(itemsR, itemsL);
            if (hamming > samples)
            {
                throw new InvalidOperationException();
            }
            GetVectorZ(itemsR, itemsL, vectorZ, vectorZabs);
            double normalW = NormalWilcoxon(vectorZ, vectorZabs);
            double meanRank = GetMeanRank(vectorZabs);
            // case statement for requested value
            double variance = Math.Sqrt(meanRank);
            log.Info("{},{},{},{},{},{},{},{},{}",
                     tag, userID, samples, subset, hamming, bubble, meanRank, normalW, variance);
            return variance;
        }

        // simple sliding-window hamming distance: a[i or plus/minus 1] == b[i]
        private static int SlidingWindowHamming(long[] itemsR, long[] itemsL)
        {
            int count = 0;
            int samples = itemsR.Length;

            if (itemsR[0].Equals(itemsL[0]) || itemsR[0].Equals(itemsL[1]))
            {
                count++;
            }
            for (int i = 1; i < samples - 1; i++)
            {
                long itemID = itemsL[i];
                if (itemsR[i] == itemID || itemsR[i - 1] == itemID || itemsR[i + 1] == itemID)
                {
                    count++;
                }
            }
            if (itemsR[samples - 1].Equals(itemsL[samples - 1]) || itemsR[samples - 1].Equals(itemsL[samples - 2]))
            {
                count++;
            }
            return count;
        }

        /// Normal-distribution probability value for matched sets of values.
        /// Based upon:
        /// http://comp9.psych.cornell.edu/Darlington/normscor.htm
        /// 
        /// The Standard Wilcoxon is not used because it requires a lookup table.
        static double NormalWilcoxon(int[] vectorZ, int[] vectorZabs)
        {
            int nitems = vectorZ.Length;

            double[] ranks = new double[nitems];
            double[] ranksAbs = new double[nitems];
            WilcoxonRanks(vectorZ, vectorZabs, ranks, ranksAbs);
            return Math.Min(GetMeanWplus(ranks), GetMeanWminus(ranks));
        }

        /// vector Z is a list of distances between the correct value and the recommended value
        /// Z[i] = position i of correct itemID - position of correct itemID in recommendation list
        /// can be positive or negative
        /// the smaller the better - means recommendations are closer
        /// both are the same length, and both sample from the same set
        /// 
        /// destructive to items arrays - allows N log N instead of N^2 order
        private static void GetVectorZ(long[] itemsR, long[] itemsL, int[] vectorZ, int[] vectorZabs)
        {
            var itemsL_isNull = new bool[itemsL.Length];
            for (int i = 0; i < itemsL_isNull.Length; i++) itemsL_isNull[i] = false;

            int nitems = itemsR.Length;
            int bottom = 0;
            int top = nitems - 1;
            for (int i = 0; i < nitems; i++)
            {
                long itemID = itemsR[i];
                for (int j = bottom; j <= top; j++)
                {
                    if (itemsL_isNull[j])
                    { //if (itemsL[j] == null) {
                        continue;
                    }
                    long test = itemsL[j];
                    if (itemID == test)
                    {
                        vectorZ[i] = i - j;
                        vectorZabs[i] = Math.Abs(i - j);
                        if (j == bottom)
                        {
                            bottom++;
                        }
                        else if (j == top)
                        {
                            top--;
                        }
                        else
                        {
                            itemsL_isNull[j] = true; //itemsL[j] = null;
                        }
                        break;
                    }
                }
            }
        }

        /// Ranks are the position of the value from low to high, divided by the # of values.
        /// I had to walk through it a few times.
        private static void WilcoxonRanks(int[] vectorZ, int[] vectorZabs, double[] ranks, double[] ranksAbs)
        {
            int nitems = vectorZ.Length;
            int[] sorted = (int[])vectorZabs.Clone();
            Array.Sort(sorted);
            int zeros = 0;
            for (; zeros < nitems; zeros++)
            {
                if (sorted[zeros] > 0)
                {
                    break;
                }
            }
            for (int i = 0; i < nitems; i++)
            {
                double rank = 0.0;
                int count = 0;
                int score = vectorZabs[i];
                for (int j = 0; j < nitems; j++)
                {
                    if (score == sorted[j])
                    {
                        rank += j + 1 - zeros;
                        count++;
                    }
                    else if (score < sorted[j])
                    {
                        break;
                    }
                }
                if (vectorZ[i] != 0)
                {
                    ranks[i] = (rank / count) * (vectorZ[i] < 0 ? -1 : 1);  // better be at least 1
                    ranksAbs[i] = Math.Abs(ranks[i]);
                }
            }
        }

        private static double GetMeanRank(int[] ranks)
        {
            int nitems = ranks.Length;
            double sum = 0.0;
            foreach (int rank in ranks)
            {
                sum += rank;
            }
            return sum / nitems;
        }

        private static double GetMeanWplus(double[] ranks)
        {
            int nitems = ranks.Length;
            double sum = 0.0;
            foreach (double rank in ranks)
            {
                if (rank > 0)
                {
                    sum += rank;
                }
            }
            return sum / nitems;
        }

        private static double GetMeanWminus(double[] ranks)
        {
            int nitems = ranks.Length;
            double sum = 0.0;
            foreach (double rank in ranks)
            {
                if (rank < 0)
                {
                    sum -= rank;
                }
            }
            return sum / nitems;
        }

        /// Do bubble sort and return number of swaps needed to match preference lists.
        /// Sort itemsR using itemsL as the reference order.
        static long Sort(long[] itemsL, long[] itemsR)
        {
            int length = itemsL.Length;
            if (length < 2)
            {
                return 0;
            }
            if (length == 2)
            {
                return itemsL[0] == itemsR[0] ? 0 : 1;
            }
            // 1) avoid changing originals; 2) primitive type is more efficient
            long[] reference = new long[length];
            long[] sortable = new long[length];
            for (int i = 0; i < length; i++)
            {
                reference[i] = itemsL[i];
                sortable[i] = itemsR[i];
            }
            int sorted = 0;
            long swaps = 0;
            while (sorted < length - 1)
            {
                // opportunistically trim back the top
                while (length > 0 && reference[length - 1] == sortable[length - 1])
                {
                    length--;
                }
                if (length == 0)
                {
                    break;
                }
                if (reference[sorted] == sortable[sorted])
                {
                    sorted++;
                }
                else
                {
                    for (int j = sorted; j < length - 1; j++)
                    {
                        // do not swap anything already in place
                        int jump = 1;
                        if (reference[j] == sortable[j])
                        {
                            while (j + jump < length && reference[j + jump] == sortable[j + jump])
                            {
                                jump++;
                            }
                        }
                        if (j + jump < length && !(reference[j] == sortable[j] && reference[j + jump] == sortable[j + jump]))
                        {
                            long tmp = sortable[j];
                            sortable[j] = sortable[j + 1];
                            sortable[j + 1] = tmp;
                            swaps++;
                        }
                    }
                }
            }
            return swaps;
        }
    }
}