using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;

namespace NReco.CF.Taste.Impl.Recommender.SVD
{
    /// <summary>
    /// A factorization of the rating matrix
    /// </summary>
    public class Factorization
    {
        /// used to find the rows in the user features matrix by userID 
        private FastByIDMap<int?> userIDMapping;
        /// used to find the rows in the item features matrix by itemID 
        private FastByIDMap<int?> itemIDMapping;

        /// user features matrix 
        private double[][] userFeatures;
        /// item features matrix 
        private double[][] itemFeatures;

        public Factorization(FastByIDMap<int?> userIDMapping, FastByIDMap<int?> itemIDMapping, double[][] userFeatures,
            double[][] itemFeatures)
        {
            this.userIDMapping = userIDMapping; //Preconditions.checkNotNull(
            this.itemIDMapping = itemIDMapping; //Preconditions.checkNotNull();
            this.userFeatures = userFeatures;
            this.itemFeatures = itemFeatures;
        }

        public double[][] AllUserFeatures()
        {
            return userFeatures;
        }

        public virtual double[] GetUserFeatures(long userID)
        {
            int? index = userIDMapping.Get(userID);
            if (index == null)
            {
                throw new NoSuchUserException(userID);
            }
            return userFeatures[index.Value];
        }

        public double[][] AllItemFeatures()
        {
            return itemFeatures;
        }

        public virtual double[] GetItemFeatures(long itemID)
        {
            int? index = itemIDMapping.Get(itemID);
            if (index == null)
            {
                throw new NoSuchItemException(itemID);
            }
            return itemFeatures[index.Value];
        }

        public int UserIndex(long userID)
        {
            int? index = userIDMapping.Get(userID);
            if (index == null)
            {
                throw new NoSuchUserException(userID);
            }
            return index.Value;
        }

        public IEnumerable<KeyValuePair<long, int?>> GetUserIDMappings()
        {
            return userIDMapping.EntrySet();
        }

        public IEnumerator<long> GetUserIDMappingKeys()
        {
            return userIDMapping.Keys.GetEnumerator();
        }

        public int ItemIndex(long itemID)
        {
            int? index = itemIDMapping.Get(itemID);
            if (index == null)
            {
                throw new NoSuchItemException(itemID);
            }
            return index.Value;
        }

        public IEnumerable<KeyValuePair<long, int?>> GetItemIDMappings()
        {
            return itemIDMapping.EntrySet();
        }

        public IEnumerator<long> GetItemIDMappingKeys()
        {
            return itemIDMapping.Keys.GetEnumerator();
        }

        public int NumFeatures()
        {
            return userFeatures.Length > 0 ? userFeatures[0].Length : 0;
        }

        public int NumUsers()
        {
            return userIDMapping.Count();
        }

        public int NumItems()
        {
            return itemIDMapping.Count();
        }

        public override bool Equals(object o)
        {
            if (o is Factorization)
            {
                Factorization other = (Factorization)o;
                return userIDMapping.Equals(other.userIDMapping) && itemIDMapping.Equals(other.itemIDMapping)
                    && Utils.ArrayDeepEquals(userFeatures, other.userFeatures) && Utils.ArrayDeepEquals(itemFeatures, other.itemFeatures);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 31 * userIDMapping.GetHashCode() + itemIDMapping.GetHashCode();
            hashCode = 31 * hashCode + Utils.GetArrayDeepHashCode(userFeatures);
            hashCode = 31 * hashCode + Utils.GetArrayDeepHashCode(itemFeatures);
            return hashCode;
        }
    }
}