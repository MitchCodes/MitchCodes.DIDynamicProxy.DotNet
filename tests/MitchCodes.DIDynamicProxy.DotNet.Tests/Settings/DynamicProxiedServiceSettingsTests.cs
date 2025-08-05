namespace MitchCodes.DIDynamicProxy.DotNet.Tests.Settings;

using Castle.DynamicProxy;
using FluentAssertions;
using MitchCodes.DIDynamicProxy.DotNet.Settings;
using MitchCodes.DIDynamicProxy.DotNet.Tests.TestHelpers;
using Xunit;

public class DynamicProxiedServiceSettingsTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeEmptyLists()
    {
        var settings = new DynamicProxiedServiceSettings();

        settings.ProxyInterceptorInstances.Should().NotBeNull().And.BeEmpty();
        settings.ProxyInterceptorTypes.Should().NotBeNull().And.BeEmpty();
        settings.ProxyAsyncInterceptorInstances.Should().NotBeNull().And.BeEmpty();
        settings.ProxyAsyncInterceptorTypes.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Constructor_WithAllParameters_ShouldSetAllProperties()
    {
        var interceptorInstances = new List<IInterceptor> { new TestInterceptor() };
        var interceptorTypes = new List<Type> { typeof(TestInterceptor) };
        var asyncInterceptorInstances = new List<IAsyncInterceptor> { new TestAsyncInterceptor() };
        var asyncInterceptorTypes = new List<Type> { typeof(TestAsyncInterceptor) };

        var settings = new DynamicProxiedServiceSettings(
            interceptorInstances, 
            interceptorTypes, 
            asyncInterceptorInstances, 
            asyncInterceptorTypes);

        settings.ProxyInterceptorInstances.Should().BeSameAs(interceptorInstances);
        settings.ProxyInterceptorTypes.Should().BeSameAs(interceptorTypes);
        settings.ProxyAsyncInterceptorInstances.Should().BeSameAs(asyncInterceptorInstances);
        settings.ProxyAsyncInterceptorTypes.Should().BeSameAs(asyncInterceptorTypes);
    }

    [Fact]
    public void Constructor_WithSyncInterceptors_ShouldSetSyncProperties()
    {
        var interceptorInstances = new List<IInterceptor> { new TestInterceptor() };
        var interceptorTypes = new List<Type> { typeof(TestInterceptor) };

        var settings = new DynamicProxiedServiceSettings(interceptorInstances, interceptorTypes);

        settings.ProxyInterceptorInstances.Should().BeSameAs(interceptorInstances);
        settings.ProxyInterceptorTypes.Should().BeSameAs(interceptorTypes);
        settings.ProxyAsyncInterceptorInstances.Should().BeEmpty();
        settings.ProxyAsyncInterceptorTypes.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithAsyncInterceptors_ShouldSetAsyncProperties()
    {
        var asyncInterceptorInstances = new List<IAsyncInterceptor> { new TestAsyncInterceptor() };
        var asyncInterceptorTypes = new List<Type> { typeof(TestAsyncInterceptor) };

        var settings = new DynamicProxiedServiceSettings(asyncInterceptorInstances, asyncInterceptorTypes);

        settings.ProxyAsyncInterceptorInstances.Should().BeSameAs(asyncInterceptorInstances);
        settings.ProxyAsyncInterceptorTypes.Should().BeSameAs(asyncInterceptorTypes);
        settings.ProxyInterceptorInstances.Should().BeEmpty();
        settings.ProxyInterceptorTypes.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithInterceptorInstances_ShouldSetInterceptorInstances()
    {
        var interceptorInstances = new List<IInterceptor> { new TestInterceptor() };

        var settings = new DynamicProxiedServiceSettings(interceptorInstances);

        settings.ProxyInterceptorInstances.Should().BeSameAs(interceptorInstances);
        settings.ProxyInterceptorTypes.Should().BeEmpty();
        settings.ProxyAsyncInterceptorInstances.Should().BeEmpty();
        settings.ProxyAsyncInterceptorTypes.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithAsyncInterceptorInstances_ShouldSetAsyncInterceptorInstances()
    {
        var asyncInterceptorInstances = new List<IAsyncInterceptor> { new TestAsyncInterceptor() };

        var settings = new DynamicProxiedServiceSettings(asyncInterceptorInstances);

        settings.ProxyAsyncInterceptorInstances.Should().BeSameAs(asyncInterceptorInstances);
        settings.ProxyInterceptorInstances.Should().BeEmpty();
        settings.ProxyInterceptorTypes.Should().BeEmpty();
        settings.ProxyAsyncInterceptorTypes.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithTypes_ShouldSetTypes()
    {
        var interceptorTypes = new List<Type> { typeof(TestInterceptor) };
        var asyncInterceptorTypes = new List<Type> { typeof(TestAsyncInterceptor) };

        var settings = new DynamicProxiedServiceSettings(interceptorTypes, asyncInterceptorTypes);

        settings.ProxyInterceptorTypes.Should().BeSameAs(interceptorTypes);
        settings.ProxyAsyncInterceptorTypes.Should().BeSameAs(asyncInterceptorTypes);
        settings.ProxyInterceptorInstances.Should().BeEmpty();
        settings.ProxyAsyncInterceptorInstances.Should().BeEmpty();
    }

    [Fact]
    public void AddProxyInterceptorInstance_ShouldAddToList()
    {
        var settings = new DynamicProxiedServiceSettings();
        var interceptor = new TestInterceptor();

        var result = settings.AddProxyInterceptorInstance(interceptor);

        result.Should().BeSameAs(settings);
        settings.ProxyInterceptorInstances.Should().Contain(interceptor);
    }

    [Fact]
    public void AddProxyInterceptor_Generic_ShouldAddTypeToList()
    {
        var settings = new DynamicProxiedServiceSettings();

        var result = settings.AddProxyInterceptor<TestInterceptor>();

        result.Should().BeSameAs(settings);
        settings.ProxyInterceptorTypes.Should().Contain(typeof(TestInterceptor));
    }

    [Fact]
    public void AddProxyInterceptor_Type_ShouldAddTypeToList()
    {
        var settings = new DynamicProxiedServiceSettings();

        var result = settings.AddProxyInterceptor(typeof(TestInterceptor));

        result.Should().BeSameAs(settings);
        settings.ProxyInterceptorTypes.Should().Contain(typeof(TestInterceptor));
    }

    [Fact]
    public void AddProxyInterceptor_InvalidType_ShouldThrowException()
    {
        var settings = new DynamicProxiedServiceSettings();

        var action = () => settings.AddProxyInterceptor(typeof(string));

        action.Should().Throw<Exception>()
            .WithMessage("*is not assignable to IInterceptor");
    }

    [Fact]
    public void AddAsyncProxyInterceptorInstance_ShouldAddToList()
    {
        var settings = new DynamicProxiedServiceSettings();
        var interceptor = new TestAsyncInterceptor();

        var result = settings.AddAsyncProxyInterceptorInstance(interceptor);

        result.Should().BeSameAs(settings);
        settings.ProxyAsyncInterceptorInstances.Should().Contain(interceptor);
    }

    [Fact]
    public void AddAsyncProxyInterceptor_Generic_ShouldAddTypeToList()
    {
        var settings = new DynamicProxiedServiceSettings();

        var result = settings.AddAsyncProxyInterceptor<TestAsyncInterceptor>();

        result.Should().BeSameAs(settings);
        settings.ProxyAsyncInterceptorTypes.Should().Contain(typeof(TestAsyncInterceptor));
    }

    [Fact]
    public void AddAsyncProxyInterceptor_Type_ShouldAddTypeToList()
    {
        var settings = new DynamicProxiedServiceSettings();

        var result = settings.AddAsyncProxyInterceptor(typeof(TestAsyncInterceptor));

        result.Should().BeSameAs(settings);
        settings.ProxyAsyncInterceptorTypes.Should().Contain(typeof(TestAsyncInterceptor));
    }

    [Fact]
    public void AddAsyncProxyInterceptor_InvalidType_ShouldThrowException()
    {
        var settings = new DynamicProxiedServiceSettings();

        var action = () => settings.AddAsyncProxyInterceptor(typeof(string));

        action.Should().Throw<Exception>()
            .WithMessage("*is not assignable to IAsyncInterceptor");
    }

    [Fact]
    public void FluentInterface_ShouldAllowChaining()
    {
        var settings = new DynamicProxiedServiceSettings();
        var interceptor = new TestInterceptor();
        var asyncInterceptor = new TestAsyncInterceptor();

        var result = settings
            .AddProxyInterceptorInstance(interceptor)
            .AddProxyInterceptor<TestInterceptor>()
            .AddAsyncProxyInterceptorInstance(asyncInterceptor)
            .AddAsyncProxyInterceptor<TestAsyncInterceptor>();

        result.Should().BeSameAs(settings);
        settings.ProxyInterceptorInstances.Should().Contain(interceptor);
        settings.ProxyInterceptorTypes.Should().Contain(typeof(TestInterceptor));
        settings.ProxyAsyncInterceptorInstances.Should().Contain(asyncInterceptor);
        settings.ProxyAsyncInterceptorTypes.Should().Contain(typeof(TestAsyncInterceptor));
    }
}