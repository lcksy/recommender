using System;
using System.Collections.Generic;
using System.Threading;

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
        private static DateTime _unixTimestampEpochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
        private IDataModel _dataModel;
        private bool _uniqueUserItemCheck = true;
        #endregion

        #region process frequency model
        public IDataModel BuilderModel()
        {
            if (this._dataModel == null || this._dataModel.GetNumItems() == 0)
            {
                var timestamps = new FastByIDMap<FastByIDMap<DateTime?>>();
                var data = new FastByIDMap<IList<IPreference>>();

                var frequencies = DataReaderResolverFactory.Create().Read();
                foreach (var freq in frequencies)
                {
                    this.ProccessFrequency(freq, data, timestamps, false);
                }

                this._dataModel = this.DoGenericDataModel(data, timestamps); 
            }

            return this._dataModel;
        }

        public IDataModel BuilderModelFromCustomerSysNo(long customerSysNo)
        {
            if (this._dataModel.GetNumItems() == 0)
                return this.BuilderModel();

            var userData = ((GenericDataModel)this._dataModel).GetRawUserData();
            var timestamps = new FastByIDMap<FastByIDMap<DateTime?>>();
            var frequencies = DataReaderResolverFactory.Create().ReadByCustomerSysNo(customerSysNo);
            foreach (var freq in frequencies)
            {
                this.ProccessFrequency(freq, userData, timestamps, true);
            }

            this._dataModel = new GenericDataModel(userData, timestamps);

            return this._dataModel;
        }

        public IDataModel BuilderModelFromTimeStamp(long timeStamp)
        {
            if (this._dataModel.GetNumItems() == 0)
                return this.BuilderModel();

            var rawData = ((GenericDataModel)this._dataModel).GetRawUserData();
            var timestamps = new FastByIDMap<FastByIDMap<DateTime?>>();
            var frequencies = DataReaderResolverFactory.Create().ReadGreaterThanTimeStamp(timeStamp);
            foreach (var freq in frequencies)
            {
                this.ProccessFrequency(freq, rawData, timestamps, true);
            }

            return new GenericDataModel(rawData, timestamps);
        }

        private IDataModel DoGenericDataModel(FastByIDMap<IList<IPreference>> data, FastByIDMap<FastByIDMap<DateTime?>> timestamps)
        {
            return new GenericDataModel(GenericDataModel.ToDataMap(data, true), timestamps);
        }

        private void ProccessFrequency<T>(ProductFrequency freqency,
                                       FastByIDMap<T> data,
                                       FastByIDMap<FastByIDMap<DateTime?>> timestamps,
                                       bool fromPriorData = false)
        {
            if (freqency == null) return;

            var maybePrefs = data.Get(freqency.CustomerSysNo);

            if (fromPriorData)
            {
                IPreferenceArray prefs = (IPreferenceArray)maybePrefs;
                float preferenceValue = this.CalculateFrequency(freqency);

                bool exists = false;
                if (_uniqueUserItemCheck && prefs != null)
                {
                    for (int i = 0; i < prefs.Length(); i++)
                    {
                        if (prefs.GetItemID(i) == freqency.ProductSysNo)
                        {
                            exists = true;
                            prefs.SetValue(i, preferenceValue);
                            break;
                        }
                    }
                }

                if (!exists)
                {
                    if (prefs == null)
                    {
                        prefs = new GenericUserPreferenceArray(1);
                    }
                    else
                    {
                        IPreferenceArray newPrefs = new GenericUserPreferenceArray(prefs.Length() + 1);
                        for (int i = 0, j = 1; i < prefs.Length(); i++, j++)
                        {
                            newPrefs.Set(j, prefs.Get(i));
                        }
                        prefs = newPrefs;
                    }
                    prefs.SetUserID(0, freqency.CustomerSysNo);
                    prefs.SetItemID(0, freqency.ProductSysNo);
                    prefs.SetValue(0, preferenceValue);
                    data.Put(freqency.CustomerSysNo, (T)prefs);
                }

                AddTimestamp(freqency.CustomerSysNo, freqency.ProductSysNo, freqency.TimeStamp, timestamps);
            }
            else
            {
                var prefs = ((IEnumerable<IPreference>)maybePrefs);
                var preferenceValue = this.CalculateFrequency(freqency);
                bool exists = false;
                if (_uniqueUserItemCheck && prefs != null)
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
            return _unixTimestampEpochStart.AddMilliseconds(value);
        } 
        #endregion

        #region common methods
        public override IEnumerator<long> GetUserIDs()
        {
            return _dataModel.GetUserIDs();
        }

        public override IPreferenceArray GetPreferencesFromUser(long userSysNo)
        {
            return _dataModel.GetPreferencesFromUser(userSysNo);
        }

        public override FastIDSet GetItemIDsFromUser(long userSysNo)
        {
            return _dataModel.GetItemIDsFromUser(userSysNo);
        }

        public override IEnumerator<long> GetItemIDs()
        {
            return _dataModel.GetItemIDs();
        }

        public override IPreferenceArray GetPreferencesForItem(long itemSysNo)
        {
            return _dataModel.GetPreferencesForItem(itemSysNo);
        }

        public override float? GetPreferenceValue(long userSysNo, long itemSysNo)
        {
            return _dataModel.GetPreferenceValue(userSysNo, itemSysNo);
        }

        public override DateTime? GetPreferenceTime(long userSysNo, long itemSysNo)
        {
            return _dataModel.GetPreferenceTime(userSysNo, itemSysNo);
        }

        public override int GetNumItems()
        {
            return _dataModel.GetNumItems();
        }

        public override int GetNumUsers()
        {
            return _dataModel.GetNumUsers();
        }

        public override int GetNumUsersWithPreferenceFor(long itemSysNo)
        {
            return _dataModel.GetNumUsersWithPreferenceFor(itemSysNo);
        }

        public override int GetNumUsersWithPreferenceFor(long itemSysNo1, long itemSysNo2)
        {
            return _dataModel.GetNumUsersWithPreferenceFor(itemSysNo1, itemSysNo2);
        }

        public override void SetPreference(long userSysNo, long itemSysNo, float value)
        {
            _dataModel.SetPreference(userSysNo, itemSysNo, value);
        }

        public override void RemovePreference(long userSysNo, long itemSysNo)
        {
            _dataModel.RemovePreference(userSysNo, itemSysNo);
        }

        public override void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            this.BuilderModel();
        }

        public override bool HasPreferenceValues()
        {
            return _dataModel.HasPreferenceValues();
        }

        public override float GetMaxPreference()
        {
            return _dataModel.GetMaxPreference();
        }

        public override float GetMinPreference()
        {
            return _dataModel.GetMinPreference();
        } 
        #endregion
    }
}