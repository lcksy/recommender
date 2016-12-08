using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Impl.Model;
using NReco.CF.Taste.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace NReco.CF.Taste.Impl.Model.SqlServer
{
    public class SqlServerDataModel : AbstractDataModel
    {
        private static Logger log = LoggerFactory.GetLogger(typeof(SqlServerDataModel));

        private static DateTime unixTimestampEpochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
        private IDataModel _delegate;
        private bool uniqueUserItemCheck;
        private string connectionString = string.Empty;

        public SqlServerDataModel(string connectionString)
        {
            this.uniqueUserItemCheck = true;
            this.connectionString = connectionString;

            this.Reload();
        }

        protected void Reload()
        {
            _delegate = BuildModel();
        }

        protected IDataModel BuildModel()
        {
            bool loadFreshData = _delegate == null;

            var timestamps = new FastByIDMap<FastByIDMap<DateTime?>>();
            FastByIDMap<IList<IPreference>> data = new FastByIDMap<IList<IPreference>>();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                var sql = "SELECT * FROM ProductComment";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    var reader = command.ExecuteReader();

                    ProcessDataReader(reader, data, timestamps);
                }
            }

            return new GenericDataModel(GenericDataModel.ToDataMap(data, true), timestamps);
        }

        protected void ProcessDataReader<T>(SqlDataReader reader, FastByIDMap<T> data, FastByIDMap<FastByIDMap<DateTime?>> timestamps)
        {
            while (reader.Read())
            {
                ProcessReaderRow(reader, data, timestamps);
            }
        }

        protected void ProcessReaderRow<T>(SqlDataReader reader, FastByIDMap<T> data, FastByIDMap<FastByIDMap<DateTime?>> timestamps)
        {
            if (!reader.HasRows) return;

            var sysno = reader.GetInt64(0).ToString();
            var userSysNo = reader.GetInt32(1).ToString();
            var itemSysNo = reader.GetInt32(2).ToString();
            var preferenceValueString = reader.GetInt32(3).ToString();
            var timestampString = reader.GetInt64(4).ToString();

            var maybePrefs = data.Get(long.Parse(userSysNo.ToString()));

            var prefs = ((IEnumerable<IPreference>)maybePrefs);
            float preferenceValue = float.Parse(preferenceValueString.ToString(), CultureInfo.InvariantCulture);

            bool exists = false;
            if (uniqueUserItemCheck && prefs != null)
            {
                foreach (IPreference pref in prefs)
                {
                    if (pref.GetItemID() == itemSysNo.ToLong())
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
                    data.Put(userSysNo.ToLong(), (T)prefs);
                }

                if (prefs is IList<IPreference>)
                    ((IList<IPreference>)prefs).Add(new GenericPreference(userSysNo.ToLong(), itemSysNo.ToLong(), preferenceValue));
            }

            AddTimestamp(userSysNo.ToLong(), itemSysNo.ToLong(), timestampString, timestamps);
        }

        private void AddTimestamp(long userSysNo,
                                  long itemSysNo,
                                  string timestampString,
                                  FastByIDMap<FastByIDMap<DateTime?>> timestamps)
        {
            if (timestampString != null)
            {
                FastByIDMap<DateTime?> itemTimestamps = timestamps.Get(userSysNo);
                if (itemTimestamps == null)
                {
                    itemTimestamps = new FastByIDMap<DateTime?>();
                    timestamps.Put(userSysNo, itemTimestamps);
                }
                var timestamp = ReadTimestampFromString(timestampString);
                itemTimestamps.Put(itemSysNo, timestamp);
            }
        }

        protected DateTime ReadTimestampFromString(string value)
        {
            var unixTimestamp = long.Parse(value, CultureInfo.InvariantCulture);
            return unixTimestampEpochStart.AddMilliseconds(unixTimestamp);
        }

        public override IEnumerator<long> GetUserIDs()
        {
            return _delegate.GetUserIDs();
        }

        public override IPreferenceArray GetPreferencesFromUser(long userSysNo)
        {
            return _delegate.GetPreferencesFromUser(userSysNo);
        }

        public override FastIDSet GetItemIDsFromUser(long userSysNo)
        {
            return _delegate.GetItemIDsFromUser(userSysNo);
        }

        public override IEnumerator<long> GetItemIDs()
        {
            return _delegate.GetItemIDs();
        }

        public override IPreferenceArray GetPreferencesForItem(long itemSysNo)
        {
            return _delegate.GetPreferencesForItem(itemSysNo);
        }

        public override float? GetPreferenceValue(long userSysNo, long itemSysNo)
        {
            return _delegate.GetPreferenceValue(userSysNo, itemSysNo);
        }

        public override DateTime? GetPreferenceTime(long userSysNo, long itemSysNo)
        {
            return _delegate.GetPreferenceTime(userSysNo, itemSysNo);
        }

        public override int GetNumItems()
        {
            return _delegate.GetNumItems();
        }

        public override int GetNumUsers()
        {
            return _delegate.GetNumUsers();
        }

        public override int GetNumUsersWithPreferenceFor(long itemSysNo)
        {
            return _delegate.GetNumUsersWithPreferenceFor(itemSysNo);
        }

        public override int GetNumUsersWithPreferenceFor(long itemSysNo1, long itemSysNo2)
        {
            return _delegate.GetNumUsersWithPreferenceFor(itemSysNo1, itemSysNo2);
        }

        public override void SetPreference(long userSysNo, long itemSysNo, float value)
        {
            _delegate.SetPreference(userSysNo, itemSysNo, value);
        }

        public override void RemovePreference(long userSysNo, long itemSysNo)
        {
            _delegate.RemovePreference(userSysNo, itemSysNo);
        }

        public override void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            Reload();
        }

        public override bool HasPreferenceValues()
        {
            return _delegate.HasPreferenceValues();
        }

        public override float GetMaxPreference()
        {
            return _delegate.GetMaxPreference();
        }

        public override float GetMinPreference()
        {
            return _delegate.GetMinPreference();
        }

        public override string ToString()
        {
            return "SqlServerDataModel[ConnectionString:" + this.connectionString + ']';
        }
    }

    public static class StringExtension
    {
        public static long ToLong(this string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(value);

            return long.Parse(value);
        }
    }
}