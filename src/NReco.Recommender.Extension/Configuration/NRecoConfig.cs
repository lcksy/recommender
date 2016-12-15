using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

using NReco.Recommender.Extension.Objects.Configuration;
using System.Collections.Concurrent;

namespace NReco.Recommender.Extension.Configuration
{
    public class NRecoConfig : IConfigurationSectionHandler
    {
        #region prop
        public DBType Current { get; private set; }
        public NRecoConfigUseage Useage { get; private set; } 
        #endregion

        #region actor
        public NRecoConfig()
        {
            this.Useage = new NRecoConfigUseage();

            this.Register();
        }
        #endregion

        private void Register()
        { 
            var sqlserver = new MongDbServerConfig();
            var redisserver = new RedisServerConfig();
            var mongodbserver = new MongoDbServerConfig();

            IOC.Register<INRecoConfig, MongDbServerConfig>(sqlserver, "sqlserver");
            IOC.Register<INRecoConfig, RedisServerConfig>(redisserver, "redis");
            IOC.Register<INRecoConfig, MongoDbServerConfig>(mongodbserver, "mongodb");
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var node = section.ChildNodes.Item(0);

            this.Current = node.GetAttributeValue("name").ToEnumByName<DBType>();

            if (this.Current == DBType.SqlServer)
                this.Useage.SqlServerNodes = this.ResoveServerConfig<SqlServerNodes>(node, DBType.SqlServer);
            else if (this.Current == DBType.RedisServer)
                this.Useage.RedisServerNodes = this.ResoveServerConfig<RedisServerNodes>(node, DBType.RedisServer);
            else if (this.Current == DBType.MongoServer)
                this.Useage.MongoDbServerNodes = this.ResoveServerConfig<MongoDbServerNodes>(node, DBType.MongoServer);
            else
                throw new ArgumentException("no such server type:" + this.Current.ToString());

            return this;
        }

        private TServer ResoveServerConfig<TServer>(XmlNode node, DBType type)
        {
            if (node.HasChildNodes == false)
                throw new ArgumentNullException("no server node");

            var dbServer = node.ChildNodes.OfType<XmlNode>().Where(x => x.GetAttributeValue("name") == type.Name());

            var serverNodes = dbServer.SelectMany(x => x.ChildNodes.OfType<XmlNode>(), (x, n) => n);

            var nodes = Activator.CreateInstance<TServer>();

            foreach (XmlNode child in serverNodes)
            {
                //var childNodes = ResolveNodes(child);

                var childNodes = IOC.Resolve<INRecoConfig>().ResolveNodes<ServerNode>(child, type);

                nodes.GetType().GetProperty(child.Name).SetValue(nodes, childNodes);
            }

            return nodes;
        }

        private List<ServerNode> ResolveNodes(XmlNode node)
        {
            if (!node.HasChildNodes)
                throw new ArgumentNullException("no sql server connection string");

            var serverNodes = new List<ServerNode>();

            foreach (XmlNode server in node.ChildNodes)
            {
                var serverNode = server.Map<ServerNode>();

                serverNodes.Add(serverNode);
            }

            return serverNodes;
        }
    }

    public interface INRecoConfig
    {
        List<TOut> ResolveNodes<TOut>(XmlNode node, DBType type);
    }

    public abstract class NRecoConfigBase : INRecoConfig
    {
        public virtual List<TOut> ResolveNodes<TOut>(XmlNode node, DBType type)
        {
            throw new NotImplementedException();
        }
    }

    public class MongDbServerConfig : NRecoConfigBase
    {
        public override List<TOut> ResolveNodes<TOut>(XmlNode node, DBType type)
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

    public class RedisServerConfig : NRecoConfigBase
    {
        public override List<TOut> ResolveNodes<TOut>(XmlNode node, DBType type)
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

    public class MongoDbServerConfig : NRecoConfigBase
    {
        public override List<TOut> ResolveNodes<TOut>(XmlNode node, DBType type)
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

    public class IOC
    {
        private static readonly ConcurrentDictionary<string, object> dictionary = new ConcurrentDictionary<string, object>();

        public static void Register<TInterface, TImplement>(TImplement instance, string name = "")
        {
            if(string.IsNullOrEmpty(name))
                dictionary.TryAdd(typeof(TInterface).Name, instance);
            else
                dictionary.TryAdd(name, instance);
        }

        public static T Resolve<T>(string name = "")
        {
            object instance = null;

            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            if (dictionary.TryGetValue(name, out instance))
                return (T)instance;
            else
                return default(T);
        }
    }
}