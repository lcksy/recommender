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
        private IDataModel DataModel;
        private bool uniqueUserItemCheck;
        #endregion

        #region process frequency model
        public IDataModel BuilderModel()
        {
            var timestamps = new FastByIDMap<FastByIDMap<DateTime?>>();
            var data = new FastByIDMap<IList<IPreference>>();

            var frequencies = DataReaderResolverFactory.Create().Read();
            foreach (var freq in frequencies)
            {
                this.ProccessFrequency(freq, data, timestamps);
            }

            this.DataModel = this.DoGenericDataModel(data, timestamps);

            return DoGenericDataModel(data, timestamps);
        }

        public IDataModel BuilderModel(int customerSysNo)
        {
            FastByIDMap<IPreferenceArray> rawData = ((GenericDataModel)this.DataModel).GetRawUserData();

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
            FastByIDMap<IPreferenceArray> rawData = ((GenericDataModel)this.DataModel).GetRawUserData();

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

            var maybePrefs = data.Get(freqency.SysNo);
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
            var value = this.DoCalculateFrequency(freqency.BuyFrequency) * 0.45F
                      + this.DoCalculateFrequency(freqency.CommentFrequency) * 0.45F
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
            return DataModel.GetUserIDs();
        }

        public override IPreferenceArray GetPreferencesFromUser(long userSysNo)
        {
            return DataModel.GetPreferencesFromUser(userSysNo);
        }

        public override FastIDSet GetItemIDsFromUser(long userSysNo)
        {
            return DataModel.GetItemIDsFromUser(userSysNo);
        }

        public override IEnumerator<long> GetItemIDs()
        {
            return DataModel.GetItemIDs();
        }

        public override IPreferenceArray GetPreferencesForItem(long itemSysNo)
        {
            return DataModel.GetPreferencesForItem(itemSysNo);
        }

        public override float? GetPreferenceValue(long userSysNo, long itemSysNo)
        {
            return DataModel.GetPreferenceValue(userSysNo, itemSysNo);
        }

        public override DateTime? GetPreferenceTime(long userSysNo, long itemSysNo)
        {
            return DataModel.GetPreferenceTime(userSysNo, itemSysNo);
        }

        public override int GetNumItems()
        {
            return DataModel.GetNumItems();
        }

        public override int GetNumUsers()
        {
            return DataModel.GetNumUsers();
        }

        public override int GetNumUsersWithPreferenceFor(long itemSysNo)
        {
            return DataModel.GetNumUsersWithPreferenceFor(itemSysNo);
        }

        public override int GetNumUsersWithPreferenceFor(long itemSysNo1, long itemSysNo2)
        {
            return DataModel.GetNumUsersWithPreferenceFor(itemSysNo1, itemSysNo2);
        }

        public override void SetPreference(long userSysNo, long itemSysNo, float value)
        {
            DataModel.SetPreference(userSysNo, itemSysNo, value);
        }

        public override void RemovePreference(long userSysNo, long itemSysNo)
        {
            DataModel.RemovePreference(userSysNo, itemSysNo);
        }

        public override void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            this.BuilderModel();
        }

        public override bool HasPreferenceValues()
        {
            return DataModel.HasPreferenceValues();
        }

        public override float GetMaxPreference()
        {
            return DataModel.GetMaxPreference();
        }

        public override float GetMinPreference()
        {
            return DataModel.GetMinPreference();
        } 
        #endregion
    }
}