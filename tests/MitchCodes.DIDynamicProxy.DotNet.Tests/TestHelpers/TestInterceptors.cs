namespace MitchCodes.DIDynamicProxy.DotNet.Tests.TestHelpers;

using Castle.DynamicProxy;

public class TestInterceptor : IInterceptor
{
    public bool WasCalled { get; private set; }
    public string? LastMethodName { get; private set; }

    public void Intercept(IInvocation invocation)
    {
        WasCalled = true;
        LastMethodName = invocation.Method.Name;
        invocation.Proceed();
    }
}

public class TestAsyncInterceptor : IAsyncInterceptor
{
    public bool WasCalled { get; private set; }
    public string? LastMethodName { get; private set; }

    public void InterceptSynchronous(IInvocation invocation)
    {
        WasCalled = true;
        LastMethodName = invocation.Method.Name;
        invocation.Proceed();
    }

    public void InterceptAsynchronous(IInvocation invocation)
    {
        WasCalled = true;
        LastMethodName = invocation.Method.Name;
        invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        WasCalled = true;
        LastMethodName = invocation.Method.Name;
        invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
    }

    private async Task InternalInterceptAsynchronous(IInvocation invocation)
    {
        invocation.Proceed();
        var task = (Task)invocation.ReturnValue;
        await task;
    }

    private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
    {
        invocation.Proceed();
        var task = (Task<TResult>)invocation.ReturnValue;
        return await task;
    }
}

public class TestAttribute : Attribute
{
    public string Message { get; set; } = "";
}

public class TestAttributeAsyncInterceptor : MitchCodes.DIDynamicProxy.DotNet.Interceptors.AbstractFunctionAttributeAsyncInterceptor<TestAttribute>
{
    public bool WasCalled { get; private set; }
    public string? LastMethodName { get; private set; }
    public string? LastAttributeMessage { get; private set; }

    public override void InterceptSynchronous(IInvocation invocation, TestAttribute attribute)
    {
        WasCalled = true;
        LastMethodName = invocation.Method.Name;
        LastAttributeMessage = attribute.Message;
        invocation.Proceed();
    }

    public override async Task InterceptAsynchronous(IInvocation invocation, TestAttribute attribute)
    {
        WasCalled = true;
        LastMethodName = invocation.Method.Name;
        LastAttributeMessage = attribute.Message;
        invocation.Proceed();
        var task = (Task)invocation.ReturnValue;
        await task;
    }

    public override async Task<TResult> InterceptAsynchronous<TResult>(IInvocation invocation, TestAttribute attribute)
    {
        WasCalled = true;
        LastMethodName = invocation.Method.Name;
        LastAttributeMessage = attribute.Message;
        invocation.Proceed();
        var task = (Task<TResult>)invocation.ReturnValue;
        return await task;
    }
}