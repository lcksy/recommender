using System.Collections.Generic;

namespace NReco.Recommender.Extension.Objects.Configuration
{
    public class RedisServerNodes
    {
        public List<ServerNode> Servers { get; set; }

        public RedisServerNodes()
        {
            this.Servers = new List<ServerNode>();
        }
    }
}