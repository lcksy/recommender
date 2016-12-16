using CQSS.Common.Infrastructure.Engine;
using CQSS.Common.Infrastructure.Serializing;

namespace CQSS.Common.Serializing.JsonDotNet
{
    public static class EngineExtension
    {
        public static IEngine UseJsonDotNetSerializer(this IEngine engine)
        {
            engine.Container.RegisterInstance<IJsonSerializer, JsonDotNetSerializer>(new JsonDotNetSerializer());
            return engine;
        }
    }
}