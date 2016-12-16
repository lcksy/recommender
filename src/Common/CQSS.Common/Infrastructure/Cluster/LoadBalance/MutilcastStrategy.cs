using System;
using System.Collections.Generic;

namespace CQSS.Common.Infrastructure.Cluster
{
    public class MutilcastStrategy : LoadBalanceStrategyBase
    {
        protected override IClusterNode InnerFind()
        {
            throw new NotSupportedException("MutilcastStrategy not support [method=InnerFind].");
        }

        protected override IEnumerable<IClusterNode> InnerFindRange()
        {
            return base.Nodes;
        }
    }
}