using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Impl.Model;
using NReco.CF.Taste.Model;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;
using NReco.Recommender.Extension.Recommender.DataReaderResolver;

namespace NReco.Recommender.Extension.Recommender.DataModelResolver
{
    public abstract class DataModelResolverBase : AbstractDataModel
    {
        #region private fields
        private static DateTime unixTimestampEpochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
        private IDataModel dataModel;
        private bool uniqueUserItemCheck = true;
        #endregion

        #region process frequency model
        public IDataModel BuilderModel()
        {
            if (this.dataModel == null)
            {
                var timestamps = new FastByIDMap<FastByIDMap<DateTime?>>();
                var data = new FastByIDMap<IList<IPreference>>();

                var frequencies = DataReaderResolverFactory.Create().Read();
                foreach (var freq in frequencies)
                {
                    this.ProccessFrequency(freq, data, timestamps);
                }

                this.dataModel = this.DoGenericDataModel(data, timestamps); 
            }

            return this.dataModel;
        }

        public IDataModel BuilderModel(int customerSysNo)
        {
            var rawData = ((GenericDataModel)this.dataModel).GetRawUserData();

            var timestamps = new FastByIDMap<FastByIDMap<DateTime?>>();

            var frequencies = DataReaderResolverFactory.Create().ReadByCustomerSysNo(customerSysNo);

            foreach (var freq in frequencies)
            {
                this.ProccessFrequency(freq, rawData, timestamps);
            }

            return new GenericDataModel(rawData, timestamps);
        }

        public IDataModel BuilderModel(long timeStamp)
        {
            FastByIDMap<IPreferenceArray> rawData = ((GenericDataModel)this.dataModel).GetRawUserData();

            var timestamps = new FastByIDMap<FastByIDMap<DateTime?>>();

            var frequencies = DataReaderResolverFactory.Create().ReadGreaterThanTimeStamp(timeStamp);

            foreach (var freq in frequencies)
            {
                this.ProccessFrequency(freq, rawData, timestamps);
            }

            return new GenericDataModel(rawData, timestamps);
        }

        private IDataModel DoGenericDataModel(FastByIDMap<IList<IPreference>> data, FastByIDMap<FastByIDMap<DateTime?>> timestamps)
        {
            return new GenericDataModel(GenericDataModel.ToDataMap(data, true), timestamps);
        }

        private void ProccessFrequency<T>(ProductFrequency freqency,
                                       FastByIDMap<T> data,
                                       FastByIDMap<FastByIDMap<DateTime?>> timestamps)
        {
            if (freqency == null) return;

            var maybePrefs = data.Get(freqency.CustomerSysNo);
            var prefs = ((IEnumerable<IPreference>)maybePrefs);
            var preferenceValue = this.CalculateFrequency(freqency);

            bool exists = false;
            if (uniqueUserItemCheck && prefs != null)
            {
                foreach (IPreference pref in prefs)
                {
                    if (pref.GetItemID() == freqency.ProductSysNo)
                    {
                        exists = true;
                        pref.SetValue(preferenceValue);
                        break;
                    }
                }
            }

            if (!exists)
            {
                if (prefs == null)
                {
                    prefs = new List<IPreference>(5);
                    data.Put(freqency.CustomerSysNo, (T)prefs);
                }

                if (prefs is IList<IPreference>)
                    ((IList<IPreference>)prefs).Add(new GenericPreference(freqency.CustomerSysNo, freqency.ProductSysNo, preferenceValue));
            }

            AddTimestamp(freqency.CustomerSysNo, freqency.ProductSysNo, freqency.TimeStamp, timestamps);
        }

        private float CalculateFrequency(ProductFrequency freqency)
        {
            var value = this.DoCalculateFrequency(freqency.BuyFrequency) * 0.495F
                      + this.DoCalculateFrequency(freqency.CommentFrequency) * 0.495F
                      + this.DoCalculateFrequency(freqency.ClickFrequency) * 0.01F;

            return value;
        }

        protected virtual float DoCalculateFrequency(float rate)
        {
            var value = 1 / (1 + rate);

            return value;
        }

        private void AddTimestamp(long userSysNo,
                          long itemSysNo,
                          long timestampValue,
                          FastByIDMap<FastByIDMap<DateTime?>> timestamps)
        {
            if (timestampValue > 0)
            {
                FastByIDMap<DateTime?> itemTimestamps = timestamps.Get(userSysNo);
                if (itemTimestamps == null)
                {
                    itemTimestamps = new FastByIDMap<DateTime?>();
                    timestamps.Put(userSysNo, itemTimestamps);
                }
                var timestamp = ReadTimestamp(timestampValue);
                itemTimestamps.Put(itemSysNo, timestamp);
            }
        }

        private DateTime ReadTimestamp(long value)
        {
            return unixTimestampEpochStart.AddMilliseconds(value);
        } 
        #endregion

        #region common methods
        public override IEnumerator<long> GetUserIDs()
        {
            return dataModel.GetUserIDs();
        }

        public override IPreferenceArray GetPreferencesFromUser(long userSysNo)
        {
            return dataModel.GetPreferencesFromUser(userSysNo);
        }

        public override FastIDSet GetItemIDsFromUser(long userSysNo)
        {
            return dataModel.GetItemIDsFromUser(userSysNo);
        }

        public override IEnumerator<long> GetItemIDs()
        {
            return dataModel.GetItemIDs();
        }

        public override IPreferenceArray GetPreferencesForItem(long itemSysNo)
        {
            return dataModel.GetPreferencesForItem(itemSysNo);
        }

        public override float? GetPreferenceValue(long userSysNo, long itemSysNo)
        {
            return dataModel.GetPreferenceValue(userSysNo, itemSysNo);
        }

        public override DateTime? GetPreferenceTime(long userSysNo, long itemSysNo)
        {
            return dataModel.GetPreferenceTime(userSysNo, itemSysNo);
        }

        public override int GetNumItems()
        {
            return dataModel.GetNumItems();
        }

        public override int GetNumUsers()
        {
            return dataModel.GetNumUsers();
        }

        public override int GetNumUsersWithPreferenceFor(long itemSysNo)
        {
            return dataModel.GetNumUsersWithPreferenceFor(itemSysNo);
        }

        public override int GetNumUsersWithPreferenceFor(long itemSysNo1, long itemSysNo2)
        {
            return dataModel.GetNumUsersWithPreferenceFor(itemSysNo1, itemSysNo2);
        }

        public override void SetPreference(long userSysNo, long itemSysNo, float value)
        {
            dataModel.SetPreference(userSysNo, itemSysNo, value);
        }

        public override void RemovePreference(long userSysNo, long itemSysNo)
        {
            dataModel.RemovePreference(userSysNo, itemSysNo);
        }

        public override void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            this.BuilderModel();
        }

        public override bool HasPreferenceValues()
        {
            return dataModel.HasPreferenceValues();
        }

        public override float GetMaxPreference()
        {
            return dataModel.GetMaxPreference();
        }

        public override float GetMinPreference()
        {
            return dataModel.GetMinPreference();
        } 
        #endregion
    }
}