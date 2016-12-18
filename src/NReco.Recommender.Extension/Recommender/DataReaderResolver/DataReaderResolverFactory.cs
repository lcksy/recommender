using System;

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
                    return new SqlServerDataReaderResolver();
                case DBType.MongoServer :
                    return new MongDbDataReaderResolver();
                case DBType.RedisServer :
                default :
                    throw new NotSupportedException("not supported:" + config.DBType.Name());
            }
        }
    }
}