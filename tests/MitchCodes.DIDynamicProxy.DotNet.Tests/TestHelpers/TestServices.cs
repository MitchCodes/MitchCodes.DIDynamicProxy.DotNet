namespace MitchCodes.DIDynamicProxy.DotNet.Tests.TestHelpers;

public interface ITestService
{
    string GetMessage();
    Task<string> GetMessageAsync();
    int Add(int a, int b);
    Task<int> AddAsync(int a, int b);
}

public class TestService : ITestService
{
    public string GetMessage() => "Hello World";

    public async Task<string> GetMessageAsync()
    {
        await Task.Delay(1);
        return "Hello World Async";
    }

    public int Add(int a, int b) => a + b;

    public async Task<int> AddAsync(int a, int b)
    {
        await Task.Delay(1);
        return a + b;
    }
}

public interface ITestServiceWithAttributes
{
    [TestAttribute(Message = "Sync method")]
    string GetMessage();
    
    [TestAttribute(Message = "Async method")]
    Task<string> GetMessageAsync();
    
    string GetMessageWithoutAttribute();
}

public class TestServiceWithAttributes : ITestServiceWithAttributes
{
    [TestAttribute(Message = "Sync method")]
    public string GetMessage() => "Hello World";

    [TestAttribute(Message = "Async method")]
    public async Task<string> GetMessageAsync()
    {
        await Task.Delay(1);
        return "Hello World Async";
    }

    public string GetMessageWithoutAttribute() => "No attribute";
}