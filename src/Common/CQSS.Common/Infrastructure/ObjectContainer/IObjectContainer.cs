using System;

namespace CQSS.Common.Infrastructure.ObjectContainer
{
    public interface IObjectContainer
    {
        #region Register

        void RegisterType(Type serviceType, LifeStyle life = LifeStyle.Singleton);

        void RegisterType(Type serviveType, Type implType, string registerName = "", LifeStyle life = LifeStyle.Singleton);

        void Register<TService, TImpl>(string registerName = "", LifeStyle life = LifeStyle.Singleton)
            where TService : class
            where TImpl : class, TService;

        void RegisterInstance<TService, TImpl>(TImpl instance, string registerName = "")
            where TService : class
            where TImpl : class, TService;

        void RegisterInstance(Type serviceType, object instance, string registerName = "");

        #endregion

        #region Resolve

        TService Resolve<TService>(string registerName = "") where TService : class;

        object Resolve(Type serviceType, string registerName = "");

        #endregion
    }

    public enum LifeStyle
    {
        /// <summary>
        /// 瞬时
        /// </summary>
        Transient,
        /// <summary>
        /// 单例
        /// </summary>
        Singleton
    }
}