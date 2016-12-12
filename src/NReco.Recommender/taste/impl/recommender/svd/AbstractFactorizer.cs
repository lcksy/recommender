using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Recommender.SVD
{
    /// <summary>
    /// Base class for <see cref="IFactorizer"/>s, provides ID to index mapping
    /// </summary>
    public abstract class AbstractFactorizer : IFactorizer
    {
        private IDataModel dataModel;
        private FastByIDMap<int?> userIDMapping;
        private FastByIDMap<int?> itemIDMapping;
        private RefreshHelper refreshHelper;

        protected AbstractFactorizer(IDataModel dataModel)
        {
            this.dataModel = dataModel;
            BuildMappings();
            refreshHelper = new RefreshHelper(() =>
            {
                BuildMappings();
            });
            refreshHelper.AddDependency(dataModel);
        }

        public abstract Factorization Factorize();

        private void BuildMappings()
        {
            userIDMapping = CreateIDMapping(dataModel.GetNumUsers(), dataModel.GetUserIDs());
            itemIDMapping = CreateIDMapping(dataModel.GetNumItems(), dataModel.GetItemIDs());
        }

        protected Factorization CreateFactorization(double[][] userFeatures, double[][] itemFeatures)
        {
            return new Factorization(userIDMapping, itemIDMapping, userFeatures, itemFeatures);
        }

        protected int UserIndex(long userID)
        {
            int? userIndex = userIDMapping.Get(userID);
            if (userIndex == null)
            {
                userIndex = userIDMapping.Count();
                userIDMapping.Put(userID, userIndex);
            }
            return userIndex.Value;
        }

        protected int ItemIndex(long itemID)
        {
            int? itemIndex = itemIDMapping.Get(itemID);
            if (itemIndex == null)
            {
                itemIndex = itemIDMapping.Count();
                itemIDMapping.Put(itemID, itemIndex);
            }
            return itemIndex.Value;
        }

        private static FastByIDMap<int?> CreateIDMapping(int size, IEnumerator<long> idIterator)
        {
            var mapping = new FastByIDMap<int?>(size);
            int index = 0;
            while (idIterator.MoveNext())
            {
                mapping.Put(idIterator.Current, index++);
            }
            return mapping;
        }

        public void Refresh(IList<IRefreshable> AlreadyRefreshed)
        {
            refreshHelper.Refresh(AlreadyRefreshed);
        }
    }
}