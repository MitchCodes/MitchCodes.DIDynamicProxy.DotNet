namespace MitchCodes.DIDynamicProxy.DotNet.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

//todo: https://github.com/Dotnet-Boxed/Templates/blob/main/Docs/NuGet.md
//todo: https://rehansaeed.com/the-fastest-nuget-package-ever-published-probably/
public class DynamicProxiedServiceSettings
{
    /// <summary>
    /// Syncronous Castle.Core interceptor instances that do not use Dependency Injection
    /// </summary>
    public List<IInterceptor> ProxyInterceptorInstances { get; set; } = new List<IInterceptor>();
    /// <summary>
    /// Syncronous Castle.Core interceptors that are registered for Dependency Injection
    /// </summary>
    public List<Type> ProxyInterceptorTypes { get; set; } = new List<Type>();
    /// <summary>
    /// Asyncronous Castle.Core interceptor instances that do not use Dependency Injection
    /// </summary>
    public List<IAsyncInterceptor> ProxyAsyncInterceptorInstances { get; set; } = new List<IAsyncInterceptor>();
    /// <summary>
    /// Asyncronous Castle.Core interceptors that are registered for dependency injection
    /// </summary>
    public List<Type> ProxyAsyncInterceptorTypes { get; set; } = new List<Type>();

    public DynamicProxiedServiceSettings()
    {
    }

    public DynamicProxiedServiceSettings(List<IInterceptor> proxyInterceptorInstances, List<Type> proxyInterceptorTypes, List<IAsyncInterceptor> proxyAsyncInterceptorInstances, List<Type> proxyAsyncInterceptorTypes)
    {
        this.ProxyInterceptorInstances = proxyInterceptorInstances;
        this.ProxyInterceptorTypes = proxyInterceptorTypes;
        this.ProxyAsyncInterceptorInstances = proxyAsyncInterceptorInstances;
        this.ProxyAsyncInterceptorTypes = proxyAsyncInterceptorTypes;
    }

    public DynamicProxiedServiceSettings(List<IInterceptor> proxyInterceptorInstances, List<Type> proxyInterceptorTypes)
    {
        this.ProxyInterceptorInstances = proxyInterceptorInstances;
        this.ProxyInterceptorTypes = proxyInterceptorTypes;
    }

    public DynamicProxiedServiceSettings(List<IAsyncInterceptor> proxyAsyncInterceptorInstances, List<Type> proxyAsyncInterceptorTypes)
    {
        this.ProxyAsyncInterceptorInstances = proxyAsyncInterceptorInstances;
        this.ProxyAsyncInterceptorTypes = proxyAsyncInterceptorTypes;
    }

    public DynamicProxiedServiceSettings(List<IInterceptor> proxyInterceptorInstances) => this.ProxyInterceptorInstances = proxyInterceptorInstances;

    public DynamicProxiedServiceSettings(List<IAsyncInterceptor> proxyAsyncInterceptorInstances) => this.ProxyAsyncInterceptorInstances = proxyAsyncInterceptorInstances;

    public DynamicProxiedServiceSettings(List<Type> proxyInterceptorTypes, List<Type> proxyAsyncInterceptorTypes)
    {
        this.ProxyInterceptorTypes = proxyInterceptorTypes;
        this.ProxyAsyncInterceptorTypes = proxyAsyncInterceptorTypes;
    }

    public DynamicProxiedServiceSettings AddProxyInterceptorInstance(IInterceptor instance)
    {
        this.ProxyInterceptorInstances.Add(instance);

        return this;
    }

    public DynamicProxiedServiceSettings AddProxyInterceptor<T>() where T : IInterceptor
    {
        this.ProxyInterceptorTypes.Add(typeof(T));

        return this;
    }

    public DynamicProxiedServiceSettings AddProxyInterceptor(Type proxyInterceptorType)
    {
        if (!proxyInterceptorType.IsAssignableTo(typeof(IInterceptor)))
        {
            throw new Exception($"{nameof(proxyInterceptorType)} is not assignable to IInterceptor");
        }

        this.ProxyInterceptorTypes.Add(proxyInterceptorType);

        return this;
    }

    public DynamicProxiedServiceSettings AddAsyncProxyInterceptorInstance(IAsyncInterceptor instance)
    {
        this.ProxyAsyncInterceptorInstances.Add(instance);

        return this;
    }

    public DynamicProxiedServiceSettings AddAsyncProxyInterceptor<T>() where T : IAsyncInterceptor
    {
        this.ProxyAsyncInterceptorTypes.Add(typeof(T));

        return this;
    }

    public DynamicProxiedServiceSettings AddAsyncProxyInterceptor(Type asyncProxyInterceptorType)
    {
        if (!asyncProxyInterceptorType.IsAssignableTo(typeof(IAsyncInterceptor)))
        {
            throw new Exception($"{nameof(asyncProxyInterceptorType)} is not assignable to IAsyncInterceptor");
        }

        this.ProxyAsyncInterceptorTypes.Add(asyncProxyInterceptorType);

        return this;
    }
}
