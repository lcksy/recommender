namespace CQSS.Common.Infrastructure.Cluster
{
    public interface IClusterNode
    {
        string ClusterName { get; }

        bool Matching(object arg);

        IClusterNodeFinder CreateClusterNodeFinder();
    }
}