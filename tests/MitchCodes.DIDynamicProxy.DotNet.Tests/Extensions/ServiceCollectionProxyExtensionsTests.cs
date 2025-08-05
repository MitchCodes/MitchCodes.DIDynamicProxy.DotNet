namespace MitchCodes.DIDynamicProxy.DotNet.Tests.Extensions;

using Castle.DynamicProxy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MitchCodes.DIDynamicProxy.DotNet.Extensions;
using MitchCodes.DIDynamicProxy.DotNet.Settings;
using MitchCodes.DIDynamicProxy.DotNet.Tests.TestHelpers;
using Xunit;

public class ServiceCollectionProxyExtensionsTests
{
    [Fact]
    public void AddInterceptor_Generic_ShouldRegisterInterceptorAndWrapper()
    {
        var services = new ServiceCollection();

        services.AddInterceptor<TestInterceptor>();

        var serviceProvider = services.BuildServiceProvider();
        var interceptors = serviceProvider.GetServices<IInterceptor>();
        var wrappers = serviceProvider.GetServices<GlobalInterceptorWrapper>();

        interceptors.Should().HaveCount(1);
        interceptors.First().Should().BeOfType<TestInterceptor>();
        wrappers.Should().HaveCount(1);
    }

    [Fact]
    public void AddInterceptor_Instance_ShouldRegisterInterceptorAndWrapper()
    {
        var services = new ServiceCollection();
        var interceptor = new TestInterceptor();

        services.AddInterceptor(interceptor);

        var serviceProvider = services.BuildServiceProvider();
        var registeredInterceptor = serviceProvider.GetService<IInterceptor>();
        var wrapper = serviceProvider.GetService<GlobalInterceptorWrapper>();

        registeredInterceptor.Should().BeSameAs(interceptor);
        wrapper.Should().NotBeNull();
        wrapper!.Interceptor.Should().BeSameAs(interceptor);
    }

    [Fact]
    public void AddAsyncInterceptor_Generic_ShouldRegisterInterceptorAndWrapper()
    {
        var services = new ServiceCollection();

        services.AddAsyncInterceptor<TestAsyncInterceptor>();

        var serviceProvider = services.BuildServiceProvider();
        var interceptors = serviceProvider.GetServices<IAsyncInterceptor>();
        var wrappers = serviceProvider.GetServices<GlobalAsyncInterceptorWrapper>();

        interceptors.Should().HaveCount(1);
        interceptors.First().Should().BeOfType<TestAsyncInterceptor>();
        wrappers.Should().HaveCount(1);
    }

    [Fact]
    public void AddAsyncInterceptor_Instance_ShouldRegisterInterceptorAndWrapper()
    {
        var services = new ServiceCollection();
        var interceptor = new TestAsyncInterceptor();

        services.AddAsyncInterceptor(interceptor);

        var serviceProvider = services.BuildServiceProvider();
        var registeredInterceptor = serviceProvider.GetService<IAsyncInterceptor>();
        var wrapper = serviceProvider.GetService<GlobalAsyncInterceptorWrapper>();

        registeredInterceptor.Should().BeSameAs(interceptor);
        wrapper.Should().NotBeNull();
        wrapper!.AsyncInterceptor.Should().BeSameAs(interceptor);
    }

    [Fact]
    public void AddDynamicProxyScoped_ShouldRegisterProxiedService()
    {
        var services = new ServiceCollection();
        services.AddInterceptor<TestInterceptor>();
        services.AddDyanamicProxyScoped<ITestService, TestService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestService>();

        service.Should().NotBeNull();
        service.Should().NotBeOfType<TestService>();
        service!.GetMessage().Should().Be("Hello World");
    }

    [Fact]
    public void AddDynamicProxyTransient_ShouldRegisterProxiedService()
    {
        var services = new ServiceCollection();

        services.AddDynamicProxyTransient<ITestService, TestService>();

        var serviceProvider = services.BuildServiceProvider();
        var service1 = serviceProvider.GetService<ITestService>();
        var service2 = serviceProvider.GetService<ITestService>();

        service1.Should().NotBeNull();
        service2.Should().NotBeNull();
        service1.Should().NotBeSameAs(service2);
    }

    [Fact]
    public void AddDynamicProxySingleton_ShouldRegisterProxiedService()
    {
        var services = new ServiceCollection();

        services.AddDynamicProxySingleton<ITestService, TestService>();

        var serviceProvider = services.BuildServiceProvider();
        var service1 = serviceProvider.GetService<ITestService>();
        var service2 = serviceProvider.GetService<ITestService>();

        service1.Should().NotBeNull();
        service2.Should().NotBeNull();
        service1.Should().BeSameAs(service2);
    }

    [Fact]
    public void AddDynamicProxySingleton_WithInstance_ShouldRegisterProxiedService()
    {
        var services = new ServiceCollection();
        var instance = new TestService();
        services.AddInterceptor<TestInterceptor>();

        services.AddDynamicProxySingleton<ITestService>(instance);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestService>();

        service.Should().NotBeNull();
        service!.GetMessage().Should().Be("Hello World");
    }

    [Fact]
    public void DynamicProxy_WithGlobalInterceptor_ShouldInterceptCalls()
    {
        var services = new ServiceCollection();
        var interceptor = new TestInterceptor();
        services.AddInterceptor(interceptor);
        services.AddDyanamicProxyScoped<ITestService, TestService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestService>();

        service!.GetMessage();

        interceptor.WasCalled.Should().BeTrue();
        interceptor.LastMethodName.Should().Be("GetMessage");
    }

    [Fact]
    public async Task DynamicProxy_WithGlobalAsyncInterceptor_ShouldInterceptAsyncCalls()
    {
        var services = new ServiceCollection();
        var interceptor = new TestAsyncInterceptor();
        services.AddAsyncInterceptor(interceptor);
        services.AddDyanamicProxyScoped<ITestService, TestService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestService>();

        await service!.GetMessageAsync();

        interceptor.WasCalled.Should().BeTrue();
        interceptor.LastMethodName.Should().Be("GetMessageAsync");
    }

    [Fact]
    public void DynamicProxy_WithServiceSpecificInterceptor_ShouldInterceptCalls()
    {
        var services = new ServiceCollection();
        var interceptor = new TestInterceptor();
        var settings = new DynamicProxiedServiceSettings();
        settings.AddProxyInterceptorInstance(interceptor);
        
        services.AddDyanamicProxyScoped<ITestService, TestService>(settings);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestService>();

        service!.GetMessage();

        interceptor.WasCalled.Should().BeTrue();
        interceptor.LastMethodName.Should().Be("GetMessage");
    }

    [Fact]
    public void DynamicProxy_WithInterceptorInstance_ShouldInterceptCalls()
    {
        var services = new ServiceCollection();
        var interceptor = new TestInterceptor();
        var settings = new DynamicProxiedServiceSettings();
        settings.AddProxyInterceptorInstance(interceptor);
        
        services.AddDyanamicProxyScoped<ITestService, TestService>(settings);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestService>();

        service!.GetMessage();

        interceptor.WasCalled.Should().BeTrue();
        interceptor.LastMethodName.Should().Be("GetMessage");
    }

    [Fact]
    public void TryAddProxyGenerator_ShouldRegisterProxyGenerator()
    {
        var services = new ServiceCollection();

        services.TryAddProxyGenerator();

        var serviceProvider = services.BuildServiceProvider();
        var proxyGenerator = serviceProvider.GetService<ProxyGenerator>();

        proxyGenerator.Should().NotBeNull();
    }

    [Fact]
    public void TryAddProxyGenerator_CalledTwice_ShouldRegisterOnlyOnce()
    {
        var services = new ServiceCollection();

        services.TryAddProxyGenerator();
        services.TryAddProxyGenerator();

        var serviceProvider = services.BuildServiceProvider();
        var proxyGenerators = serviceProvider.GetServices<ProxyGenerator>();

        proxyGenerators.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    [InlineData(ServiceLifetime.Singleton)]
    public void AddServiceLifetime_WithInterface_ShouldRegisterWithCorrectLifetime(ServiceLifetime lifetime)
    {
        var services = new ServiceCollection();

        services.AddServiceLifetime<ITestService, TestService>(lifetime);

        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITestService));
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(lifetime);
        descriptor.ImplementationType.Should().Be(typeof(TestService));
    }

    [Theory]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    [InlineData(ServiceLifetime.Singleton)]
    public void AddServiceLifetime_WithoutInterface_ShouldRegisterWithCorrectLifetime(ServiceLifetime lifetime)
    {
        var services = new ServiceCollection();

        services.AddServiceLifetime<TestService>(lifetime);

        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(TestService));
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(lifetime);
        descriptor.ImplementationType.Should().Be(typeof(TestService));
    }
}