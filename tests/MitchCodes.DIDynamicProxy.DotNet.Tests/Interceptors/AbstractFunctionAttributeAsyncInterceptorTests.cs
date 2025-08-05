namespace MitchCodes.DIDynamicProxy.DotNet.Tests.Interceptors;

using Castle.DynamicProxy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MitchCodes.DIDynamicProxy.DotNet.Extensions;
using MitchCodes.DIDynamicProxy.DotNet.Settings;
using MitchCodes.DIDynamicProxy.DotNet.Tests.TestHelpers;
using Xunit;

public class AbstractFunctionAttributeAsyncInterceptorTests
{
    [Fact]
    public void InterceptSynchronous_WithAttribute_ShouldCallCustomInterceptMethod()
    {
        var services = new ServiceCollection();
        var interceptor = new TestAttributeAsyncInterceptor();
        var settings = new DynamicProxiedServiceSettings();
        settings.AddAsyncProxyInterceptorInstance(interceptor);
        
        services.AddDyanamicProxyScoped<ITestServiceWithAttributes, TestServiceWithAttributes>(settings);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceWithAttributes>();

        var result = service!.GetMessage();

        result.Should().Be("Hello World");
        interceptor.WasCalled.Should().BeTrue();
        interceptor.LastMethodName.Should().Be("GetMessage");
        interceptor.LastAttributeMessage.Should().Be("Sync method");
    }

    [Fact]
    public async Task InterceptAsynchronous_WithAttribute_ShouldCallCustomInterceptMethod()
    {
        var services = new ServiceCollection();
        var settings = new DynamicProxiedServiceSettings();
        var interceptor = new TestAttributeAsyncInterceptor();
        settings.AddAsyncProxyInterceptorInstance(interceptor);
        
        services.AddDyanamicProxyScoped<ITestServiceWithAttributes, TestServiceWithAttributes>(settings);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceWithAttributes>();

        var result = await service!.GetMessageAsync();

        result.Should().Be("Hello World Async");
        interceptor.WasCalled.Should().BeTrue();
        interceptor.LastMethodName.Should().Be("GetMessageAsync");
        interceptor.LastAttributeMessage.Should().Be("Async method");
    }

    [Fact]
    public void InterceptSynchronous_WithoutAttribute_ShouldNotCallCustomInterceptMethod()
    {
        var services = new ServiceCollection();
        var settings = new DynamicProxiedServiceSettings();
        var interceptor = new TestAttributeAsyncInterceptor();
        settings.AddAsyncProxyInterceptorInstance(interceptor);
        
        services.AddDyanamicProxyScoped<ITestServiceWithAttributes, TestServiceWithAttributes>(settings);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceWithAttributes>();

        var result = service!.GetMessageWithoutAttribute();

        result.Should().Be("No attribute");
        interceptor.WasCalled.Should().BeFalse();
        interceptor.LastMethodName.Should().BeNull();
        interceptor.LastAttributeMessage.Should().BeNull();
    }

    [Fact]
    public void AbstractFunctionAttributeAsyncInterceptor_ShouldOnlyInterceptMethodsWithSpecificAttribute()
    {
        var services = new ServiceCollection();
        var settings = new DynamicProxiedServiceSettings();
        var interceptor = new TestAttributeAsyncInterceptor();
        settings.AddAsyncProxyInterceptorInstance(interceptor);
        
        services.AddDyanamicProxyScoped<ITestServiceWithAttributes, TestServiceWithAttributes>(settings);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceWithAttributes>();

        service!.GetMessage();
        interceptor.WasCalled.Should().BeTrue();
        
        var firstCallMethod = interceptor.LastMethodName;
        var firstCallMessage = interceptor.LastAttributeMessage;

        interceptor = new TestAttributeAsyncInterceptor();
        settings = new DynamicProxiedServiceSettings();
        settings.AddAsyncProxyInterceptorInstance(interceptor);
        
        services = new ServiceCollection();
        services.AddDyanamicProxyScoped<ITestServiceWithAttributes, TestServiceWithAttributes>(settings);
        serviceProvider = services.BuildServiceProvider();
        service = serviceProvider.GetService<ITestServiceWithAttributes>();

        service!.GetMessageWithoutAttribute();

        interceptor.WasCalled.Should().BeFalse();
        interceptor.LastMethodName.Should().BeNull();
        interceptor.LastAttributeMessage.Should().BeNull();
    }
}