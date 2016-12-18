using CQSS.Common.Infrastructure.Configuration;
using CQSS.Common.Infrastructure.Engine;
using CQSS.Common.Infrastructure.ObjectContainer;
using NReco.Recommender.Extension.Configuration;
using NReco.Recommender.Extension.Recommender.DataReaderResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NReco.Recommender.Engine
{
    public class RecommenderEngine : DefaultEngine
    {
        public RecommenderEngine(IObjectContainer container)
            : base(container)
        { 
        }

        protected override void RegisterCommonComponents(CQSSConfig config)
        {
            base.RegisterCommonComponents(config);

            var sqlResolver = new SqlServerConfigResolver();
            var redisResolver = new RedisConfigResolver();
            var mongoResolver = new MongoDbConfigResolver();
            var sqlDataReader = new SqlServerDataReaderResolver();

            base.Container.RegisterInstance<INRecoConfigResolver, SqlServerConfigResolver>(sqlResolver, "sqlResolver");
            base.Container.RegisterInstance<INRecoConfigResolver, RedisConfigResolver>(redisResolver, "redisResolver");
            base.Container.RegisterInstance<INRecoConfigResolver, MongoDbConfigResolver>(mongoResolver, "mongoResolver");
            base.Container.RegisterInstance<IDataReaderResolver, SqlServerDataReaderResolver>(sqlDataReader, "sqlDataReader");
        }
    }
}