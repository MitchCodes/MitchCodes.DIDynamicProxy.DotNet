# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

MitchCodes.DIDynamicProxy.DotNet is a .NET 6 library that simplifies the integration of dynamic proxies into .NET applications using Dependency Injection with Castle.Core IInterceptors and IAsyncInterceptors. The library enables aspect-oriented programming for cross-cutting concerns like logging, caching, and security.

## Common Development Commands

### Build and Test
```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release

# Run tests (if any exist)
dotnet test --configuration Release --verbosity normal

# Pack the NuGet package
dotnet pack --configuration Release --output ./artifacts
```

### Development Workflow
- The project automatically generates NuGet packages on build (`GeneratePackageOnBuild` is enabled)
- Symbol packages are generated for debugging support
- Target framework is .NET 6.0 with implicit usings and nullable reference types enabled

## Architecture Overview

### Core Components

**ServiceCollectionProxyExtensions** (`src/Extensions/ServiceCollectionProxyExtensions.cs`):
- Primary API for registering dynamic proxies in DI container
- Provides extension methods like `AddDynamicProxyScoped<TInterface, TImplementation>()`
- Handles both synchronous (`IInterceptor`) and asynchronous (`IAsyncInterceptor`) interceptors
- Supports app-level interceptors that apply to all proxied services
- Supports service-specific interceptors via `DynamicProxiedServiceSettings`

**Abstract Interceptor Base Classes**:
- `AbstractFunctionAttributeInterceptor<TAttribute>`: For attribute-based synchronous interception
- `AbstractFunctionAttributeAsyncInterceptor<TAttribute>`: For attribute-based asynchronous interception
- These classes enable method-level interception controlled by custom attributes

**Configuration**:
- `DynamicProxiedServiceSettings`: Configures interceptors for specific services
- `LifetimeWrappers.cs`: Contains wrapper classes for global interceptors

### Key Dependencies
- `Castle.Core.AsyncInterceptor` (v2.1.0): Provides async interception capabilities
- `Microsoft.Extensions.DependencyInjection.Abstractions` (v6.0.0): For DI integration

### Proxy Registration Pattern
1. Register global interceptors using `AddInterceptor<T>()` or `AddAsyncInterceptor<T>()`
2. Register proxied services using `AddDynamicProxy[Lifetime]<TInterface, TImplementation>()`
3. Optionally configure service-specific interceptors via `DynamicProxiedServiceSettings`

The library automatically:
- Registers the Castle `ProxyGenerator` as a singleton
- Resolves and applies global interceptors to all proxied services
- Creates interface proxies that wrap the actual implementations
- Handles composition of sync and async interceptors on the same service

## Important Notes

- Use `IAsyncInterceptor` for better async method handling unless only intercepting synchronous methods
- Global interceptors apply to ALL proxied services registered through this library
- Service-specific interceptors are added on top of global interceptors
- The library supports both type-based and instance-based interceptor registration
- Method names use "Dyanamic" (with typo) in some extension methods for backward compatibility