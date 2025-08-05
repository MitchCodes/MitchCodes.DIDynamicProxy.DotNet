namespace MitchCodes.DIDynamicProxy.DotNet.Tests;

using Castle.DynamicProxy;
using FluentAssertions;
using MitchCodes.DIDynamicProxy.DotNet.Tests.TestHelpers;
using Xunit;

public class LifetimeWrappersTests
{
    [Fact]
    public void GlobalInterceptorWrapper_ShouldWrapInterceptor()
    {
        var interceptor = new TestInterceptor();

        var wrapper = new GlobalInterceptorWrapper(interceptor);

        wrapper.Interceptor.Should().BeSameAs(interceptor);
    }

    [Fact]
    public void GlobalAsyncInterceptorWrapper_ShouldWrapAsyncInterceptor()
    {
        var asyncInterceptor = new TestAsyncInterceptor();

        var wrapper = new GlobalAsyncInterceptorWrapper(asyncInterceptor);

        wrapper.AsyncInterceptor.Should().BeSameAs(asyncInterceptor);
    }
}