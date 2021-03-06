using System;

using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Impl.Recommender;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Neighborhood
{
    /// <summary>
    /// Computes a neighborhood consisting of the nearest n users to a given user. "Nearest" is defined by the
    /// given <see cref="IUserSimilarity"/>.
    /// </summary>
    public sealed class NearestNUserNeighborhood : AbstractUserNeighborhood
    {
        private int n;
        private double minSimilarity;

        /// @param n neighborhood size; capped at the number of users in the data model
        /// @throws IllegalArgumentException
        ///           if {@code n < 1}, or userSimilarity or dataModel are {@code null}
        public NearestNUserNeighborhood(int n, IUserSimilarity userSimilarity, IDataModel dataModel) :
            this(n, Double.NegativeInfinity, userSimilarity, dataModel, 1.0)
        {
        }

        /// @param n neighborhood size; capped at the number of users in the data model
        /// @param minSimilarity minimal similarity required for neighbors
        /// @throws IllegalArgumentException
        ///           if {@code n < 1}, or userSimilarity or dataModel are {@code null}
        public NearestNUserNeighborhood(int n,
                                        double minSimilarity,
                                        IUserSimilarity userSimilarity,
                                        IDataModel dataModel) :
            this(n, minSimilarity, userSimilarity, dataModel, 1.0)
        {
        }

        /// @param n neighborhood size; capped at the number of users in the data model
        /// @param minSimilarity minimal similarity required for neighbors
        /// @param samplingRate percentage of users to consider when building neighborhood -- decrease to trade quality for
        ///   performance
        /// @throws IllegalArgumentException
        ///           if {@code n < 1} or samplingRate is NaN or not in (0,1], or userSimilarity or dataModel are
        ///           {@code null}
        public NearestNUserNeighborhood(int n,
                                        double minSimilarity,
                                        IUserSimilarity userSimilarity,
                                        IDataModel dataModel,
                                        double samplingRate)
            : base(userSimilarity, dataModel, samplingRate)
        {
            //Preconditions.checkArgument(n >= 1, "n must be at least 1");
            int numUsers = dataModel.GetNumUsers();
            this.n = n > numUsers ? numUsers : n;
            this.minSimilarity = minSimilarity;
        }

        public override long[] GetUserNeighborhood(long userID)
        {

            IDataModel dataModel = GetDataModel();
            IUserSimilarity userSimilarityImpl = GetUserSimilarity();

            TopItems.IEstimator<long> estimator = new Estimator(userSimilarityImpl, userID, minSimilarity);

            var userIDs = SamplinglongPrimitiveIterator.MaybeWrapIterator(dataModel.GetUserIDs(),
              GetSamplingRate());
            return TopItems.GetTopUsers(n, userIDs, null, estimator);
        }

        public override string ToString()
        {
            return "NearestNUserNeighborhood";
        }

        private sealed class Estimator : TopItems.IEstimator<long>
        {
            private IUserSimilarity userSimilarityImpl;
            private long theUserID;
            private double minSim;

            internal Estimator(IUserSimilarity userSimilarityImpl, long theUserID, double minSim)
            {
                this.userSimilarityImpl = userSimilarityImpl;
                this.theUserID = theUserID;
                this.minSim = minSim;
            }

            public double Estimate(long userID)
            {
                if (userID == theUserID)
                {
                    return Double.NaN;
                }
                double sim = userSimilarityImpl.UserSimilarity(theUserID, userID);
                return sim >= minSim ? sim : Double.NaN;
            }
        }
    }
}