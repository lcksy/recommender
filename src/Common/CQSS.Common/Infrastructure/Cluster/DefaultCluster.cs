using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQSS.Common.Infrastructure.Cluster
{
    public class DefaultCluster : ICluster
    {
        public string ClusterName { get; private set; }
        public List<IClusterNode> Nodes { get; private set; }
        public IClusterNodeFinder Finder { get; private set; }

        public DefaultCluster(string clusterName, IClusterNodeFinder finder)
        {
            if (finder == null)
                throw new ArgumentNullException("finder");

            this.ClusterName = clusterName;
            this.Finder = finder;
            this.Nodes = new List<IClusterNode>();
        }

        public void Add(IClusterNode node)
        {
            if (string.Equals(this.ClusterName, node.ClusterName, StringComparison.OrdinalIgnoreCase))
            {
                this.Nodes.Add(node);
                this.Finder.Add(node);
            }
        }

        public IClusterNode GetNode()
        {
            return this.Finder.Find();
        }

        public IEnumerable<IClusterNode> GetNodeRange()
        {
            return this.Finder.FindRange();
        }

        public bool Matching(object arg)
        {
            return this.Nodes.Any() ? this.Nodes.First().Matching(arg) : false;
        }
    }
}