using System.Collections.Generic;

namespace CQSS.Common.Infrastructure.Cluster
{
    public interface IClusterNodeFinder
    {
        IClusterNode Find();

        IEnumerable<IClusterNode> FindRange();

        void Add(IClusterNode node);
    }
}