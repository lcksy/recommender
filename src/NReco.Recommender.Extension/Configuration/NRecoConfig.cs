using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

using NReco.Recommender.Extension.Objects.Configuration;

namespace NReco.Recommender.Extension.Configuration
{
    public class NRecoConfig : IConfigurationSectionHandler
    {
        #region prop
        public DBType DBType { get; set; }
        public IEnumerable<ServerNode> ServerNodes { get; set; }
        #endregion

        #region actor
        private NRecoConfig()
        {
            this.ServerNodes = Enumerable.Empty<ServerNode>();
        }
        #endregion

        public object Create(object parent, object configContext, XmlNode section)
        {
            var node = section.ChildNodes.Item(0);

            var dbType = node.GetAttributeValue("name").ToEnumByName<DBType>();

            this.DBType = dbType;

            this.ServerNodes = NRecoConfigResolverFactory.Create(dbType).ResoveServerConfig<ServerNode>(node, dbType);

            return this;
        }
    }
}