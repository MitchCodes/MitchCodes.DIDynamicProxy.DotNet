namespace MitchCodes.DIDynamicProxy.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

public class GlobalInterceptorWrapper
{
    public IInterceptor Interceptor { get; }

    public GlobalInterceptorWrapper(IInterceptor interceptor) => this.Interceptor = interceptor;
}

public class GlobalAsyncInterceptorWrapper
{
    public IAsyncInterceptor AsyncInterceptor { get; }

    public GlobalAsyncInterceptorWrapper(IAsyncInterceptor asyncInterceptor) => this.AsyncInterceptor = asyncInterceptor;
}

/*
public class ScopedLifetimeWrapper<T>
{
    public T Object { get; set; }

    public ScopedLifetimeWrapper(T obj) => this.Object = obj;
}

public class TransientLifetimeWrapper<T>
{
    public T Object { get; set; }

    public TransientLifetimeWrapper(T obj) => this.Object = obj;
}

public class SingletonLifetimeWrapper<T>
{
    public T Object { get; set; }

    public SingletonLifetimeWrapper(T obj) => this.Object = obj;
}
*/






/*
public interface ILifetimeScopeWrapper<T>
{
    T Object { get; }
}

public class ScopedInterceptor<T> : ILifetimeScopeWrapper<T> where T : IInterceptor
{
    public T Object { get; set; }

    public ScopedInterceptor(T interceptor) => this.Object = interceptor;
}

public class TransientInterceptor<T> : ILifetimeScopeWrapper<T> where T : IInterceptor
{
    public T Object { get; set; }

    public TransientInterceptor(T interceptor) => this.Object = interceptor;
}

public class SingletonInterceptor<T> : ILifetimeScopeWrapper<T> where T : IInterceptor
{
    public T Object { get; set; }

    public SingletonInterceptor(T interceptor) => this.Object = interceptor;
}

public class ScopedAsyncInterceptor<T> : ILifetimeScopeWrapper<T> where T : IAsyncInterceptor
{
    public T Object { get; set; }

    public ScopedAsyncInterceptor(T interceptor) => this.Object = interceptor;
}

public class TransientAsyncInterceptor<T> : ILifetimeScopeWrapper<T> where T : IAsyncInterceptor
{
    public T Object { get; set; }

    public TransientAsyncInterceptor(T interceptor) => this.Object = interceptor;
}

public class SingletonAsyncInterceptor<T> : ILifetimeScopeWrapper<T> where T : IAsyncInterceptor
{
    public T Object { get; set; }

    public SingletonAsyncInterceptor(T interceptor) => this.Object = interceptor;
}
*/
