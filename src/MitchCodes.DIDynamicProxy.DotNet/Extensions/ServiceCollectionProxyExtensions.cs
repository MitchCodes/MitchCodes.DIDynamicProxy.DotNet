namespace MitchCodes.DIDynamicProxy.DotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MitchCodes.DIDynamicProxy.DotNet.Settings;

public static class ServiceCollectionProxyExtensions
{
    public static IServiceCollection AddServiceLifetime<IInterface, IImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
        where IInterface : class
        where IImplementation : class, IInterface
    {
        if (lifetime == ServiceLifetime.Scoped)
        {
            services.AddScoped<IInterface, IImplementation>();
        }
        else if (lifetime == ServiceLifetime.Transient)
        {
            services.AddTransient<IInterface, IImplementation>();
        }
        else
        {
            services.AddSingleton<IInterface, IImplementation>();
        }

        return services;
    }

    public static IServiceCollection AddServiceLifetime<IImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
        where IImplementation : class
    {
        if (lifetime == ServiceLifetime.Scoped)
        {
            services.AddScoped<IImplementation>();
        } else if (lifetime == ServiceLifetime.Transient)
        {
            services.AddTransient<IImplementation>();
        } else
        {
            services.AddSingleton<IImplementation>();
        }

        return services;
    }

    public static IServiceCollection TryAddProxyGenerator(this IServiceCollection services, IProxyBuilder? builder = null)
    {
        // try to register the ProxyGenerator as a singleton
        services.TryAddSingleton<ProxyGenerator>(builder == null ? new ProxyGenerator() : new ProxyGenerator(builder));

        return services;
    }

    public static IServiceCollection AddInterceptor<T>(this IServiceCollection services) where T: class, IInterceptor
    {
        services.AddTransient<IInterceptor, T>();
        services.AddTransient<GlobalInterceptorWrapper>();

        return services;
    }

    public static IServiceCollection AddInterceptor(this IServiceCollection services, IInterceptor instance)
    {
        services.AddSingleton<IInterceptor>(instance);
        services.AddSingleton<GlobalInterceptorWrapper>(new GlobalInterceptorWrapper(instance));

        return services;
    }

    public static IServiceCollection AddAsyncInterceptor<T>(this IServiceCollection services) where T : class, IAsyncInterceptor
    {
        services.AddTransient<IAsyncInterceptor, T>();
        services.AddTransient<GlobalAsyncInterceptorWrapper>();

        return services;
    }

    public static IServiceCollection AddAsyncInterceptor(this IServiceCollection services, IAsyncInterceptor instance)
    {
        services.AddSingleton<IAsyncInterceptor>(instance);
        services.AddSingleton<GlobalAsyncInterceptorWrapper>(new GlobalAsyncInterceptorWrapper(instance));

        return services;
    }

    public static IServiceCollection AddDyanamicProxyScoped<IInterface, IImplementation>(this IServiceCollection services, DynamicProxiedServiceSettings? dynamicProxySettings = null)
        where IInterface : class
        where IImplementation : class, IInterface
    {
        services.AddDynamicProxyService<IInterface, IImplementation>(ServiceLifetime.Scoped, dynamicProxySettings);

        return services;
    }

    public static IServiceCollection AddDynamicProxyTransient<IInterface, IImplementation>(this IServiceCollection services, DynamicProxiedServiceSettings? dynamicProxySettings = null)
        where IInterface : class
        where IImplementation : class, IInterface
    {
        services.AddDynamicProxyService<IInterface, IImplementation>(ServiceLifetime.Transient, dynamicProxySettings);

        return services;
    }

    public static IServiceCollection AddDynamicProxySingleton<IInterface, IImplementation>(this IServiceCollection services, DynamicProxiedServiceSettings? dynamicProxySettings = null)
        where IInterface : class
        where IImplementation : class, IInterface
    {
        services.AddDynamicProxyService<IInterface, IImplementation>(ServiceLifetime.Singleton, dynamicProxySettings);

        return services;
    }

    public static IServiceCollection AddDynamicProxySingleton<IInterface>(this IServiceCollection services, IInterface instance, DynamicProxiedServiceSettings? dynamicProxySettings = null)
        where IInterface : class
    {
        services.AddDynamicProxyService<IInterface>(instance, dynamicProxySettings);

        return services;
    }

    public static IServiceCollection AddDynamicProxyService<IInterface, IImplementation>(this IServiceCollection services, ServiceLifetime lifetime, DynamicProxiedServiceSettings? dynamicProxySettings = null)
        where IInterface : class
        where IImplementation : class, IInterface
    {
        var implementationType = typeof(IImplementation) ?? throw new Exception("Dynamic proxy service resolution implementation type is not set");

        // get settings
        dynamicProxySettings ??= new DynamicProxiedServiceSettings();
        
        // register the implementation using the requested lifetime
        services.AddServiceLifetime<IImplementation>(lifetime);

        services.RegisterDynamicProxyService<IInterface>(implementationType, lifetime, dynamicProxySettings);

        return services;
    }

    public static IServiceCollection AddDynamicProxyService<IInterface>(this IServiceCollection services, IInterface instance, DynamicProxiedServiceSettings? dynamicProxySettings = null)
        where IInterface : class
    {
        var implementationType = instance.GetType() ?? throw new Exception("Dynamic proxy service resolution implementation type is not set");

        // get settings
        dynamicProxySettings ??= new DynamicProxiedServiceSettings();

        // register the implementation as a singleton
        services.AddSingleton(instance);

        services.RegisterDynamicProxyService<IInterface>(implementationType, ServiceLifetime.Singleton, dynamicProxySettings);

        return services;
    }

    private static IServiceCollection RegisterDynamicProxyService<IInterface>(this IServiceCollection services, Type implementationType, ServiceLifetime lifetime, DynamicProxiedServiceSettings dynamicProxySettings)
        where IInterface: class
    {
        // try to add the proxy generator in-case it isn't registered yet
        services.TryAddProxyGenerator();

        // register interceptors specific to this dynamic proxy in DI

        RegisterDynamicProxySpecificTransientInterceptors(services, dynamicProxySettings);

        services.Add(new ServiceDescriptor(typeof(IInterface), provider =>
        {
            // get the actual implementation of IInterface to proxy. if implementation is null or not registered, activate a new instance of it
            var implementationObj = provider.GetRequiredService(implementationType) ?? Activator.CreateInstance(implementationType);
            var implementation = (IInterface)implementationObj;

            // Use the resolved implementation as the initial proxy 
            var proxy = implementation;

            // get the proxy generator from DI
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();


            // Handle syncronous interceptors

            // get all interceptors from DI
            var allInterceptors = provider.GetServices<IInterceptor>().ToList() ?? [];
            var interceptors = new List<IInterceptor>();

            // load all global interceptors from DI
            provider.GetServices<GlobalInterceptorWrapper>().ToList().ForEach(wrapper => interceptors.Add(wrapper.Interceptor));

            // load all interceptor instances from settings directly into interceptors list
            interceptors.AddRange(dynamicProxySettings.ProxyInterceptorInstances);

            // load all specific keyed interceptors from DI
            if (dynamicProxySettings.ProxyInterceptorTypes.Count > 0)
            {
                foreach (var interceptorType in dynamicProxySettings.ProxyInterceptorTypes)
                {
                    var interceptorResolved = provider.GetService(interceptorType);
                    if (interceptorResolved != null && interceptorResolved.GetType().IsAssignableTo(typeof(IInterceptor)))
                    {
                        interceptors.Add((IInterceptor)interceptorResolved);
                    }
                }
            }


            // Handle asyncronous interceptors

            // get all async interceptors from DI
            var allAsyncInterceptors = provider.GetServices<IAsyncInterceptor>().ToList() ?? [];
            var asyncInterceptors = new List<IAsyncInterceptor>();

            // load all global async interceptors from DI
            provider.GetServices<GlobalAsyncInterceptorWrapper>().ToList().ForEach(wrapper => asyncInterceptors.Add(wrapper.AsyncInterceptor));

            // load all async interceptor instances from settings directly into async interceptors list
            asyncInterceptors.AddRange(dynamicProxySettings.ProxyAsyncInterceptorInstances);

            // load all specific keyed async interceptors from DI
            if (dynamicProxySettings.ProxyAsyncInterceptorTypes.Count > 0)
            {
                foreach (var asyncInterceptorType in dynamicProxySettings.ProxyAsyncInterceptorTypes)
                {
                    var asyncInterceptorResolved = provider.GetService(asyncInterceptorType);
                    if (asyncInterceptorResolved != null && asyncInterceptorResolved.GetType().IsAssignableTo(typeof(IAsyncInterceptor)))
                    {
                        asyncInterceptors.Add((IAsyncInterceptor)asyncInterceptorResolved);
                    }
                }
            }


            // create a proxy using syncronous interceptors if we have any
            if (interceptors.Count > 0 && proxy != null)
            {
                proxy = proxyGenerator.CreateInterfaceProxyWithTarget<IInterface>(proxy, interceptors.ToArray());
            }

            // create an async proxy using async interceptors if we have any, composing it from a potential sync proxy
            if (asyncInterceptors.Count > 0 && proxy != null)
            {
                proxy = proxyGenerator.CreateInterfaceProxyWithTarget<IInterface>(proxy, asyncInterceptors.ToArray());
            }

            return proxy ?? throw new Exception("Dynamic proxy service resolution resulted in a null implementation");
        }, lifetime));

        return services;
    }

    private static void RegisterDynamicProxySpecificTransientInterceptors(IServiceCollection services, DynamicProxiedServiceSettings? dynamicProxySettings)
    {
        if (dynamicProxySettings != null)
        {
            foreach (var interceptorType in dynamicProxySettings.ProxyInterceptorTypes ?? [])
            {
                services.AddTransient(typeof(IInterceptor), interceptorType);
                services.AddTransient(interceptorType);
            }

            foreach (var asyncInterceptorType in dynamicProxySettings.ProxyAsyncInterceptorTypes ?? [])
            {
                services.AddTransient(typeof(IAsyncInterceptor), asyncInterceptorType);
                services.AddTransient(asyncInterceptorType);
            }
        }
    }
}
