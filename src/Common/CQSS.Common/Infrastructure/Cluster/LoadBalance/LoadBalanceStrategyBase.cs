using System.Collections.Generic;

namespace CQSS.Common.Infrastructure.Cluster
{
    public abstract class LoadBalanceStrategyBase : ClusterNodeFinderBase
    {
        protected override IEnumerable<IClusterNode> InnerFindRange()
        {
            yield return base.Find();
        }
    }
}