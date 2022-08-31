using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional;

[PublicAPI]
public static class NullableExtensions {
    [Pure]
    public static IEnumerable<T> AsEnumerable<T>(this T? nullableValue)
        where T : struct {
        return nullableValue.HasValue ? Enumerable.Repeat(nullableValue.Value, 1) : Enumerable.Empty<T>();
    }

    [Pure]
    public static Optional<T> ToOptional<T>(this T? nullableValue) {
        return nullableValue == null ? Optional.Empty<T>() : Optional.Of(nullableValue);
    }

    [Pure]
    public static Optional<T> ToOptional<T>(this T? nullableValue)
        where T : struct {
        return nullableValue.HasValue ? Optional.Of(nullableValue.Value) : default;
    }

    /// <returns>negation of <see cref="Nullable{T}.HasValue"/></returns>
    [Pure]
    public static bool IsEmpty<T>([NotNullWhen(false)] this T? nullable)
        where T : struct => nullable.HasValue == false;

    /// <param name="nullable">this <see cref="Nullable{T}"/> value</param>
    /// <typeparam name="T">this <see cref="Nullable.GetUnderlyingType"/></typeparam>
    /// <returns>an <see cref="IEnumerable{T}"/> containing the <see cref="Nullable{T}.Value"/>, if this <see cref="Nullable{T}.HasValue"/>; otherwise, <see cref="Enumerable.Empty{TResult}"/></returns>
    [Pure]
    public static IEnumerable<T> Enumerate<T>(this T? nullable)
        where T : struct {
        return nullable.HasValue ? Enumerable.Repeat(nullable.Value, 1) : Enumerable.Empty<T>();
    }

    /// <summary>
    /// Invokes <paramref name="selector"/> against this <see cref="Nullable{T}"/>'s <see cref="Nullable{T}.Value"/> if this <see cref="Nullable{T}.HasValue"/>.
    /// </summary>
    /// <param name="nullable">this <see cref="Nullable{T}"/></param>
    /// <param name="selector">a <see cref="Func{TResult}"/> to be invoked with this <see cref="Nullable{T}.Value"/></param>
    /// <typeparam name="T">the <see cref="Nullable.GetUnderlyingType"/></typeparam>
    /// <typeparam name="T2">the <paramref name="selector"/> result type</typeparam>
    /// <returns>an <see cref="Optional{T}"/> containing the <paramref name="selector"/> result, if this <see cref="Nullable{T}.HasValue"/>; otherwise, <see cref="Optional.Empty{T}"/></returns>
    [Pure]
    public static Optional<T2> Select<T, T2>(this T? nullable, Func<T, T2> selector)
        where T : struct {
        return nullable.IsEmpty() ? Optional.Empty<T2>() : selector(nullable.Value);
    }
}