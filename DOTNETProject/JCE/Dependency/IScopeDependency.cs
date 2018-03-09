﻿namespace JCE.Dependency
{
    /// <summary>
    /// 依赖注入接口，实现该接口将自动注册到IOC容器，生命周期为每次请求创建一个实例
    /// </summary>
    public interface IScopeDependency:IDependency
    {
    }
}
