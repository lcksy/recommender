using System.Collections.Generic;

namespace CQSS.Common.Infrastructure.Cluster
{
    public interface ICluster
    {
        string ClusterName { get; }

        List<IClusterNode> Nodes { get; }

        IClusterNodeFinder Finder { get; }

        void Add(IClusterNode node);

        IClusterNode GetNode();

        IEnumerable<IClusterNode> GetNodeRange();

        bool Matching(object arg);
    }
}