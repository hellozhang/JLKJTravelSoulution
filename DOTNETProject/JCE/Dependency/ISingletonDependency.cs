﻿namespace JCE.Dependency
{
    /// <summary>
    /// 依赖注入接口，实现该接口将自动注册到IOC容器，生命周期为单例
    /// </summary>
    public interface ISingletonDependency:IDependency
    {
    }
}
