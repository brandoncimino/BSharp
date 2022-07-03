using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using NUnit.Framework;

namespace FowlFever.Testing;

/// <summary>
/// Tracks the numbers of times an <see cref="IEnumerable{T}"/> gets enumerated.
/// </summary>
public record LoopCounter<T> {
    public string?            Nickname     { get; init; }
    public ConcurrentQueue<T> History      { get; } = new();
    public Action<T>?         ActionOnLoop { get; init; }

    public T Loop(T element) {
        History.Enqueue(element);
        ActionOnLoop?.Invoke(element);
        return element;
    }
}

public static class LoopCounterExtensions {
    /// <summary>
    /// Creates a <see cref="LoopCounter{T}"/> that will track the number of times <paramref name="source"/> gets enumerated.
    /// </summary>
    /// <param name="source">this <see cref="IEnumerable{T}"/></param>
    /// <param name="loopCounter">the new <see cref="LoopCounter{T}"/></param>
    /// <param name="onLoop">an optional bonus action on each <see cref="LoopCounter{T}.Loop"/></param>
    /// <param name="parameterName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <typeparam name="T">the type of elements in <paramref name="source"/></typeparam>
    /// <returns><paramref name="source"/>, so that it can continue to be chained through Linq methods</returns>
    public static IEnumerable<T> Counting<T>(this IEnumerable<T> source, out LoopCounter<T> loopCounter, Action<T>? onLoop = default, [CallerArgumentExpression("source")] string? parameterName = default) {
        loopCounter = new LoopCounter<T> {
            Nickname     = parameterName,
            ActionOnLoop = onLoop
        };
        return source.Select(loopCounter.Loop);
    }

    public static IEnumerable<T> AssertCounter<T>(this IEnumerable<T> source, LoopCounter<T> loopCounter, int expected, [CallerArgumentExpression("source")] string? description = default) {
        Assert.That(loopCounter.History, Has.Exactly(expected).Items, description);
        return source;
    }

    public static IList<T> AssertCounter<T>(this IList<T> source, LoopCounter<T> loopCounter, int expected, [CallerArgumentExpression("source")] string? description = default) {
        AssertCounter(source.AsEnumerable(), loopCounter, expected, description);
        return source;
    }
}