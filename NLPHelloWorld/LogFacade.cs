using AboditNLP;
using Microsoft.Extensions.Logging;
using System;

namespace NLPHelloWorld
{
    /// <summary>
    /// Simple console logger for demonstration implementing the Microsoft abstraction
    /// Use SeriLog or similar in your application.
    /// </remarks>
    internal class ConsoleLogger : ILogger, ILogger<NLP>
    {
        private class DisposableScope : IDisposable
        {
            private bool disposedValue;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new DisposableScope();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.WriteLine(formatter(state, exception));
        }
    }
}