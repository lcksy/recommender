using System;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>Simply encapsulates a user and a similarity value. </summary>
    public sealed class SimilarUser : IComparable<SimilarUser>
    {
        private long userID;
        private double similarity;

        public SimilarUser(long userID, double similarity)
        {
            this.userID = userID;
            this.similarity = similarity;
        }

        public long GetUserID()
        {
            return userID;
        }

        public double GetSimilarity()
        {
            return similarity;
        }

        public override int GetHashCode()
        {
            return (int)userID ^ RandomUtils.hashDouble(similarity);
        }

        public override bool Equals(object o)
        {
            if (!(o is SimilarUser))
            {
                return false;
            }
            SimilarUser other = (SimilarUser)o;
            return userID == other.GetUserID() && similarity == other.GetSimilarity();
        }

        public override string ToString()
        {
            return "SimilarUser[user:" + userID + ", similarity:" + similarity + ']';
        }

        /// Defines an ordering from most similar to least similar. 
        public int CompareTo(SimilarUser other)
        {
            double otherSimilarity = other.GetSimilarity();
            if (similarity > otherSimilarity)
            {
                return -1;
            }
            if (similarity < otherSimilarity)
            {
                return 1;
            }
            long otherUserID = other.GetUserID();
            if (userID < otherUserID)
            {
                return -1;
            }
            if (userID > otherUserID)
            {
                return 1;
            }
            return 0;
        }
    }
}