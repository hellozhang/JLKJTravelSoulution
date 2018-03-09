﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using JCE.Contexts;
using JCE.Events.Handlers;
using JCE.Helpers;
using JCE.Reflections;
using JCE.Utils.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace JCE.Dependency
{
    /// <summary>
    /// 依赖配置
    /// </summary>
    public class DependencyConfiguration
    {
        /// <summary>
        /// 服务集合
        /// </summary>
        private readonly IServiceCollection _services;

        /// <summary>
        /// 依赖配置
        /// </summary>
        private readonly IConfig[] _configs;

        /// <summary>
        /// 容器生成器
        /// </summary>
        private ContainerBuilder _builder;

        /// <summary>
        /// 类型查找器
        /// </summary>
        private ITypeFinder _finder;

        /// <summary>
        /// 程序集列表
        /// </summary>
        private List<Assembly> _assemblies;

        /// <summary>
        /// 初始化一个<see cref="DependencyConfiguration"/>类型的实例
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configs">依赖配置</param>
        public DependencyConfiguration(IServiceCollection services, IConfig[] configs)
        {
            _services = services;
            _configs = configs;
        }

        /// <summary>
        /// 配置依赖
        /// </summary>
        /// <returns></returns>
        public IServiceProvider Config()
        {
            return Ioc.DefaultContainer.Register(_services, RegistServices, _configs);
        }

        /// <summary>
        /// 注册服务集合
        /// </summary>
        /// <param name="builder">容器生成器</param>
        private void RegistServices(ContainerBuilder builder)
        {
            _builder = builder;
            _finder=new WebAppTypeFinder();
            _assemblies = _finder.GetAssemblies();
            RegistInfrastracture();
            RegistEventHandlers();
            RegistDependency();
        }

        #region 基础设施注册

        /// <summary>
        /// 注册基础设施
        /// </summary>
        private void RegistInfrastracture()
        {
            EnableAop();
            RegistFinder();
            RegistContext();
        }

        /// <summary>
        /// 启用Aop
        /// </summary>
        private void EnableAop()
        {
            _builder.EnableAop();
        }

        /// <summary>
        /// 注册类型查找器
        /// </summary>
        private void RegistFinder()
        {
            _builder.AddSingleton(_finder);
        }

        /// <summary>
        /// 注册上下文
        /// </summary>
        private void RegistContext()
        {
            _builder.AddSingleton<IContext, WebContext>();
            _builder.AddScoped<IUserContext, NullUserContext>();
        }

        /// <summary>
        /// 注册Http上下文访问器
        /// </summary>
        private void ReigstHttpContextAccessor()
        {
            Web.SetHttpContextAccessor(Ioc.Create<IHttpContextAccessor>());
        }

        #endregion

        #region 事件处理器注册

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        private void RegistEventHandlers()
        {
            var handlerTypes = GetTypes(typeof(IEventHandler<>));
            foreach (var handler in handlerTypes)
            {
                _builder.RegisterType(handler)
                    .As(handler.FindInterfaces(
                        (filter, criteria) => filter.IsGenericType &&
                                              ((Type) criteria).IsAssignableFrom(filter.GetGenericTypeDefinition()),
                        typeof(IEventHandler<>)
                    )).InstancePerLifetimeScope();
            }
        }

        #endregion

        #region 依赖自动注册

        /// <summary>
        /// 查找并注册依赖
        /// </summary>
        private void RegistDependency()
        {
            RegistSingletonDependency();
            RegistScopeDependency();
            RegistTransientDependency();
            ResolveDependencyRegistrar();
        }

        /// <summary>
        /// 注册单例依赖
        /// </summary>
        private void RegistSingletonDependency()
        {
            _builder.RegisterTypes(GetTypes<ISingletonDependency>()).AsImplementedInterfaces().PropertiesAutowired().SingleInstance();
        }

        /// <summary>
        /// 注册作用域依赖
        /// </summary>
        private void RegistScopeDependency()
        {
            _builder.RegisterTypes(GetTypes<IScopeDependency>()).AsImplementedInterfaces().PropertiesAutowired().InstancePerLifetimeScope();
        }

        /// <summary>
        /// 注册瞬态依赖
        /// </summary>
        private void RegistTransientDependency()
        {
            _builder.RegisterTypes(GetTypes<ITransientDependency>()).AsImplementedInterfaces().PropertiesAutowired().InstancePerDependency();
        }

        /// <summary>
        /// 解析依赖注册器
        /// </summary>
        private void ResolveDependencyRegistrar()
        {
            var types = GetTypes<IDependencyRegistrar>();
            types.Select(type => Reflection.CreateInstance<IDependencyRegistrar>(type)).ToList()
                .ForEach(t => t.Register(_services));
        }

        #endregion

        /// <summary>
        /// 获取类型集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        private Type[] GetTypes<T>()
        {
            return _finder.Find<T>(_assemblies).ToArray();
        }

        /// <summary>
        /// 获取类型集合
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        private Type[] GetTypes(Type type)
        {
            return _finder.Find(type, _assemblies).ToArray();
        }

    }
}
