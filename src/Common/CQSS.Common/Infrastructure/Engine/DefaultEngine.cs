using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CQSS.Common.Infrastructure.Configuration;
using CQSS.Common.Infrastructure.Database;
using CQSS.Common.Infrastructure.Dependency;
using CQSS.Common.Infrastructure.Logging;
using CQSS.Common.Infrastructure.ObjectContainer;
using CQSS.Common.Infrastructure.Serializing;
using CQSS.Common.Util;

namespace CQSS.Common.Infrastructure.Engine
{
    public class DefaultEngine : IEngine
    {
        #region Ctor

        public DefaultEngine(IObjectContainer container)
        {
            Container = container;
        }

        #endregion

        #region IEngine implements

        public IObjectContainer Container { get; protected set; }

        public TService Resolve<TService>(string registerName = "")
            where TService : class
        {
            return Container.Resolve<TService>(registerName);
        }

        public object Resolve(Type serviceType, string registerName = "")
        {
            return Container.Resolve(serviceType, registerName);
        }

        public void Initialize(CQSSConfig config)
        {
            RegisterCommonComponents(config);
            RegisterDependencies(config);
            RunStartupTasks(config);
        }

        #endregion

        #region Method

        /// <summary>
        /// 注册公共组件
        /// </summary>
        protected virtual void RegisterCommonComponents(CQSSConfig config)
        {
            Container.RegisterInstance<IEngine, DefaultEngine>(this);
            Container.RegisterInstance<CQSSConfig, CQSSConfig>(config);
            Container.RegisterInstance<ILoggerFactory, EmptyLoggerFactory>(new EmptyLoggerFactory());
            Container.RegisterInstance<IJsonSerializer, DefaultJsonSerializer>(new DefaultJsonSerializer());
            Container.RegisterInstance<IDbConnectionFactory, DefaultDbConnectionFactory>(new DefaultDbConnectionFactory());
        }

        /// <summary>
        /// 注册自定义组件
        /// </summary>
        protected virtual void RegisterDependencies(CQSSConfig config)
        {
            var assemblies = AssemblyLocator.GetAssemblies(config.IsWebApplication)
                                            .Where(t => t.IsNotMatch(config.AssemblySkipPattern) && t.IsMatch(config.AssemblyRestrictPattern));

            var types = assemblies.SelectMany(t => t.GetTypes<IDependencyRegistrar>())
                                  .Where(t => t.FullName != "ASP.global_asax")
                                  .Distinct();

            var registrars = types.Select(t => Activator.CreateInstance(t) as IDependencyRegistrar)
                                  .OrderBy(r => r.Order)
                                  .ToList();

            registrars.ForEach(r => r.Register(this.Container, assemblies));
        }

        /// <summary>
        /// 运行启动任务
        /// </summary>
        protected virtual void RunStartupTasks(CQSSConfig config)
        {

        }

        #endregion
    }

    internal static class InternalExtension
    {
        /// <summary>
        /// 根据配置文件的配置，过滤当前应用程序域的所有程序集，得到满足条件的程序集列表
        /// </summary>
        /// <param name="domain">当前应用程序域</param>
        /// <param name="config">配置文件</param>
        /// <returns>满足条件的程序集列表</returns>
        public static IEnumerable<Assembly> GetAssemblies(this AppDomain domain, CQSSConfig config)
        {
            return domain.GetAssemblies()
                         .Where(t => t.IsNotMatch(config.AssemblySkipPattern) && t.IsMatch(config.AssemblyRestrictPattern));
        }

        /// <summary>
        /// 判断程序集的全名是否匹配正则表达式，如果正则表达式为空返回 true
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns>是否匹配</returns>
        public static bool IsMatch(this Assembly assembly, string pattern)
        {
            bool match = true;
            if (!string.IsNullOrEmpty(pattern))
                match = Regex.IsMatch(assembly.FullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return match;
        }

        /// <summary>
        /// 判断程序集的全名是否不匹配正则表达式
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns>是否匹配</returns>
        public static bool IsNotMatch(this Assembly assembly, string pattern)
        {
            return !IsMatch(assembly, pattern);
        }

        /// <summary>
        /// 从程序集中获取所有继承于 TService 的子类型列表
        /// </summary>
        /// <typeparam name="TService">父类型</typeparam>
        /// <param name="assembly">程序集</param>
        /// <param name="ignoreReflectionErrors">是否忽略异常</param>
        /// <param name="onlyConcertClass">是否只获取非虚类</param>
        /// <returns>子类型列表</returns>
        public static IEnumerable<Type> GetTypes<TService>(this Assembly assembly, bool ignoreReflectionErrors = true, bool onlyConcertClass = true)
        {
            try
            {
                var types = assembly.GetTypes().Where(t => typeof(TService).IsAssignableFrom(t));
                if (onlyConcertClass)
                    types = types.Where(t => t.IsClass && !t.IsAbstract);

                return types;
            }
            catch
            {
                if (!ignoreReflectionErrors)
                    throw;
                else
                    return Enumerable.Empty<Type>();
            }
        }
    }
}