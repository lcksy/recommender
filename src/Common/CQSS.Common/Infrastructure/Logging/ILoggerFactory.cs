using System;

namespace CQSS.Common.Infrastructure.Logging
{
    public interface ILoggerFactory
    {
        ILogger Create(string name = "");

        ILogger Create(Type type);
    }
}