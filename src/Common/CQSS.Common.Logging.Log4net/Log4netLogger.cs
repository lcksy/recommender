using System;
using CQSS.Common.Infrastructure.Logging;
using log4net;

namespace CQSS.Common.Logging.Log4net
{
    public class Log4netLogger : ILogger
    {
        private readonly ILog _log;

        public Log4netLogger(ILog log)
        {
            _log = log;
        }

        #region ILogger Members

        public bool IsDebugEnable
        {
            get { return _log.IsDebugEnabled; }
        }

        public void DebugFormat(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }
        public void Debug(object message, Exception exception = null)
        {
            _log.Debug(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }
        public void Info(object message, Exception exception = null)
        {
            _log.Info(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _log.ErrorFormat(format, args);
        }
        public void Error(object message, Exception exception = null)
        {
            _log.Error(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _log.WarnFormat(format, args);
        }
        public void Warn(object message, Exception exception = null)
        {
            _log.Warn(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _log.FatalFormat(format, args);
        }
        public void Fatal(object message, Exception exception = null)
        {
            _log.Fatal(message, exception);
        }

        #endregion
    }
}