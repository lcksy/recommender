using System;
using System.Collections.Generic;
using System.Linq;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Similarity;
using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// A simple class that refactors the "find top N things" logic that is used in several places.
    /// </summary>
    public sealed class TopItems
    {
        private static long[] NO_IDS = new long[0];

        private TopItems() { }

        public static List<IRecommendedItem> GetTopItems(int howMany,
                                                        IEnumerator<long> possibleItemIDs,
                                                        IDRescorer rescorer,
                                                        IEstimator<long> estimator)
        {
            //Preconditions.checkArgument(possibleItemIDs != null, "possibleItemIDs is null");
            //Preconditions.checkArgument(estimator != null, "estimator is null");

            var topItems = new SortedSet<IRecommendedItem>(ByValueRecommendedItemComparator.GetReverseInstance());
            bool full = false;
            double lowestTopValue = Double.NegativeInfinity;
            while (possibleItemIDs.MoveNext())
            {
                long itemID = possibleItemIDs.Current;
                if (rescorer == null || !rescorer.IsFiltered(itemID))
                {
                    double preference;
                    try
                    {
                        preference = estimator.Estimate(itemID);
                    }
                    catch (NoSuchItemException)
                    {
                        continue;
                    }
                    double rescoredPref = rescorer == null ? preference : rescorer.Rescore(itemID, preference);
                    if (!Double.IsNaN(rescoredPref) && (!full || rescoredPref > lowestTopValue))
                    {
                        topItems.Add(new GenericRecommendedItem(itemID, (float)rescoredPref));
                        if (full)
                        {
                            topItems.Remove(topItems.Min);
                        }
                        else if (topItems.Count > howMany)
                        {
                            full = true;
                            topItems.Remove(topItems.Min); //     topItems.poll();
                        }
                        lowestTopValue = topItems.Min.GetValue();
                    }
                }
            }
            int size = topItems.Count;
            if (size == 0)
            {
                return new List<IRecommendedItem>();
            }
            List<IRecommendedItem> result = new List<IRecommendedItem>(size);
            result.AddRange(topItems);
            result.Reverse();
            //Collections.sort(result, ByValueRecommendedItemComparator.getInstance());
            return result;
        }

        public static long[] GetTopUsers(int howMany,
                                         IEnumerator<long> allUserIDs,
                                         IDRescorer rescorer,
                                         IEstimator<long> estimator)
        {
            var topUsers = new SortedSet<SimilarUser>();
            bool full = false;
            double lowestTopValue = Double.NegativeInfinity;
            while (allUserIDs.MoveNext())
            {
                long userID = allUserIDs.Current;
                if (rescorer != null && rescorer.IsFiltered(userID))
                {
                    continue;
                }
                double similarity;
                try
                {
                    similarity = estimator.Estimate(userID);
                }
                catch (NoSuchUserException)
                {
                    continue;
                }
                double rescoredSimilarity = rescorer == null ? similarity : rescorer.Rescore(userID, similarity);
                if (!Double.IsNaN(rescoredSimilarity) && (!full || rescoredSimilarity > lowestTopValue))
                {
                    topUsers.Add(new SimilarUser(userID, rescoredSimilarity));
                    if (full)
                    {
                        topUsers.Remove(topUsers.Max); // topUsers.poll();
                    }
                    else if (topUsers.Count > howMany)
                    {
                        full = true;
                        topUsers.Remove(topUsers.Max); // topUsers.poll();
                    }
                    lowestTopValue = topUsers.Max.GetSimilarity();
                }
            }
            int size = topUsers.Count;
            if (size == 0)
            {
                return NO_IDS;
            }
            List<SimilarUser> sorted = new List<SimilarUser>(size);
            return topUsers.Select(s => s.GetUserID()).ToArray();
        }

        /// <p>
        /// Thanks to tsmorton for suggesting this functionality and writing part of the code.
        /// </p>
        /// 
        /// @see GenericItemSimilarity#GenericItemSimilarity(Iterable, int)
        /// @see GenericItemSimilarity#GenericItemSimilarity(NReco.CF.Taste.Similarity.ItemSimilarity,
        ///      NReco.CF.Taste.Model.DataModel, int)
        public static List<GenericItemSimilarity.ItemItemSimilarity> GetTopItemItemSimilarities(
          int howMany, IEnumerator<GenericItemSimilarity.ItemItemSimilarity> allSimilarities)
        {

            var topSimilarities
              = new SortedSet<GenericItemSimilarity.ItemItemSimilarity>();
            bool full = false;
            double lowestTopValue = Double.NegativeInfinity;
            while (allSimilarities.MoveNext())
            {
                GenericItemSimilarity.ItemItemSimilarity similarity = allSimilarities.Current;
                double value = similarity.GetValue();
                if (!Double.IsNaN(value) && (!full || value > lowestTopValue))
                {
                    topSimilarities.Add(similarity);
                    if (full)
                    {
                        topSimilarities.Remove(topSimilarities.Max);  //poll();
                    }
                    else if (topSimilarities.Count > howMany)
                    {
                        full = true;
                        topSimilarities.Remove(topSimilarities.Max);//topSimilarities.poll();
                    }
                    lowestTopValue = topSimilarities.Max.GetValue();
                }
            }
            int size = topSimilarities.Count;
            var result = new List<GenericItemSimilarity.ItemItemSimilarity>(size);
            result.AddRange(topSimilarities);
            return result;
        }

        public static List<GenericUserSimilarity.UserUserSimilarity> GetTopUserUserSimilarities(
          int howMany, IEnumerator<GenericUserSimilarity.UserUserSimilarity> allSimilarities)
        {

            var topSimilarities = new SortedSet<GenericUserSimilarity.UserUserSimilarity>();
            bool full = false;
            double lowestTopValue = Double.NegativeInfinity;
            while (allSimilarities.MoveNext())
            {
                GenericUserSimilarity.UserUserSimilarity similarity = allSimilarities.Current;
                double value = similarity.getValue();
                if (!Double.IsNaN(value) && (!full || value > lowestTopValue))
                {
                    topSimilarities.Add(similarity);
                    if (full)
                    {
                        topSimilarities.Remove(topSimilarities.Max);// topSimilarities.poll();
                    }
                    else if (topSimilarities.Count > howMany)
                    {
                        full = true;
                        topSimilarities.Remove(topSimilarities.Max);//topSimilarities.poll();
                    }
                    lowestTopValue = topSimilarities.Max.getValue();
                }
            }
            int size = topSimilarities.Count;
            var result = new List<GenericUserSimilarity.UserUserSimilarity>(size);
            result.AddRange(topSimilarities);
            return result;
        }

        public interface IEstimator<T>
        {
            double Estimate(T thing);
        }
    }
}