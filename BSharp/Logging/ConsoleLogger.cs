using System;
using System.Collections.Concurrent;

using FowlFever.BSharp.Collections;

using Microsoft.Extensions.Logging;

namespace FowlFever.BSharp.Logging;

internal static class ConsoleLoggerCache {
    private static readonly ConcurrentDictionary<Type, Lazy<ILogger>> Cache = new();

    public static ConsoleLogger<T> GetLogger<T>(this T owner) {
        return (ConsoleLogger<T>)Cache.GetOrAddLazily(typeof(T), static t => new ConsoleLogger<T>());
    }
}

/// <summary>
/// A super simple implementation of <see cref="ILogger"/>.
/// </summary>
/// <typeparam name="T"><inheritdoc cref="ILogger"/></typeparam>
internal class ConsoleLogger<T> : IBasicLogger<T> {
    public LogLevel Level { get; init; } = LogLevel.Information;

    public void WriteLogMessage(LogLevel logLevel, string message) {
        Console.WriteLine($"{logLevel.GetLabel()} {message}");
    }
}