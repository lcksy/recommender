using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using CQSS.Common.Infrastructure.Configuration;
using CQSS.Common.Infrastructure.ObjectContainer;

namespace CQSS.Common.Infrastructure.Engine
{
    public class EngineContext
    {
        /// <summary>
        /// 获取全局唯一的 IEngine 实例
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                    Initialize(false);
                return Singleton<IEngine>.Instance;
            }
        }

        /// <summary>
        /// 初始化 EngineContext.Current 实例
        /// </summary>
        /// <param name="forceRecreate">是否重建</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                var config = ConfigurationManager.GetSection("CQSSConfig") as CQSSConfig;
                config = config ?? CQSSConfig.Default;
                Singleton<IEngine>.Instance = CreateEngineInstance(config);
                Singleton<IEngine>.Instance.Initialize(config);
            }
            return Singleton<IEngine>.Instance;
        }

        /// <summary>
        /// 实例化 IEngine 实例
        /// </summary>
        protected static IEngine CreateEngineInstance(CQSSConfig config)
        {
            if (config == null)
                throw new ConfigurationErrorsException("配置文件中未定义 CQSSConfig");

            if (string.IsNullOrEmpty(config.EngineType))
                throw new ConfigurationErrorsException("配置文件未定义 CQSSConfig.EngineType");

            var engineType = Type.GetType(config.EngineType);
            if (engineType == null)
                throw new ConfigurationErrorsException("未找到 CQSSConfig.EngineType 所定义的类型");

            if (string.IsNullOrEmpty(config.ObjectContainerType))
                throw new ConfigurationErrorsException("配置文件未定义 CQSSConfig.ObjectContainerType");

            var objectContainerType = Type.GetType(config.ObjectContainerType);
            if (objectContainerType == null)
                throw new ConfigurationErrorsException("未找到 CQSSConfig.ObjectContainerType 所定义的类型");

            var objectContainer = Activator.CreateInstance(objectContainerType) as IObjectContainer;
            var engine = Activator.CreateInstance(engineType, objectContainer) as IEngine;

            return engine;
        }

        /// <summary>
        /// 替换 IEngine 实例
        /// </summary>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Replace(engine);
        }
    }
}