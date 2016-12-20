using System;

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
                    return new SqlServerDataModelResolver();
                case DBType.MongoServer:
                    return new MongoDbDataModelResolver();
                default:
                    throw new NotSupportedException("not support " + config.DBType.Name());
            }
        }
    }
}