using System;
using System.Collections;
using System.Collections.Generic;

namespace NReco.CF.Taste.Impl.Common
{
    /// @see FastByIDMap
    public sealed class FastIDSet : ICloneable, IEnumerable<long>
    {
        private static float DEFAULT_LOAD_FACTOR = 1.5f;

        /// Dummy object used to represent a key that has been removed. 
        private static long REMOVED = long.MaxValue;
        private static long NULL = Int64.MinValue;

        private long[] keys;
        private float loadFactor;
        private int numEntries;
        private int numSlotsUsed;

        /// Creates a new {@link FastIDSet} with default capacity. 
        public FastIDSet()
            : this(2)
        {
        }

        public FastIDSet(long[] initialKeys)
            : this(initialKeys.Length)
        {
            AddAll(initialKeys);
        }

        public FastIDSet(int size)
            : this(size, DEFAULT_LOAD_FACTOR)
        {
        }

        public FastIDSet(int size, float loadFactor)
        {
            //Preconditions.checkArgument(size >= 0, "size must be at least 0");
            //Preconditions.checkArgument(loadFactor >= 1.0f, "loadFactor must be at least 1.0");
            this.loadFactor = loadFactor;
            int max = (int)(RandomUtils.MAX_INT_SMALLER_TWIN_PRIME / loadFactor);
            //Preconditions.checkArgument(size < max, "size must be less than %d", max);
            int hashSize = RandomUtils.nextTwinPrime((int)(loadFactor * size));
            keys = new long[hashSize];

            ArrayFill(keys, NULL);
        }

        private FastIDSet(FastIDSet copyFrom)
        {
            keys = copyFrom.keys;
            loadFactor = copyFrom.loadFactor;
            numEntries = copyFrom.numEntries;
            numSlotsUsed = copyFrom.numSlotsUsed;
        }

        void ArrayFill<T>(T[] a, T val)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] = val;
        }

        /// @see #findForAdd(long)
        private int Find(long key)
        {
            int theHashCode = (int)key & 0x7FFFFFFF; // make sure it's positive
            long[] keys = this.keys;
            int hashSize = keys.Length;
            int jump = 1 + theHashCode % (hashSize - 2);
            int index = theHashCode % hashSize;
            long currentKey = keys[index];
            while (currentKey != NULL && key != currentKey)
            { // note: true when currentKey == REMOVED
                index -= index < jump ? jump - hashSize : jump;
                currentKey = keys[index];
            }
            return index;
        }

        /// @see #find(long)
        private int FindForAdd(long key)
        {
            int theHashCode = (int)key & 0x7FFFFFFF; // make sure it's positive
            long[] keys = this.keys;
            int hashSize = keys.Length;
            int jump = 1 + theHashCode % (hashSize - 2);
            int index = theHashCode % hashSize;
            long currentKey = keys[index];
            while (currentKey != NULL && currentKey != REMOVED && key != currentKey)
            {
                index -= index < jump ? jump - hashSize : jump;
                currentKey = keys[index];
            }
            if (currentKey != REMOVED)
            {
                return index;
            }
            // If we're adding, it's here, but, the key might have a value already later
            int addIndex = index;
            while (currentKey != NULL && key != currentKey)
            {
                index -= index < jump ? jump - hashSize : jump;
                currentKey = keys[index];
            }
            return key == currentKey ? index : addIndex;
        }

        public int Count()
        {
            return numEntries;
        }

        public bool IsEmpty()
        {
            return numEntries == 0;
        }

        public bool Contains(long key)
        {
            return key != NULL && key != REMOVED && keys[Find(key)] != NULL;
        }

        public bool Add(long key)
        {
            if (key == NULL || key == REMOVED)
                throw new ArgumentException();
            //Preconditions.checkArgument(key != NULL && key != REMOVED);

            // If less than half the slots are open, let's clear it up
            if (numSlotsUsed * loadFactor >= keys.Length)
            {
                // If over half the slots used are actual entries, let's grow
                if (numEntries * loadFactor >= numSlotsUsed)
                {
                    GrowAndRehash();
                }
                else
                {
                    // Otherwise just rehash to clear REMOVED entries and don't grow
                    Rehash();
                }
            }
            // Here we may later consider implementing Brent's variation described on page 532
            int index = FindForAdd(key);
            long keyIndex = keys[index];
            if (keyIndex != key)
            {
                keys[index] = key;
                numEntries++;
                if (keyIndex == NULL)
                {
                    numSlotsUsed++;
                }
                return true;
            }
            return false;
        }

        public IEnumerator<long> GetEnumerator()
        {
            for (int position = 0; position < keys.Length; position++)
            {
                if (keys[position] != NULL && keys[position] != REMOVED)
                {
                    yield return keys[position];
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public long[] ToArray()
        {
            long[] result = new long[numEntries];
            for (int i = 0, position = 0; i < result.Length; i++)
            {
                while (keys[position] == NULL || keys[position] == REMOVED)
                {
                    position++;
                }
                result[i] = keys[position++];
            }
            return result;
        }

        public bool Remove(long key)
        {
            if (key == NULL || key == REMOVED)
            {
                return false;
            }
            int index = Find(key);
            if (keys[index] == NULL)
            {
                return false;
            }
            else
            {
                keys[index] = REMOVED;
                numEntries--;
                return true;
            }
        }

        public bool AddAll(long[] c)
        {
            bool changed = false;
            foreach (long k in c)
            {
                if (Add(k))
                {
                    changed = true;
                }
            }
            return changed;
        }

        public bool AddAll(FastIDSet c)
        {
            bool changed = false;
            foreach (long k in c.keys)
            {
                if (k != NULL && k != REMOVED && Add(k))
                {
                    changed = true;
                }
            }
            return changed;
        }

        public bool RemoveAll(long[] c)
        {
            bool changed = false;
            foreach (long o in c)
            {
                if (Remove(o))
                {
                    changed = true;
                }
            }
            return changed;
        }

        public bool RemoveAll(FastIDSet c)
        {
            bool changed = false;
            foreach (long k in c.keys)
            {
                if (k != NULL && k != REMOVED && Remove(k))
                {
                    changed = true;
                }
            }
            return changed;
        }

        public bool RetainAll(FastIDSet c)
        {
            bool changed = false;
            for (int i = 0; i < keys.Length; i++)
            {
                long k = keys[i];
                if (k != NULL && k != REMOVED && !c.Contains(k))
                {
                    keys[i] = REMOVED;
                    numEntries--;
                    changed = true;
                }
            }
            return changed;
        }

        public void Clear()
        {
            numEntries = 0;
            numSlotsUsed = 0;
            ArrayFill(keys, NULL);
        }

        private void GrowAndRehash()
        {
            if (keys.Length * loadFactor >= RandomUtils.MAX_INT_SMALLER_TWIN_PRIME)
            {
                throw new InvalidOperationException("Can't grow any more");
            }
            Rehash(RandomUtils.nextTwinPrime((int)(loadFactor * keys.Length)));
        }

        public void Rehash()
        {
            Rehash(RandomUtils.nextTwinPrime((int)(loadFactor * numEntries)));
        }

        private void Rehash(int newHashSize)
        {
            long[] oldKeys = keys;
            numEntries = 0;
            numSlotsUsed = 0;
            keys = new long[newHashSize];
            ArrayFill(keys, NULL);
            foreach (long key in oldKeys)
            {
                if (key != NULL && key != REMOVED)
                {
                    Add(key);
                }
            }
        }

        /// Convenience method to quickly compute just the size of the intersection with another {@link FastIDSet}.
        /// 
        /// @param other
        ///          {@link FastIDSet} to intersect with
        /// @return number of elements in intersection
        public int IntersectionSize(FastIDSet other)
        {
            int count = 0;
            foreach (long key in other.keys)
            {
                if (key != NULL && key != REMOVED && keys[Find(key)] != NULL)
                {
                    count++;
                }
            }
            return count;
        }

        public object Clone()
        {
            return new FastIDSet(this);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            long[] keys = this.keys;
            foreach (long key in keys)
            {
                if (key != NULL && key != REMOVED)
                {
                    hash = 31 * hash + ((int)(key >> 32) ^ (int)key);
                }
            }
            return hash;
        }

        public override bool Equals(Object other)
        {
            if (!(other is FastIDSet))
            {
                return false;
            }
            FastIDSet otherMap = (FastIDSet)other;
            long[] otherKeys = otherMap.keys;
            int length = keys.Length;
            int otherLength = otherKeys.Length;
            int max = Math.Min(length, otherLength);

            int i = 0;
            while (i < max)
            {
                long key = keys[i];
                long otherKey = otherKeys[i];
                if (key == NULL || key == REMOVED)
                {
                    if (otherKey != NULL && otherKey != REMOVED)
                    {
                        return false;
                    }
                }
                else
                {
                    if (key != otherKey)
                    {
                        return false;
                    }
                }
                i++;
            }
            while (i < length)
            {
                long key = keys[i];
                if (key != NULL && key != REMOVED)
                {
                    return false;
                }
                i++;
            }
            while (i < otherLength)
            {
                long key = otherKeys[i];
                if (key != NULL && key != REMOVED)
                {
                    return false;
                }
                i++;
            }
            return true;
        }

        public override string ToString()
        {
            if (IsEmpty())
            {
                return "[]";
            }
            var result = new System.Text.StringBuilder();
            result.Append('[');
            foreach (long key in keys)
            {
                if (key != NULL && key != REMOVED)
                {
                    result.Append(key).Append(',');
                }
            }
            result[result.Length - 1] = ']';
            return result.ToString();
        }
    }
}