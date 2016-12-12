using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// A simple recommender that always estimates preference for an item to be the average of all known preference
    /// values for that item. No information about users is taken into account. This implementation is provided for
    /// experimentation; while simple and fast, it may not produce very good recommendations.
    /// </summary>
    public sealed class ItemAverageRecommender : AbstractRecommender
    {
        private static Logger log = LoggerFactory.GetLogger(typeof(ItemAverageRecommender));

        private FastByIDMap<IRunningAverage> itemAverages;
        private RefreshHelper refreshHelper;

        public ItemAverageRecommender(IDataModel dataModel)
            : base(dataModel)
        {
            this.itemAverages = new FastByIDMap<IRunningAverage>();
            this.refreshHelper = new RefreshHelper(() =>
            {
                BuildAverageDiffs();
            });
            refreshHelper.AddDependency(dataModel);
            BuildAverageDiffs();
        }

        public override IList<IRecommendedItem> Recommend(long userID, int howMany, IDRescorer rescorer)
        {
            //Preconditions.checkArgument(howMany >= 1, "howMany must be at least 1");
            log.Debug("Recommending items for user ID '{}'", userID);

            IPreferenceArray preferencesFromUser = GetDataModel().GetPreferencesFromUser(userID);
            FastIDSet possibleItemIDs = GetAllOtherItems(userID, preferencesFromUser);

            TopItems.IEstimator<long> estimator = new Estimator(this);

            List<IRecommendedItem> topItems = TopItems.GetTopItems(howMany, possibleItemIDs.GetEnumerator(), rescorer,
              estimator);

            log.Debug("Recommendations are: {}", topItems);
            return topItems;
        }

        public override float EstimatePreference(long userID, long itemID)
        {
            IDataModel dataModel = GetDataModel();
            float? actualPref = dataModel.GetPreferenceValue(userID, itemID);
            if (actualPref.HasValue)
            {
                return actualPref.Value;
            }
            return DoEstimatePreference(itemID);
        }

        private float DoEstimatePreference(long itemID)
        {
            lock (this)
            { // buildAveragesLock.readLock().lock();
                //try {
                IRunningAverage average = itemAverages.Get(itemID);
                return average == null ? float.NaN : (float)average.GetAverage();
                //} finally {
                //buildAveragesLock.readLock().unlock();
                //}
            }
        }

        private void BuildAverageDiffs()
        {
            lock (this)
            {
                //buildAveragesLock.writeLock().lock();
                IDataModel dataModel = GetDataModel();
                var it = dataModel.GetUserIDs();
                while (it.MoveNext())
                {
                    IPreferenceArray prefs = dataModel.GetPreferencesFromUser(it.Current);
                    int size = prefs.Length();
                    for (int i = 0; i < size; i++)
                    {
                        long itemID = prefs.GetItemID(i);
                        IRunningAverage average = itemAverages.Get(itemID);
                        if (average == null)
                        {
                            average = new FullRunningAverage();
                            itemAverages.Put(itemID, average);
                        }
                        average.AddDatum(prefs.GetValue(i));
                    }
                }
            }
            //finally {
            //buildAveragesLock.writeLock().unlock();
            //}
        }

        public override void SetPreference(long userID, long itemID, float value)
        {
            IDataModel dataModel = GetDataModel();
            double prefDelta;
            try
            {
                float? oldPref = dataModel.GetPreferenceValue(userID, itemID);
                prefDelta = !oldPref.HasValue ? value : value - oldPref.Value;
            }
            catch (NoSuchUserException)
            {
                prefDelta = value;
            }
            base.SetPreference(userID, itemID, value);
            lock (this)
            {
                //buildAveragesLock.writeLock().lock();
                IRunningAverage average = itemAverages.Get(itemID);
                if (average == null)
                {
                    IRunningAverage newAverage = new FullRunningAverage();
                    newAverage.AddDatum(prefDelta);
                    itemAverages.Put(itemID, newAverage);
                }
                else
                {
                    average.ChangeDatum(prefDelta);
                }
            }
            //finally {
            //buildAveragesLock.writeLock().unlock();
            //}
        }

        public override void RemovePreference(long userID, long itemID)
        {
            IDataModel dataModel = GetDataModel();
            float? oldPref = dataModel.GetPreferenceValue(userID, itemID);
            base.RemovePreference(userID, itemID);
            if (oldPref.HasValue)
            {
                lock (this)
                {
                    //buildAveragesLock.writeLock().lock();
                    IRunningAverage average = itemAverages.Get(itemID);
                    if (average == null)
                    {
                        throw new InvalidOperationException("No preferences exist for item ID: " + itemID);
                    }
                    else
                    {
                        average.RemoveDatum(oldPref.Value);
                    }
                } //finally {
                //buildAveragesLock.writeLock().unlock();
                //}
            }
        }

        public override void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            refreshHelper.Refresh(alreadyRefreshed);
        }

        public override string ToString()
        {
            return "ItemAverageRecommender";
        }

        private sealed class Estimator : TopItems.IEstimator<long>
        {
            ItemAverageRecommender r;
            internal Estimator(ItemAverageRecommender r)
            {
                this.r = r;
            }

            public double Estimate(long itemID)
            {
                return r.DoEstimatePreference(itemID);
            }
        }
    }
}