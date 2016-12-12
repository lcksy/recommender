using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Model
{
    /// <summary>
    /// Like <see cref="BooleanUserPreferenceArray"/> but stores preferences for one item (all item IDs the same) rather
    /// than one user.
    /// </summary>
    /// <seealso cref="BooleanPreference"/>
    /// <seealso cref="BooleanUserPreferenceArray"/>
    /// <seealso cref="GenericItemPreferenceArray"/>
    public sealed class BooleanItemPreferenceArray : IPreferenceArray
    {
        private long[] ids;
        private long id;

        public BooleanItemPreferenceArray(int size)
        {
            this.ids = new long[size];
            this.id = Int64.MinValue; // as a sort of 'unspecified' value
        }

        public BooleanItemPreferenceArray(List<IPreference> prefs, bool forOneUser)
            : this(prefs.Count)
        {
            int size = prefs.Count;
            for (int i = 0; i < size; i++)
            {
                IPreference pref = prefs[i];
                ids[i] = forOneUser ? pref.GetItemID() : pref.GetUserID();
            }
            if (size > 0)
            {
                id = forOneUser ? prefs[0].GetUserID() : prefs[0].GetItemID();
            }
        }

        /// This is a private copy constructor for clone().
        private BooleanItemPreferenceArray(long[] ids, long id)
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
            id = pref.GetItemID();
            ids[i] = pref.GetUserID();
        }

        public long GetUserID(int i)
        {
            return ids[i];
        }

        public void SetUserID(int i, long userID)
        {
            ids[i] = userID;
        }

        public long GetItemID(int i)
        {
            return id;
        }

        /// {@inheritDoc}
        /// 
        /// Note that this method will actually set the item ID for <em>all</em> preferences.
        public void SetItemID(int i, long itemID)
        {
            id = itemID;
        }

        /// @return all user IDs
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

        public void SortByUser()
        {
            Array.Sort(ids);
        }

        public void SortByItem() { }

        public void SortByValue() { }

        public void SortByValueReversed() { }

        public bool HasPrefWithUserID(long userID)
        {
            foreach (long id in ids)
            {
                if (userID == id)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasPrefWithItemID(long itemID)
        {
            return id == itemID;
        }

        public IPreferenceArray Clone()
        {
            return new BooleanItemPreferenceArray((long[])ids.Clone(), id);
        }

        public override int GetHashCode()
        {
            return (int)(id >> 32) ^ (int)id ^ Utils.GetArrayHashCode(ids);
        }

        public override bool Equals(Object other)
        {
            if (!(other is BooleanItemPreferenceArray))
            {
                return false;
            }
            BooleanItemPreferenceArray otherArray = (BooleanItemPreferenceArray)other;
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
            result.Append("BooleanItemPreferenceArray[itemID:");
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
            BooleanItemPreferenceArray arr;

            internal PreferenceView(BooleanItemPreferenceArray arr, int i)
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