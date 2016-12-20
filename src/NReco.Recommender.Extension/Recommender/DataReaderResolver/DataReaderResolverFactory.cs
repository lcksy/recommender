using System;

using CQSS.Common.Extension;
using CQSS.Common.Infrastructure.Engine;
using NReco.Recommender.Extension.Configuration;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public class DataReaderResolverFactory
    {
        public static IDataReaderResolver Create()
        {
            var config = NRecoConfigResolver.Resolve<NRecoConfig>();

            switch (config.DBType)
            { 
                case DBType.SqlServer :
                    return EngineContext.Current.Resolve<IDataReaderResolver>("sqlDataReader");
                case DBType.MongoServer :
                    return EngineContext.Current.Resolve<IDataReaderResolver>("mongoDataReader");
                default :
                    throw new NotSupportedException("not supported:" + config.DBType.Name());
            }
        }
    }
}