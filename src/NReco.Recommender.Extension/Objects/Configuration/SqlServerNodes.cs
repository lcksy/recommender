using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NReco.Recommender.Extension.Objects.Configuration
{
    public class SqlServerNodes
    {
        public List<ServerNode> ReadServers { get; set; }
        public List<ServerNode> WriteServers { get; set; }

        public SqlServerNodes()
        {
            this.ReadServers = new List<ServerNode>();
            this.WriteServers = new List<ServerNode>();
        }
    }
}