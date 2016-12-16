using System.Collections.Generic;
using System.Xml;

namespace NReco.Recommender.Extension.Configuration
{
    public class SqlServerConfigResolver : NRecoConfigResolverBase
    {
        protected override IEnumerable<TOut> DoResoveServerConfig<TOut>(XmlNode node, DBType type)
        {
            return base.DoResoveServerConfig<TOut>(node, type);
        }
    }
}