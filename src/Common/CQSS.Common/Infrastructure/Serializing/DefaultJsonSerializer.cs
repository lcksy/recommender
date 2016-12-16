using System;
using System.Web.Script.Serialization;

namespace CQSS.Common.Infrastructure.Serializing
{
    public class DefaultJsonSerializer : IJsonSerializer
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public string Serialize(object obj)
        {
            return _serializer.Serialize(obj);
        }

        public object Deserialize(string json, Type type)
        {
            return _serializer.Deserialize(json, type);
        }

        public T Deserialize<T>(string json)
        {
            return _serializer.Deserialize<T>(json);
        }
    }
}