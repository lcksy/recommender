using System;
using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Similarity
{
    public abstract class AbstractItemSimilarity : IItemSimilarity
    {
        private IDataModel dataModel;
        private RefreshHelper refreshHelper;

        protected AbstractItemSimilarity(IDataModel dataModel)
        {
            //Preconditions.checkArgument(dataModel != null, "dataModel is null");
            this.dataModel = dataModel;
            this.refreshHelper = new RefreshHelper(null);
            refreshHelper.AddDependency(this.dataModel);
        }

        protected IDataModel GetDataModel()
        {
            return dataModel;
        }

        public abstract double ItemSimilarity(long itemID1, long itemID2);

        public abstract double[] ItemSimilarities(long itemID1, long[] itemID2s);

        public virtual long[] AllSimilarItemIDs(long itemID)
        {
            FastIDSet allSimilarItemIDs = new FastIDSet();
            var allItemIDs = dataModel.GetItemIDs();
            while (allItemIDs.MoveNext())
            {
                long possiblySimilarItemID = allItemIDs.Current;
                if (!Double.IsNaN(ItemSimilarity(itemID, possiblySimilarItemID)))
                {
                    allSimilarItemIDs.Add(possiblySimilarItemID);
                }
            }
            return allSimilarItemIDs.ToArray();
        }

        public virtual void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            refreshHelper.Refresh(alreadyRefreshed);
        }
    }
}