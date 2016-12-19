using System;

using CQSS.Common;
using CQSS.Common.Infrastructure.Engine;

namespace NReco.Recommender.Extension.Configuration
{
    public class NRecoConfigResolverFactory
    {
        public static INRecoConfigResolver Create(DBType type)
        {
            switch (type)
            {
                case DBType.SqlServer:
                    return EngineContext.Current.Resolve<INRecoConfigResolver>("sqlResolver");//new SqlServerConfigResolver();
                case DBType.MongoServer:
                    return EngineContext.Current.Resolve<INRecoConfigResolver>("mongoResolver");//new MongoDbConfigResolver();
                default:
                    throw new NotSupportedException("not supported db type" + type.ToString());
            }
        }
    }
}