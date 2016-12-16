using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CQSS.Common.Infrastructure.Logging
{
    public class EmptyLogger : ILogger
    {
        public bool IsDebugEnable
        {
            get { return false; }
        }

        public void DebugFormat(string format, params object[] args) { }

        public void Debug(object message, Exception exception = null) { }

        public void InfoFormat(string format, params object[] args) { }

        public void Info(object message, Exception exception = null) { }

        public void ErrorFormat(string format, params object[] args) { }

        public void Error(object message, Exception exception = null) { }

        public void WarnFormat(string format, params object[] args) { }

        public void Warn(object message, Exception exception = null) { }

        public void FatalFormat(string format, params object[] args) { }

        public void Fatal(object message, Exception exception = null) { }
    }
}
