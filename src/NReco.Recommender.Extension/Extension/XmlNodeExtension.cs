using System;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace NReco.Recommender.Extension
{
    internal static class XmlNodeExtension
    {
        internal static XmlNode GetNode(this XmlNode parentNode, string nodeName)
        {
            return parentNode.SelectSingleNode(nodeName);
        }

        internal static string GetAttributeValue(this XmlNode node, string attrName)
        {
            if (node == null || node.Attributes == null || node.Attributes.Count == 0)
                return string.Empty;

            var attr = node.Attributes[attrName];
            return attr != null ? attr.Value : string.Empty;
        }

        internal static T Map<T>(this XmlNode node)
        {
            var instance = Activator.CreateInstance<T>();

            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

            foreach (var prop in properties)
            {
                var connectionString = node.GetAttributeValue(prop.Name);

                prop.SetValue(instance, connectionString);
            }

            return instance;
        }
    }
}