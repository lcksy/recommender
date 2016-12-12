using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Recommender
{
    public abstract class AbstractRecommender : IRecommender
    {
        private static Logger log = LoggerFactory.GetLogger(typeof(AbstractRecommender));

        private IDataModel dataModel;
        private ICandidateItemsStrategy candidateItemsStrategy;

        protected AbstractRecommender(IDataModel dataModel, ICandidateItemsStrategy candidateItemsStrategy)
        {
            this.dataModel = dataModel; //Preconditions.checkNotNull(dataModel);
            this.candidateItemsStrategy = candidateItemsStrategy; // Preconditions.checkNotNull(candidateItemsStrategy);
        }

        protected AbstractRecommender(IDataModel dataModel)
            : this(dataModel, GetDefaultCandidateItemsStrategy())
        {
        }

        protected static ICandidateItemsStrategy GetDefaultCandidateItemsStrategy()
        {
            return new PreferredItemsNeighborhoodCandidateItemsStrategy();
        }

        /// <p>
        /// Default implementation which just calls
        /// {@link Recommender#recommend(long, int, NReco.CF.Taste.Recommender.IDRescorer)}, with a
        /// {@link NReco.CF.Taste.Recommender.Rescorer} that does nothing.
        /// </p>
        public virtual IList<IRecommendedItem> Recommend(long userID, int howMany)
        {
            return Recommend(userID, howMany, null);
        }

        public abstract IList<IRecommendedItem> Recommend(long userID, int howMany, IDRescorer rescorer);

        public abstract float EstimatePreference(long userID, long itemID);

        /// <p>
        /// Default implementation which just calls {@link DataModel#setPreference(long, long, float)}.
        /// </p>
        ///
        /// @throws IllegalArgumentException
        ///           if userID or itemID is {@code null}, or if value is {@link Double#NaN}
        public virtual void SetPreference(long userID, long itemID, float value)
        {
            //Preconditions.checkArgument(!Float.isNaN(value), "NaN value");
            log.Debug("Setting preference for user {}, item {}", userID, itemID);
            dataModel.SetPreference(userID, itemID, value);
        }

        /// <p>
        /// Default implementation which just calls {@link DataModel#removePreference(long, long)} (Object, Object)}.
        /// </p>
        ///
        /// @throws IllegalArgumentException
        ///           if userID or itemID is {@code null}
        public virtual void RemovePreference(long userID, long itemID)
        {
            log.Debug("Remove preference for user '{}', item '{}'", userID, itemID);
            dataModel.RemovePreference(userID, itemID);
        }

        public virtual IDataModel GetDataModel()
        {
            return dataModel;
        }

        /// @param userID
        ///          ID of user being evaluated
        /// @param preferencesFromUser
        ///          the preferences from the user
        /// @return all items in the {@link DataModel} for which the user has not expressed a preference and could
        ///         possibly be recommended to the user
        /// @throws TasteException
        ///           if an error occurs while listing items
        protected virtual FastIDSet GetAllOtherItems(long userID, IPreferenceArray preferencesFromUser)
        {
            return candidateItemsStrategy.GetCandidateItems(userID, preferencesFromUser, dataModel);
        }

        public abstract void Refresh(IList<IRefreshable> alreadyRefreshed);
    }
}