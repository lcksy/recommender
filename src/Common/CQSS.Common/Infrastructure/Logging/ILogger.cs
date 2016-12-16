using System;

namespace CQSS.Common.Infrastructure.Logging
{
    public interface ILogger
    {
        bool IsDebugEnable { get; }

        void DebugFormat(string format, params object[] args);

        void Debug(object message, Exception exception = null);

        void InfoFormat(string format, params object[] args);

        void Info(object message, Exception exception = null);

        void ErrorFormat(string format, params object[] args);

        void Error(object message, Exception exception = null);

        void WarnFormat(string format, params object[] args);

        void Warn(object message, Exception exception = null);

        void FatalFormat(string format, params object[] args);

        void Fatal(object message, Exception exception = null);
    }
}