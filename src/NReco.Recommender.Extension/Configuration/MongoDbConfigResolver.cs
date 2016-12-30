using System.Collections.Generic;
using System.Xml;

namespace NReco.Recommender.Extension.Configuration
{
    public class MongoDbConfigResolver : NRecoConfigResolverBase
    {
        protected override IEnumerable<TOut> DoResoveServerConfig<TOut>(XmlNode node, DBType type)
        {
            return base.DoResoveServerConfig<TOut>(node, type);
        }

        protected override IEnumerable<TOut> ResolveNodes<TOut>(XmlNode node)
        {
            return base.ResolveNodes<TOut>(node);
        }
    }
}