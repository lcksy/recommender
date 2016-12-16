using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CQSS.Common.Infrastructure.Logging
{
    public class EmptyLoggerFactory : ILoggerFactory
    {
        public ILogger Create(string name = "")
        {
            if (Singleton<EmptyLogger>.Instance == null)
                Singleton<EmptyLogger>.Instance = new EmptyLogger();

            return Singleton<EmptyLogger>.Instance;
        }

        public ILogger Create(Type type)
        {
            if (Singleton<EmptyLogger>.Instance == null)
                Singleton<EmptyLogger>.Instance = new EmptyLogger();

            return Singleton<EmptyLogger>.Instance;
        }
    }
}