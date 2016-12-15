using System.Collections.Generic;

namespace NReco.Recommender.Extension.Objects.Configuration
{
    public class MongoDbServerNodes
    {
        public List<ServerNode> Servers { get; set; }
        public MongoDbServerNodes()
        {
            this.Servers = new List<ServerNode>();
        }
    }
}