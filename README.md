# MitchCodes.DIDynamicProxy.DotNet

MitchCodes.DIDynamicProxy.DotNet is a library designed to simplify the integration of dynamic proxies into .NET applications using Dependency Injection (DI) with [Castle.Core](https://github.com/castleproject/Core) `IInterceptor`s and [Castle.Core.AsyncInterceptor](https://github.com/JSkimming/Castle.Core.AsyncInterceptor) `IAsyncInterceptor`s. This library is aimed at developers looking to enhance their applications with aspects such as logging, caching, security, and more, by intercepting method calls on objects at runtime. By leveraging the functionality provided, developers can apply cross-cutting concerns to their services without modifying the actual code of their classes, thus adhering to the Open/Closed principle.

## Features

- Easy registration of classes for proxying within the Microsoft.Extensions.DependencyInjection DI container.
- Support for synchronous and asynchronous method interception using Castle.Core interceptors.
- Support for dependency injection inside the interceptors.
- Flexible configuration for interceptors, allowing for both instance-based and type-based registration.
- Integration with Castle.Core.AsyncInterceptor for handling asynchronous method calls seamlessly.
- Utility methods for service lifetime management, making it straightforward to control the scope of proxied services.
- Convenient abstract classes for creating interceptors that are controlled via an Attribute

## Requirements

- *.NET 6* and up

## Getting Started

To use MitchCodes.DIDynamicProxy.DotNet in your project, first install the package from NuGet:

```shell
Install-Package MitchCodes.DIDynamicProxy.DotNet
```

## Usage

### Setting up interceptors and proxied services through Dependency Injection

1. **Register app-level interceptors wherever you configure your services**

   This example demonstrates how to register a `LogInterceptor` and `CacheAsyncInterceptor` at the app-level that are applied to every proxied service added by this library. The interceptors can be registered as scoped, transient, singleton or singleton with a provided instance.

   ```csharp
   using MitchCodes.DIDynamicProxy.DotNet.Extensions;
   using Microsoft.Extensions.DependencyInjection;

   public void ConfigureServices(IServiceCollection services)
   {
       // Assuming LogInterceptor is an IInterceptor implementation
       services.AddInterceptorSingleton<LogInterceptor>();

       // Assuming CacheAsyncInterceptor is an IAsyncInterceptor implementation
       services.AddAsyncInterceptorSingleton<CacheAsyncInterceptor>();
   }
   ```

2. **Register proxied services wherever you configure your services**

   This example demonstrates how you can register services that will be created as a proxy. Any app-level interceptors will be resolved and added automatically during the creation of the proxy. Additionally, you can configure your services by either directly adding instances or by registering types of interceptors that will be added specifically to the service.

   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       // Assuming LogInterceptor is an IInterceptor implementation
       services.AddInterceptorSingleton<LogInterceptor>();

       // Register your service and its proxy as before
       services.AddSingletonDynamicProxy<IMyService, MyService>();

       // Register another service with some specific interceptors
       services.AddSingletonDynamicProxy<IMyOtherService, MyOtherService>(new DynamicProxiedServiceSettings()
       {
           ProxyAsyncInterceptorTypes = new List<Type>() { typeof(CacheAsyncInterceptor) }
       });
   }
   ```

### Implementing Interceptors

- **Synchronous Interceptor**

  Implement the `IInterceptor` interface for synchronous method interception. Here is a simple logging interceptor:

  ```csharp
  public class LogInterceptor : IInterceptor
  {
      public void Intercept(IInvocation invocation)
      {
          Console.WriteLine($"Before executing {invocation.Method.Name}");
          invocation.Proceed();
          Console.WriteLine($"After executing {invocation.Method.Name}");
      }
  }
  ```

- **Asynchronous Interceptor**

  For asynchronous method interception, implement the `IAsyncInterceptor` interface. Here is a sample async interceptor:

    ```csharp
    public class AsyncInterceptor : IAsyncInterceptor
    {
        public void InterceptSynchronous(IInvocation invocation) => this.InternalInterceptSynchronous(invocation);

        public void InterceptAsynchronous(IInvocation invocation) => invocation.ReturnValue = this.InternalInterceptAsynchronous(invocation);

        public void InterceptAsynchronous<TResult>(IInvocation invocation) => invocation.ReturnValue = this.InternalInterceptAsynchronous<TResult>(invocation);

        private void InternalInterceptSynchronous(IInvocation invocation)
        {
            // Step 1. Do something prior to invocation.

            invocation.Proceed();

            // Step 2. Do something after invocation.
        }

        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            // Step 1. Do something prior to invocation.

            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;

            // Step 2. Do something after invocation.
        }

        private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            // Step 1. Do something prior to invocation.

            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            var result = await task;

            // Step 2. Do something after invocation.

            return result;
        }
    }
    ```

  Read more about implementing `IAsyncInterceptor` [here](https://github.com/JSkimming/Castle.Core.AsyncInterceptor#option-1--implement-iasyncinterceptor-interface-to-intercept-invocations).

- **Attribute-based interception**

  This library provides two abstract classes for creating interceptors that can be applied at the function-level of a proxied service: `AbstractFunctionAttributeInterceptor<TAttribute>` and `AbstractFunctionAttributeAsyncInterceptor<TAttribute>`. Using these classes, you can restrict the interception logic to only occur when the `TAttribute` attribute is applied to the function that is being invoked. The `InterceptSynchronous`, `InterceptAsynchronous` and `InterceptAsynchronous<TResult>` abstract functions are only called when the function being invoked has the attribute. The attribute is provided in the abstract functions. Below is an example of creating an attribute interceptor, using it in a service and registering it:

    ```csharp
    public class LogAttribute : Attribute
    {
        public string CustomMessage { get; set; }
    }

    public class LogAsyncInterceptor : AbstractFunctionAttributeAsyncInterceptor<LogAttribute>
    {
        private readonly ILogger<LogAsyncInterceptor>? _logger;

        public LogAsyncInterceptor(ILogger<LogAsyncInterceptor> logger) => this._logger = logger;

        public override void InterceptSynchronous(IInvocation invocation, LogAttribute attribute)
        {
            // Step 1. Do something prior to invocation.
            this.LogBefore(invocation, attribute);

            invocation.Proceed();

            // Step 2. Do something after invocation.
            this.LogAfter(invocation, attribute);
        }

        public override async Task InterceptAsynchronous(IInvocation invocation, LogAttribute attribute)
        {
            // Step 1. Do something prior to invocation.
            this.LogBefore(invocation, attribute);

            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;

            // Step 2. Do something after invocation.
            this.LogAfter(invocation, attribute);
        }

        public override async Task<TResult> InterceptAsynchronous<TResult>(IInvocation invocation, LogAttribute attribute)
        {
            // Step 1. Do something prior to invocation.
            this.LogBefore(invocation, attribute);

            invocation.Proceed();
            var task = (Task<TResult>)invocation.ReturnValue;
            var result = await task;

            // Step 2. Do something after invocation.
            this.LogAfter(invocation, attribute);

            return result;
        }

        private void LogBefore(IInvocation invocation, LogAttribute attribute)
        {
            this._logger?.LogInformation($"Hello from before {invocation.Method.Name}: {attribute.CustomMessage}");
        }

        private void LogAfter(IInvocation invocation, LogAttribute attribute)
        {
            this._logger?.LogInformation($"Hello from after {invocation.Method.Name}: {attribute.CustomMessage}");
        }
    }

    public interface IEchoService
    {
        Task<string> EchoAsync(string input);
        Task<string> EchoNoLogAsync(string input);
    }

    public class EchoService : IEchoService
    {
        [Log(CustomMessage = "Echo")]
        public async Task<string> EchoAsync(string input)
        {
            await Task.Delay(1000);

            return input;
        }

        // just here as a demo
        public async Task<string> EchoNoLogAsync(string input)
        {
            await Task.Delay(1000);

            return input;
        }
    }

    // in Program.cs or whereever you configure your services
    public void ConfigureServices(IServiceCollection services)
    {
        // Since LogAsyncInterceptor is added at the app-level, all proxies will use this interceptor for all functions
        //  However, the requirement for the attribute means that for all other functions without the attribute, the interceptor will do nothing
        services.AddAsyncInterceptorScoped<LogAsyncInterceptor>();

        // Register your service and its proxy as before
        services.AddScopedDynamicProxy<IEchoService, EchoService>();
    }
    ```

## Remarks

- Unless you know you only are going to intercept syncronous functions, only need to do work *before* proceeding, or are going to manually handle `Task` return types from the intercepted function, I recommend to use `IAsyncInterceptor`. Read more about the benefits of IAsyncInterceptor [here](https://github.com/JSkimming/Castle.Core.AsyncInterceptor#whats-not-simple-about-asynchronous-method-interception).
