using System;
using CQSS.Common.Infrastructure.Configuration;
using CQSS.Common.Infrastructure.ObjectContainer;

namespace CQSS.Common.Infrastructure.Engine
{
    public interface IEngine
    {
        IObjectContainer Container { get; }

        TService Resolve<TService>(string registerName = "") where TService : class;

        object Resolve(Type serviceType, string registerName = "");

        void Initialize(CQSSConfig config);
    }
}