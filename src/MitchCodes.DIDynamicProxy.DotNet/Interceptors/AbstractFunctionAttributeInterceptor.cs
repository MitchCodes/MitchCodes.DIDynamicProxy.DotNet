namespace MitchCodes.DIDynamicProxy.DotNet.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using MitchCodes.DIDynamicProxy.DotNet.Extensions;
using MitchCodes.DIDynamicProxy.DotNet.Settings;

public abstract class AbstractFunctionAttributeInterceptor<TAttribute> : IInterceptor
    where TAttribute : Attribute
{
    public abstract void Intercept(IInvocation invocation, TAttribute attribute);

    public void Intercept(IInvocation invocation)
    {
        if (invocation.MethodInvocationTarget
            .GetCustomAttributes(typeof(TAttribute), false)
            .FirstOrDefault() is TAttribute attribute)
        {
            this.Intercept(invocation, attribute);
        } else
        {
            invocation.Proceed();
        }
    }
}
