using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Recommender;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// Abstract base implementation for retrieving candidate items to recommend
    /// </summary>
    public abstract class AbstractCandidateItemsStrategy : ICandidateItemsStrategy,
        IMostSimilarItemsCandidateItemsStrategy
    {
        public FastIDSet GetCandidateItems(long userID, IPreferenceArray preferencesFromUser, IDataModel dataModel)
        {
            return DoGetCandidateItems(preferencesFromUser.GetIDs(), dataModel);
        }

        public FastIDSet GetCandidateItems(long[] itemIDs, IDataModel dataModel)
        {
            return DoGetCandidateItems(itemIDs, dataModel);
        }

        protected abstract FastIDSet DoGetCandidateItems(long[] preferredItemIDs, IDataModel dataModel);

        public virtual void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
        }
    }
}