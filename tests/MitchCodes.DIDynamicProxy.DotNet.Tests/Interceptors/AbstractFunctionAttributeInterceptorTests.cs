namespace MitchCodes.DIDynamicProxy.DotNet.Tests.Interceptors;

using Castle.DynamicProxy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MitchCodes.DIDynamicProxy.DotNet.Extensions;
using MitchCodes.DIDynamicProxy.DotNet.Settings;
using MitchCodes.DIDynamicProxy.DotNet.Tests.TestHelpers;
using Xunit;

public class AbstractFunctionAttributeInterceptorTests
{
    [Fact]
    public void Intercept_WithAttribute_ShouldCallCustomInterceptMethod()
    {
        var services = new ServiceCollection();
        var settings = new DynamicProxiedServiceSettings();
        var interceptor = new TestAttributeInterceptor();
        settings.AddProxyInterceptorInstance(interceptor);
        
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
    public void Intercept_WithoutAttribute_ShouldNotCallCustomInterceptMethod()
    {
        var services = new ServiceCollection();
        var settings = new DynamicProxiedServiceSettings();
        var interceptor = new TestAttributeInterceptor();
        settings.AddProxyInterceptorInstance(interceptor);
        
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
    public void AbstractFunctionAttributeInterceptor_ShouldOnlyInterceptMethodsWithSpecificAttribute()
    {
        var services = new ServiceCollection();
        var interceptor = new TestAttributeInterceptor();
        var settings = new DynamicProxiedServiceSettings();
        settings.AddProxyInterceptorInstance(interceptor);
        
        services.AddDyanamicProxyScoped<ITestServiceWithAttributes, TestServiceWithAttributes>(settings);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceWithAttributes>();

        service!.GetMessage();
        interceptor.WasCalled.Should().BeTrue();
        interceptor.LastMethodName.Should().Be("GetMessage");
        interceptor.LastAttributeMessage.Should().Be("Sync method");

        var interceptor2 = new TestAttributeInterceptor();
        settings = new DynamicProxiedServiceSettings();
        settings.AddProxyInterceptorInstance(interceptor2);
        
        services = new ServiceCollection();
        services.AddDyanamicProxyScoped<ITestServiceWithAttributes, TestServiceWithAttributes>(settings);
        serviceProvider = services.BuildServiceProvider();
        service = serviceProvider.GetService<ITestServiceWithAttributes>();

        service!.GetMessageWithoutAttribute();

        interceptor2.WasCalled.Should().BeFalse();
        interceptor2.LastMethodName.Should().BeNull();
        interceptor2.LastAttributeMessage.Should().BeNull();
    }
}