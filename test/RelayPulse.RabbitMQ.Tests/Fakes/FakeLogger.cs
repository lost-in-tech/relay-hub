using Microsoft.Extensions.Logging;

namespace RelayPulse.RabbitMQ.Tests.Fakes;

internal sealed class FakeLogger<T> : ILogger<T> 
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return false;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}