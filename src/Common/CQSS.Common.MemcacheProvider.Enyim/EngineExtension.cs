using CQSS.Common.Infrastructure.Engine;
using CQSS.Common.Infrastructure.MemcacheProvider;
using CQSS.Common.Infrastructure.Serializing;

namespace CQSS.Common.MemcacheProvider.Enyim
{
    public static class EngineExtension
    {
        /// <summary>
        /// 使用 Enyim.Caching 作为 Memcached 客户端管理工具
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="sieralizer"></param>
        /// <returns></returns>
        public static IEngine UseEnyimMemcacheProvider(this IEngine engine, IJsonSerializer sieralizer = null)
        {
            engine.Container.RegisterInstance<IMemcacheProvider, EnyimMemcacheProvider>(new EnyimMemcacheProvider(sieralizer));
            return engine;
        }
    }
}