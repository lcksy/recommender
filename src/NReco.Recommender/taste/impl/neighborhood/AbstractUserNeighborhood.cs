using System.Collections.Generic;

using NReco.CF.Taste.Common;
using NReco.CF.Taste.Impl.Common;
using NReco.CF.Taste.Model;
using NReco.CF.Taste.Neighborhood;
using NReco.CF.Taste.Similarity;

namespace NReco.CF.Taste.Impl.Neighborhood
{
    /// <summary>
    /// Contains methods and resources useful to all classes in this package.
    /// </summary>
    public abstract class AbstractUserNeighborhood : IUserNeighborhood
    {
        private IUserSimilarity userSimilarity;
        private IDataModel dataModel;
        private double samplingRate;
        private RefreshHelper refreshHelper;

        public AbstractUserNeighborhood(IUserSimilarity userSimilarity, IDataModel dataModel, double samplingRate)
        {
            //Preconditions.checkArgument(userSimilarity != null, "userSimilarity is null");
            //Preconditions.checkArgument(dataModel != null, "dataModel is null");
            //Preconditions.checkArgument(samplingRate > 0.0 && samplingRate <= 1.0, "samplingRate must be in (0,1]");
            this.userSimilarity = userSimilarity;
            this.dataModel = dataModel;
            this.samplingRate = samplingRate;
            this.refreshHelper = new RefreshHelper(null);
            this.refreshHelper.AddDependency(this.dataModel);
            this.refreshHelper.AddDependency(this.userSimilarity);
        }

        public abstract long[] GetUserNeighborhood(long userID);

        public virtual IUserSimilarity GetUserSimilarity()
        {
            return userSimilarity;
        }

        public virtual IDataModel GetDataModel()
        {
            return dataModel;
        }

        public virtual double GetSamplingRate()
        {
            return samplingRate;
        }

        public virtual void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            refreshHelper.Refresh(alreadyRefreshed);
        }
    }
}