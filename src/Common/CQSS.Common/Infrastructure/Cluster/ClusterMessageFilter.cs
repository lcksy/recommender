using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace CQSS.Common.Infrastructure.Cluster
{
    public sealed class ClusterMessageFilter : MessageFilter, IClusterNode
    {
        public string ClusterName { get; private set; }
        private string Address { get; set; }
        private EndpointAddressMessageFilter InnerFilter { get; set; }
        private string FinderFullTypeName { get; set; }
        private string FinderFullAssemblyName { get; set; }

        public ClusterMessageFilter(string argString)
        {
            if (string.IsNullOrEmpty(argString))
                throw new ArgumentNullException("argString");

            var args = argString.Split('|');
            if (args == null || args.Count() != 3)
                throw new ArgumentException("argString");

            var address = args[0];
            var clusterName = args[1];
            var finderFullTypeName = args[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).First();
            var finderFullAssemblyName = args[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Last();

            this.InnerFilter = new EndpointAddressMessageFilter(new EndpointAddress(address));
            this.Address = address;
            this.ClusterName = clusterName;
            this.FinderFullTypeName = finderFullTypeName;
            this.FinderFullAssemblyName = finderFullAssemblyName;
        }

        protected override IMessageFilterTable<TFilterData> CreateFilterTable<TFilterData>()
        {
            return new ClusterMessageFilterTable<TFilterData>();
        }

        public override bool Match(Message message)
        {
            return this.InnerFilter.Match(message);
        }

        public override bool Match(MessageBuffer buffer)
        {
            return this.InnerFilter.Match(buffer);
        }

        public bool Matching(object arg)
        {
            return arg is Message ? this.Match((Message)arg) : false;
        }

        public IClusterNodeFinder CreateClusterNodeFinder()
        {
            var assembly = Assembly.Load(this.FinderFullAssemblyName);
            var type = assembly.GetType(this.FinderFullTypeName);
            object instance = Activator.CreateInstance(type);
            return (IClusterNodeFinder)instance;
        }
    }
}