using System;
using System.Collections;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Impl.Recommender;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Similarity
{
    public sealed class GenericUserSimilarity : IUserSimilarity
    {
        private FastByIDMap<FastByIDMap<Double>> similarityMaps = new FastByIDMap<FastByIDMap<Double>>();

        public GenericUserSimilarity(IEnumerable<UserUserSimilarity> similarities)
        {
            InitSimilarityMaps(similarities.GetEnumerator());
        }

        public GenericUserSimilarity(IEnumerable<UserUserSimilarity> similarities, int maxToKeep)
        {
            IEnumerable<UserUserSimilarity> keptSimilarities =
              TopItems.GetTopUserUserSimilarities(maxToKeep, similarities.GetEnumerator());
            InitSimilarityMaps(keptSimilarities.GetEnumerator());
        }

        public GenericUserSimilarity(IUserSimilarity otherSimilarity, IDataModel dataModel)
        {
            long[] userIDs = LongIteratorToList(dataModel.GetUserIDs());
            InitSimilarityMaps(new DataModelSimilaritiesIterator(otherSimilarity, userIDs));
        }

        public GenericUserSimilarity(IUserSimilarity otherSimilarity,
                                     IDataModel dataModel,
                                     int maxToKeep)
        {
            long[] userIDs = LongIteratorToList(dataModel.GetUserIDs());
            IEnumerator<UserUserSimilarity> it = new DataModelSimilaritiesIterator(otherSimilarity, userIDs);
            var keptSimilarities = TopItems.GetTopUserUserSimilarities(maxToKeep, it);
            InitSimilarityMaps(keptSimilarities.GetEnumerator());
        }

        public static long[] LongIteratorToList(IEnumerator<long> iterator)
        {
            long[] result = new long[5];
            int size = 0;
            while (iterator.MoveNext())
            {
                if (size == result.Length)
                {
                    long[] newResult = new long[result.Length << 1];
                    Array.Copy(result, 0, newResult, 0, result.Length);
                    result = newResult;
                }
                result[size++] = iterator.Current;
            }
            if (size != result.Length)
            {
                long[] newResult = new long[size];
                Array.Copy(result, 0, newResult, 0, size);
                result = newResult;
            }
            return result;
        }

        private void InitSimilarityMaps(IEnumerator<UserUserSimilarity> similarities)
        {
            while (similarities.MoveNext())
            {
                UserUserSimilarity uuc = similarities.Current;
                long similarityUser1 = uuc.getUserID1();
                long similarityUser2 = uuc.getUserID2();
                if (similarityUser1 != similarityUser2)
                {
                    // Order them -- first key should be the "smaller" one
                    long user1;
                    long user2;
                    if (similarityUser1 < similarityUser2)
                    {
                        user1 = similarityUser1;
                        user2 = similarityUser2;
                    }
                    else
                    {
                        user1 = similarityUser2;
                        user2 = similarityUser1;
                    }
                    FastByIDMap<Double> map = similarityMaps.Get(user1);
                    if (map == null)
                    {
                        map = new FastByIDMap<Double>();
                        similarityMaps.Put(user1, map);
                    }
                    map.Put(user2, uuc.getValue());
                }
                // else similarity between user and itself already assumed to be 1.0
            }
        }

        public double UserSimilarity(long userID1, long userID2)
        {
            if (userID1 == userID2)
            {
                return 1.0;
            }
            long first;
            long second;
            if (userID1 < userID2)
            {
                first = userID1;
                second = userID2;
            }
            else
            {
                first = userID2;
                second = userID1;
            }
            FastByIDMap<Double> nextMap = similarityMaps.Get(first);
            if (nextMap == null)
            {
                return Double.NaN;
            }
            Double similarity = nextMap.Get(second);
            return similarity == null ? Double.NaN : similarity;
        }

        public void SetPreferenceInferrer(IPreferenceInferrer inferrer)
        {
            throw new NotSupportedException();
        }

        public void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            // Do nothing
        }

        public class UserUserSimilarity : IComparable<UserUserSimilarity>
        {
            private long userID1;
            private long userID2;
            private double value;

            public UserUserSimilarity(long userID1, long userID2, double value)
            {
                //Preconditions.checkArgument(value >= -1.0 && value <= 1.0, "Illegal value: " + value + ". Must be: -1.0 <= value <= 1.0");
                this.userID1 = userID1;
                this.userID2 = userID2;
                this.value = value;
            }

            public long getUserID1()
            {
                return userID1;
            }

            public long getUserID2()
            {
                return userID2;
            }

            public double getValue()
            {
                return value;
            }

            public override string ToString()
            {
                return "UserUserSimilarity[" + userID1 + ',' + userID2 + ':' + value + ']';
            }

            /// Defines an ordering from highest similarity to lowest. 
            public int CompareTo(UserUserSimilarity other)
            {
                double otherValue = other.getValue();
                return value > otherValue ? -1 : value < otherValue ? 1 : 0;
            }

            public override bool Equals(object other)
            {
                if (!(other is UserUserSimilarity))
                {
                    return false;
                }
                UserUserSimilarity otherSimilarity = (UserUserSimilarity)other;
                return otherSimilarity.getUserID1() == userID1
                    && otherSimilarity.getUserID2() == userID2
                    && otherSimilarity.getValue() == value;
            }

            public override int GetHashCode()
            {
                return (int)userID1 ^ (int)userID2 ^ RandomUtils.hashDouble(value);
            }
        }

        private sealed class DataModelSimilaritiesIterator : IEnumerator<UserUserSimilarity>
        {
            private IUserSimilarity otherSimilarity;
            private long[] itemIDs;
            private int i;
            private long itemID1;
            private int j;

            internal DataModelSimilaritiesIterator(IUserSimilarity otherSimilarity, long[] itemIDs)
            {
                this.otherSimilarity = otherSimilarity;
                this.itemIDs = itemIDs;
                i = 0;
                itemID1 = itemIDs[0];
                j = 1;
            }

            protected UserUserSimilarity computeNext()
            {
                int size = itemIDs.Length;
                while (i < size - 1)
                {
                    long itemID2 = itemIDs[j];
                    double similarity;
                    try
                    {
                        similarity = otherSimilarity.UserSimilarity(itemID1, itemID2);
                    }
                    catch (TasteException te)
                    {
                        // ugly:
                        throw new InvalidOperationException(te.Message, te);
                    }
                    if (!Double.IsNaN(similarity))
                    {
                        return new UserUserSimilarity(itemID1, itemID2, similarity);
                    }
                    if (++j == size)
                    {
                        itemID1 = itemIDs[++i];
                        j = i + 1;
                    }
                }
                return null;
            }

            UserUserSimilarity _Current;

            public UserUserSimilarity Current
            {
                get
                {
                    if (_Current == null)
                        throw new InvalidOperationException();
                    return _Current;
                }
            }

            public void Dispose()
            {

            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                _Current = computeNext();
                return _Current != null;
            }

            public void Reset()
            {
                _Current = null;
                i = 0;
                j = 1;
            }
        }
    }
}