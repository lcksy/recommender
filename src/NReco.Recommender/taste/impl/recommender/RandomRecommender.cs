using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// Produces random recommendations and preference estimates. This is likely only useful as a novelty and for benchmarking.
    /// </summary>
    public sealed class RandomRecommender : AbstractRecommender
    {
        private RandomWrapper random = RandomUtils.getRandom();
        private float minPref;
        private float maxPref;

        public RandomRecommender(IDataModel dataModel)
            : base(dataModel)
        {
            float maxPref = float.NegativeInfinity;
            float minPref = float.PositiveInfinity;
            var userIterator = dataModel.GetUserIDs();
            while (userIterator.MoveNext())
            {
                long userID = userIterator.Current;
                IPreferenceArray prefs = dataModel.GetPreferencesFromUser(userID);
                for (int i = 0; i < prefs.Length(); i++)
                {
                    float prefValue = prefs.GetValue(i);
                    if (prefValue < minPref)
                    {
                        minPref = prefValue;
                    }
                    if (prefValue > maxPref)
                    {
                        maxPref = prefValue;
                    }
                }
            }
            this.minPref = minPref;
            this.maxPref = maxPref;
        }

        public override IList<IRecommendedItem> Recommend(long userID, int howMany, IDRescorer rescorer)
        {
            IDataModel dataModel = GetDataModel();
            int numItems = dataModel.GetNumItems();
            List<IRecommendedItem> result = new List<IRecommendedItem>(howMany);
            while (result.Count < howMany)
            {
                var it = dataModel.GetItemIDs();
                it.MoveNext();

                var skipNum = random.nextInt(numItems);
                for (int i = 0; i < skipNum; i++)
                    if (!it.MoveNext()) { break; }  // skip() ??

                long itemID = it.Current;
                if (dataModel.GetPreferenceValue(userID, itemID) == null)
                {
                    result.Add(new GenericRecommendedItem(itemID, RandomPref()));
                }
            }
            return result;
        }

        public override float EstimatePreference(long userID, long itemID)
        {
            return RandomPref();
        }

        private float RandomPref()
        {
            return minPref + (float)random.nextDouble() * (maxPref - minPref);
        }

        public override void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            GetDataModel().Refresh(alreadyRefreshed);
        }
    }
}