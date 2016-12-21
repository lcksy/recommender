using CQSS.Common.Infrastructure.Configuration;
using CQSS.Common.Infrastructure.Engine;
using CQSS.Common.Infrastructure.ObjectContainer;
using NReco.Recommender.Extension.Configuration;
using NReco.Recommender.Extension.Recommender.DataModelResolver;
using NReco.Recommender.Extension.Recommender.DataReaderResolver;

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

            var sqlConfigResolver = new SqlServerConfigResolver();
            var mongoConfigResolver = new MongoDbConfigResolver();
            var sqlDataReader = new SqlServerDataReaderResolver();
            var mongoDataReader = new MongDbDataReaderResolver();
            var sqlDataModelResolver = new SqlServerDataModelResolver();
            var mongoDataModelResolver = new MongoDbDataModelResolver();

            base.Container.RegisterInstance<INRecoConfigResolver, SqlServerConfigResolver>(sqlConfigResolver, "sqlConfigResolver");
            base.Container.RegisterInstance<INRecoConfigResolver, MongoDbConfigResolver>(mongoConfigResolver, "mongoConfigResolver");
            base.Container.RegisterInstance<IDataReaderResolver, SqlServerDataReaderResolver>(sqlDataReader, "sqlDataReaderResolver");
            base.Container.RegisterInstance<IDataReaderResolver, MongDbDataReaderResolver>(mongoDataReader, "mongoDataReaderResolver");
            base.Container.RegisterInstance<DataModelResolverBase, SqlServerDataModelResolver>(sqlDataModelResolver, "sqlDataModelResolver");
            base.Container.RegisterInstance<DataModelResolverBase, MongoDbDataModelResolver>(mongoDataModelResolver, "mongoDataModelResolver");
        }
    }
}