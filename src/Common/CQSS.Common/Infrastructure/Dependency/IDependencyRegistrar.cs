using CQSS.Common.Infrastructure.ObjectContainer;
using System.Collections.Generic;
using System.Reflection;

namespace CQSS.Common.Infrastructure.Dependency
{
    public interface IDependencyRegistrar
    {
        /// <summary>
        /// 自定义注册代码
        /// </summary>
        void Register(IObjectContainer container, IEnumerable<Assembly> assemblies);

        /// <summary>
        /// 注册顺序
        /// </summary>
        int Order { get; }
    }
}