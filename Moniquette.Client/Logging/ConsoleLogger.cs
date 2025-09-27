using Microsoft.Extensions.Logging;

namespace Moniquette.Client.Logging;

public class ConsoleLogger : ILogger
{
    public void Info<T>(T message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[INFO] ");
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void Warn<T>(T message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[WARN] ");
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void Error<T>(T message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ERROR] ");
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}