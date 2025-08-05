namespace MitchCodes.DIDynamicProxy.DotNet.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

public abstract class AbstractFunctionAttributeAsyncInterceptor<TAttribute> : IAsyncInterceptor
    where TAttribute : Attribute
{
    public abstract void InterceptSynchronous(IInvocation invocation, TAttribute attribute);
    public abstract Task InterceptAsynchronous(IInvocation invocation, TAttribute attribute);
    public abstract Task<TResult> InterceptAsynchronous<TResult>(IInvocation invocation, TAttribute attribute);

    public void InterceptSynchronous(IInvocation invocation) => this.InternalInterceptSynchronous(invocation);
    public void InterceptAsynchronous(IInvocation invocation) => invocation.ReturnValue = this.InternalInterceptAsynchronous(invocation);
    public void InterceptAsynchronous<TResult>(IInvocation invocation) => invocation.ReturnValue = this.InternalInterceptAsynchronous<TResult>(invocation);

    private async void InternalInterceptSynchronous(IInvocation invocation)
    {
        if (invocation.MethodInvocationTarget
            .GetCustomAttributes(typeof(TAttribute), false)
            .FirstOrDefault() is TAttribute attribute)
        {
            this.InterceptSynchronous(invocation, attribute);
        }
        else
        {
            invocation.Proceed();
        }
    }

    private async Task InternalInterceptAsynchronous(IInvocation invocation)
    {
        if (invocation.MethodInvocationTarget
            .GetCustomAttributes(typeof(TAttribute), false)
            .FirstOrDefault() is TAttribute attribute)
        {
            await this.InterceptAsynchronous(invocation, attribute);
        } else
        {
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;
        }
    }

    private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
    {
        if (invocation.MethodInvocationTarget
            .GetCustomAttributes(typeof(TAttribute), false)
            .FirstOrDefault() is TAttribute attribute)
        {
            return await this.InterceptAsynchronous<TResult>(invocation, attribute);
        }
        else
        {
            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            var result = await task;

            return result;
        }
    }
}
