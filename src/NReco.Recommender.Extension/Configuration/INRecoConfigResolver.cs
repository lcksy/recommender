using System.Collections.Generic;
using System.Xml;

namespace NReco.Recommender.Extension.Configuration
{
    public interface INRecoConfigResolver
    {
        IEnumerable<TOut> ResolveServerConfig<TOut>(XmlNode node, DBType type);
    }
}