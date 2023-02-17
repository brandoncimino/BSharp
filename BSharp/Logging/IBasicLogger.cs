using System;

using Microsoft.Extensions.Logging;

namespace FowlFever.BSharp.Logging;

/// <summary>
/// Partially implements the <see cref="ILogger{T}"/> interface to make it easier to work with. 
/// </summary>
/// <typeparam name="T"><inheritdoc cref="ILogger{T}"/></typeparam>
internal interface IBasicLogger<out T> : ILogger<T> {
    /// <summary>
    /// Only messages <b>greater than or equal to</b> this <see cref="LogLevel"/> will actually get logged.
    /// </summary>
    public LogLevel Level { get; }

    bool ILogger.IsEnabled(LogLevel logLevel) {
        return logLevel >= Level;
    }

    void ILogger.Log<TState>(
        LogLevel                         logLevel,
        EventId                          eventId,
        TState                           state,
        Exception?                       exception,
        Func<TState, Exception?, string> formatter
    ) {
        if (!IsEnabled(logLevel)) {
            return;
        }

        var message = formatter(state, exception);
        WriteLogMessage(logLevel, message);
    }

    /// <summary>
    /// Takes in a fully formatted log message and does something with it.
    /// </summary>
    /// <param name="logLevel">the <see cref="LogLevel"/> of the message</param>
    /// <param name="message">the actual message <see cref="string"/></param>
    protected void WriteLogMessage(LogLevel logLevel, string message);

    IDisposable ILogger.BeginScope<TState>(TState state) {
        throw new NotImplementedException();
    }
}