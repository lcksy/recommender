using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Model;
//using NReco.CF.iterator;

namespace NReco.CF.Taste.Impl.Model
{
    /// <summary>
    /// Like <see cref="GenericUserPreferenceArray"/> but stores preferences for one item (all item IDs the same) rather
    /// than one user.
    /// </summary>
    /// <seealso cref="BooleanItemPreferenceArray"/>
    /// <seealso cref="GenericUserPreferenceArray"/>
    /// <seealso cref="GenericPreference"/> 
    [Serializable]
    public sealed class GenericItemPreferenceArray : IPreferenceArray
    {
        private const int USER = 0;
        private const int VALUE = 2;
        private const int VALUE_REVERSED = 3;

        private long[] ids;
        private long id;
        private float[] values;

        public GenericItemPreferenceArray(int size)
        {
            this.ids = new long[size];
            values = new float[size];
            this.id = Int64.MinValue; // as a sort of 'unspecified' value
        }

        public GenericItemPreferenceArray(IList<IPreference> prefs)
            : this(prefs.Count)
        {
            int size = prefs.Count;
            long itemID = Int64.MinValue;
            for (int i = 0; i < size; i++)
            {
                IPreference pref = prefs[i];
                ids[i] = pref.GetUserID();
                if (i == 0)
                {
                    itemID = pref.GetItemID();
                }
                else
                {
                    if (itemID != pref.GetItemID())
                    {
                        throw new ArgumentException("Not all item IDs are the same");
                    }
                }
                values[i] = pref.GetValue();
            }
            id = itemID;
        }

        /// This is a private copy constructor for clone().
        private GenericItemPreferenceArray(long[] ids, long id, float[] values)
        {
            this.ids = ids;
            this.id = id;
            this.values = values;
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
            values[i] = pref.GetValue();
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
            return values[i];
        }

        public void SetValue(int i, float value)
        {
            values[i] = value;
        }

        public void SortByUser()
        {
            LateralSort(USER);
        }

        public void SortByItem() { }

        public void SortByValue()
        {
            LateralSort(VALUE);
        }

        public void SortByValueReversed()
        {
            LateralSort(VALUE_REVERSED);
        }

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

        private void LateralSort(int type)
        {
            //Comb sort: http://en.wikipedia.org/wiki/Comb_sort
            int len = Length();
            int gap = len;
            bool swapped = false;
            while (gap > 1 || swapped)
            {
                if (gap > 1)
                {
                    gap = (int)((double)gap / 1.247330950103979); // = 1 / (1 - 1/e^phi)
                }
                swapped = false;
                int max = len - gap;
                for (int i = 0; i < max; i++)
                {
                    int other = i + gap;
                    if (IsLess(other, i, type))
                    {
                        Swap(i, other);
                        swapped = true;
                    }
                }
            }
        }

        private bool IsLess(int i, int j, int type)
        {
            switch (type)
            {
                case USER:
                    return ids[i] < ids[j];
                case VALUE:
                    return values[i] < values[j];
                case VALUE_REVERSED:
                    return values[i] > values[j];
                default:
                    throw new InvalidOperationException();
            }
        }

        private void Swap(int i, int j)
        {
            long temp1 = ids[i];
            float temp2 = values[i];
            ids[i] = ids[j];
            values[i] = values[j];
            ids[j] = temp1;
            values[j] = temp2;
        }

        public IPreferenceArray Clone()
        {
            return new GenericItemPreferenceArray((long[])ids.Clone(), id, (float[])values.Clone());
        }

        public override int GetHashCode()
        {
            return (int)(id >> 32) ^ (int)id ^ Utils.GetArrayHashCode(ids) ^ Utils.GetArrayHashCode(values);
        }

        public override bool Equals(Object other)
        {
            if (!(other is GenericItemPreferenceArray))
            {
                return false;
            }
            GenericItemPreferenceArray otherArray = (GenericItemPreferenceArray)other;
            return id == otherArray.id && Enumerable.SequenceEqual(ids, otherArray.ids) && Enumerable.SequenceEqual(values, otherArray.values);
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
            if (ids == null || ids.Length == 0)
            {
                return "GenericItemPreferenceArray[{}]";
            }
            var result = new System.Text.StringBuilder(20 * ids.Length);
            result.Append("GenericItemPreferenceArray[itemID:");
            result.Append(id);
            result.Append(",{");
            for (int i = 0; i < ids.Length; i++)
            {
                if (i > 0)
                {
                    result.Append(',');
                }
                result.Append(ids[i]);
                result.Append('=');
                result.Append(values[i]);
            }
            result.Append("}]");
            return result.ToString();
        }

        private sealed class PreferenceView : IPreference
        {
            private int i;
            GenericItemPreferenceArray arr;

            internal PreferenceView(GenericItemPreferenceArray arr, int i)
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
                return arr.values[i];
            }

            public void SetValue(float value)
            {
                arr.values[i] = value;
            }
        }
    }
}