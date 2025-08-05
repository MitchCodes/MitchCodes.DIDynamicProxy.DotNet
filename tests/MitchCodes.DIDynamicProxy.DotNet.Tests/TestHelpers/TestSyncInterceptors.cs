namespace MitchCodes.DIDynamicProxy.DotNet.Tests.TestHelpers;

using Castle.DynamicProxy;
using MitchCodes.DIDynamicProxy.DotNet.Interceptors;

public class TestAttributeInterceptor : AbstractFunctionAttributeInterceptor<TestAttribute>
{
    public bool WasCalled { get; private set; }
    public string? LastMethodName { get; private set; }
    public string? LastAttributeMessage { get; private set; }

    public override void Intercept(IInvocation invocation, TestAttribute attribute)
    {
        WasCalled = true;
        LastMethodName = invocation.Method.Name;
        LastAttributeMessage = attribute.Message;
        invocation.Proceed();
    }
}