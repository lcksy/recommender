using System;

using CQSS.Common.Infrastructure.Engine;
using NReco.CF.Taste.Model;
using NReco.Recommender.Extension.Configuration;

namespace NReco.Recommender.Extension.Recommender.DataModelResolver
{
    public class DataModelResolverFactory
    {
        public static DataModelResolverBase Create()
        {
            var config = NRecoConfigResolver.Resolve<NRecoConfig>();

            switch (config.DBType)
            {
                case DBType.SqlServer:
                    var resolver = EngineContext.Current.Resolve<DataModelResolverBase>("sqlDataModelResolver");
                    var hash = resolver.GetHashCode();
                    return resolver;
                case DBType.MongoServer:
                    return EngineContext.Current.Resolve<DataModelResolverBase>("mongoDataModelResolver");
                default:
                    throw new NotSupportedException("not support " + config.DBType.Name());
            }
        }
    }
}