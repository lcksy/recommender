using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQSS.Common.Infrastructure.Engine;
using CQSS.Common.Infrastructure.Logging;

namespace CQSS.Common.Logging.Log4net
{
    public static class EngineExtension
    {
        public static IEngine UseLog4net(this IEngine engine, string configFile = "")
        {
            if (string.IsNullOrEmpty(configFile))
                configFile = "log4net.config";

            var loggerFactory = new Log4netLoggerFactory(configFile);
            engine.Container.RegisterInstance<ILoggerFactory, Log4netLoggerFactory>(loggerFactory);
            return engine;
        }
    }
}