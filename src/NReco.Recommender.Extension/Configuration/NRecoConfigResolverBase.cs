using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace NReco.Recommender.Extension.Configuration
{
    public abstract class NRecoConfigResolverBase : INRecoConfigResolver
    {
        public IEnumerable<TOut> ResolveServerConfig<TOut>(XmlNode node, DBType type)
        {
            try
            {
                return this.DoResoveServerConfig<TOut>(node, type);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual IEnumerable<TOut> DoResoveServerConfig<TOut>(XmlNode node, DBType type)
        {
            if (node.HasChildNodes == false)
                throw new ArgumentNullException("no server node");

            var dbServer = node.ChildNodes.OfType<XmlNode>().Where(x => x.GetAttributeValue("name") == type.Name());

            var serverNodes = dbServer.SelectMany(x => x.ChildNodes.OfType<XmlNode>(), (x, n) => n);

            var nodes = new List<TOut>();

            foreach (XmlNode child in serverNodes)
            {
                var childNodes = this.ResolveNodes<TOut>(child);

                nodes.AddRange(childNodes);
            }

            return nodes;
        }

        protected virtual IEnumerable<TOut> ResolveNodes<TOut>(XmlNode node)
        {
            if (!node.HasChildNodes)
                throw new ArgumentNullException("no sql server connection string");

            var serverNodes = new List<TOut>();

            foreach (XmlNode server in node.ChildNodes)
            {
                var serverNode = server.Map<TOut>();

                serverNodes.Add(serverNode);
            }

            return serverNodes;
        }
    }
}