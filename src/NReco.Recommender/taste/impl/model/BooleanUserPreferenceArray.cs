using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using NReco.CF.Taste.Model;
using NReco.CF.Taste.Common;
//using NReco.CF.iterator;

namespace NReco.CF.Taste.Impl.Model
{
    /// <summary>
    /// Like <see cref="GenericUserPreferenceArray"/> but stores, conceptually, <see cref="BooleanPreference"/> objects which
    /// have no associated preference value.
    /// </summary>
    /// <seealso cref="BooleanPreference"/>
    /// <seealso cref="BooleanItemPreferenceArray"/>
    /// <seealso cref="GenericUserPreferenceArray"/>
    public sealed class BooleanUserPreferenceArray : IPreferenceArray
    {
        private long[] ids;
        private long id;

        public BooleanUserPreferenceArray(int size)
        {
            this.ids = new long[size];
            this.id = Int64.MinValue; // as a sort of 'unspecified' value
        }

        public BooleanUserPreferenceArray(List<IPreference> prefs)
            : this(prefs.Count)
        {
            int size = prefs.Count;
            for (int i = 0; i < size; i++)
            {
                IPreference pref = prefs[i];
                ids[i] = pref.GetItemID();
            }
            if (size > 0)
            {
                id = prefs[0].GetUserID();
            }
        }

        /// This is a private copy constructor for clone().
        private BooleanUserPreferenceArray(long[] ids, long id)
        {
            this.ids = ids;
            this.id = id;
        }

        public int Length()
        {
            return ids.Length;
        }

        public IPreference Get(int i)
        {
            return new PreferenceView(this, i);
        }

        public void Set(int i, IPreference pref)
        {
            id = pref.GetUserID();
            ids[i] = pref.GetItemID();
        }

        public long GetUserID(int i)
        {
            return id;
        }

        /// {@inheritDoc}
        /// 
        /// Note that this method will actually set the user ID for <em>all</em> preferences.
        public void SetUserID(int i, long userID)
        {
            id = userID;
        }

        public long GetItemID(int i)
        {
            return ids[i];
        }

        public void SetItemID(int i, long itemID)
        {
            ids[i] = itemID;
        }

        /// @return all item IDs
        public long[] GetIDs()
        {
            return ids;
        }

        public float GetValue(int i)
        {
            return 1.0f;
        }

        public void SetValue(int i, float value)
        {
            throw new NotSupportedException();
        }

        public void SortByUser() { }

        public void SortByItem()
        {
            Array.Sort(ids);
        }

        public void SortByValue() { }

        public void SortByValueReversed() { }

        public bool HasPrefWithUserID(long userID)
        {
            return id == userID;
        }

        public bool HasPrefWithItemID(long itemID)
        {
            foreach (long id in ids)
            {
                if (itemID == id)
                {
                    return true;
                }
            }
            return false;
        }

        public IPreferenceArray Clone()
        {
            return new BooleanUserPreferenceArray((long[])ids.Clone(), id);
        }

        public override int GetHashCode()
        {
            return (int)(id >> 32) ^ (int)id ^ Utils.GetArrayHashCode(ids);
        }

        public override bool Equals(Object other)
        {
            if (!(other is BooleanUserPreferenceArray))
            {
                return false;
            }
            var otherArray = (BooleanUserPreferenceArray)other;
            return id == otherArray.id && Enumerable.SequenceEqual(ids, otherArray.ids);
        }

        public IEnumerator<IPreference> GetEnumerator()
        {
            for (int i = 0; i < Length(); i++)
                yield return new PreferenceView(this, i);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var result = new System.Text.StringBuilder(10 * ids.Length);
            result.Append("BooleanUserPreferenceArray[userID:");
            result.Append(id);
            result.Append(",{");
            for (int i = 0; i < ids.Length; i++)
            {
                if (i > 0)
                {
                    result.Append(',');
                }
                result.Append(ids[i]);
            }
            result.Append("}]");
            return result.ToString();
        }

        private sealed class PreferenceView : IPreference
        {

            private int i;
            BooleanUserPreferenceArray arr;

            internal PreferenceView(BooleanUserPreferenceArray arr, int i)
            {
                this.i = i;
                this.arr = arr;
            }

            public long GetUserID()
            {
                return arr.GetUserID(i);
            }

            public long GetItemID()
            {
                return arr.GetItemID(i);
            }

            public float GetValue()
            {
                return 1.0f;
            }

            public void SetValue(float value)
            {
                throw new NotSupportedException();
            }
        }
    }
}