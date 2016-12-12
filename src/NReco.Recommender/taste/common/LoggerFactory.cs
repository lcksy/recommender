using System;

namespace NReco.CF.Taste.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Logger GetLogger(Type type)
        {
            return new Logger(type);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Logger
    {
        Type LogType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public Logger(Type type)
        {
            LogType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Info(string format, params object[] args)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Warn(string format, params object[] args)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Debug(string format, params object[] args)
        {

        }
    }
}