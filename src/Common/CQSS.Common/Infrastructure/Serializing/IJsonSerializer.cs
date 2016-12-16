using System;

namespace CQSS.Common.Infrastructure.Serializing
{
    public interface IJsonSerializer
    {
        string Serialize(object obj);

        object Deserialize(string json, Type type);

        T Deserialize<T>(string json);
    }
}