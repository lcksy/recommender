using System;

using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Neighborhood
{
    /// <summary>
    /// Computes a neigbhorhood consisting of all users whose similarity to the given user meets or exceeds a
    /// certain threshold. Similarity is defined by the given <see cref="IUserSimilarity"/>.
    /// </summary>
    public sealed class ThresholdUserNeighborhood : AbstractUserNeighborhood
    {
        private double threshold;

        /// @param threshold
        ///          similarity threshold
        /// @param userSimilarity
        ///          similarity metric
        /// @param dataModel
        ///          data model
        /// @throws IllegalArgumentException
        ///           if threshold is {@link Double#NaN}, or if samplingRate is not positive and less than or equal
        ///           to 1.0, or if userSimilarity or dataModel are {@code null}
        public ThresholdUserNeighborhood(double threshold, IUserSimilarity userSimilarity, IDataModel dataModel) :
            this(threshold, userSimilarity, dataModel, 1.0)
        {

        }

        /// @param threshold
        ///          similarity threshold
        /// @param userSimilarity
        ///          similarity metric
        /// @param dataModel
        ///          data model
        /// @param samplingRate
        ///          percentage of users to consider when building neighborhood -- decrease to trade quality for
        ///          performance
        /// @throws IllegalArgumentException
        ///           if threshold or samplingRate is {@link Double#NaN}, or if samplingRate is not positive and less
        ///           than or equal to 1.0, or if userSimilarity or dataModel are {@code null}
        public ThresholdUserNeighborhood(double threshold,
                                         IUserSimilarity userSimilarity,
                                         IDataModel dataModel,
                                         double samplingRate)
            : base(userSimilarity, dataModel, samplingRate)
        {

            //Preconditions.checkArgument(!Double.isNaN(threshold), "threshold must not be NaN");
            this.threshold = threshold;
        }

        public override long[] GetUserNeighborhood(long userID)
        {

            IDataModel dataModel = GetDataModel();
            FastIDSet neighborhood = new FastIDSet();
            var usersIterable = SamplinglongPrimitiveIterator.MaybeWrapIterator(dataModel
                .GetUserIDs(), GetSamplingRate());
            IUserSimilarity userSimilarityImpl = GetUserSimilarity();

            while (usersIterable.MoveNext())
            {
                long otherUserID = usersIterable.Current;
                if (userID != otherUserID)
                {
                    double theSimilarity = userSimilarityImpl.UserSimilarity(userID, otherUserID);
                    if (!Double.IsNaN(theSimilarity) && theSimilarity >= threshold)
                    {
                        neighborhood.Add(otherUserID);
                    }
                }
            }

            return neighborhood.ToArray();
        }

        public override string ToString()
        {
            return "ThresholdUserNeighborhood";
        }
    }
}