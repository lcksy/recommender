using System;
using System.Configuration;
using System.Xml;

namespace CQSS.Common.Infrastructure.Configuration
{
    public class CQSSConfig : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new CQSSConfig();

            config.EngineType = section.GetNode("Engine").GetAttributeValue("Type");
            config.ObjectContainerType = section.GetNode("ObjectContainer").GetAttributeValue("Type");
            config.AssemblySkipPattern = section.GetNode("AssemblySkipPattern").GetAttributeValue("Value");
            config.AssemblyRestrictPattern = section.GetNode("AssemblyRestrictPattern").GetAttributeValue("Value");
            config.IsWebApplication = Convert.ToBoolean(section.GetNode("IsWebApplication").GetAttributeValue("Value"));

            return config;
        }

        /// <summary>
        /// 用户自定义 <see cref="IEngine"/> 的类型字符串
        /// </summary>
        public string EngineType { get; private set; }

        /// <summary>
        /// 用户自定义 <see cref="IObjectContainer"/> 的类型字符串
        /// </summary>
        public string ObjectContainerType { get; private set; }

        /// <summary>
        /// 正则表达式，如果程序集的全名满足此正则表达式，则框架将不在此程序集中搜索 IDependencyRegistrar 的具体实现类
        /// </summary>
        public string AssemblySkipPattern { get; private set; }

        /// <summary>
        /// 正则表达式，如果程序集的全名不满足此正则表达式，则框架将不在此程序集中搜索 IDependencyRegistrar 的具体实现类
        /// </summary>
        public string AssemblyRestrictPattern { get; private set; }

        public bool IsWebApplication { get; private set; }

        public static CQSSConfig Default { get; private set; }

        static CQSSConfig()
        {
            Default = new CQSSConfig();
            Default.EngineType = "CQSS.Common.Infrastructure.Engine.DefaultEngine, CQSS.Common";
            Default.ObjectContainerType = "CQSS.Common.ObjectContainer.Autofac.AutofacObjectContainer, CQSS.Common.ObjectContainer.Autofac";
            Default.AssemblySkipPattern = "^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";
            Default.AssemblyRestrictPattern = "CQSS.*";
            Default.IsWebApplication = true;
        }
    }

    internal static class Extension
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
    }
}