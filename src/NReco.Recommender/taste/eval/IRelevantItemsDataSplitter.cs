using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Eval
{
    /// Implementations of this interface determine the items that are considered relevant,
    /// and splits data into a training and test subset, for purposes of precision/recall
    /// tests as implemented by implementations of {@link RecommenderIRStatsEvaluator}.
    public interface IRelevantItemsDataSplitter
    {
        /// During testing, relevant items are removed from a particular users' preferences,
        /// and a model is build using this user's other preferences and all other users.
        ///
        /// @param at                 Maximum number of items to be removed
        /// @param relevanceThreshold Minimum strength of preference for an item to be considered
        ///                           relevant
        /// @return IDs of relevant items
        FastIDSet GetRelevantItemsIDs(long userID,
                                      int at,
                                      double relevanceThreshold,
                                      IDataModel dataModel);

        /// Adds a single user and all their preferences to the training model.
        ///
        /// @param userID          ID of user whose preferences we are trying to predict
        /// @param relevantItemIDs IDs of items considered relevant to that user
        /// @param trainingUsers   the database of training preferences to which we will
        ///                        append the ones for otherUserID.
        /// @param otherUserID     for whom we are adding preferences to the training model
        void ProcessOtherUser(long userID,
                              FastIDSet relevantItemIDs,
                              FastByIDMap<IPreferenceArray> trainingUsers,
                              long otherUserID,
                              IDataModel dataModel);
    }
}