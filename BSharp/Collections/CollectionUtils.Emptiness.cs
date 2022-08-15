using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using FowlFever.BSharp.Optional;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

public static partial class CollectionUtils {
    #region Emptiness

    /// <summary>
    /// Negation of <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>.
    /// </summary>
    /// <param name="source"><inheritdoc cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/></param>
    /// <typeparam name="TSource"><inheritdoc cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/></typeparam>
    /// <returns>the inverse of <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/></returns>
    /// <remarks>
    /// Specifically delegates to <see cref="string.Length"/> if possible.
    /// TODO: Experiment on whether it makes sense to have a special version of <see cref="IsEmpty{TSource}(System.Collections.Generic.IEnumerable{TSource}?)"/> as an <see cref="IOptional{T}"/> extension method, which would return the inverse of <see cref="IOptional{T}.HasValue"/>. The problem is that this method causes ambiguity with the <see cref="IEnumerable{T}"/> version of <see cref="IOptional{T}"/>
    /// </remarks>
    /// <seealso cref="None{T}(System.Collections.Generic.IEnumerable{T})"/>
    [Pure]
    public static bool IsEmpty<TSource>([NotNullWhen(false)] [InstantHandle] this IEnumerable<TSource>? source) {
        return source switch {
            null         => true,
            string s     => s.Length  == 0,
            TSource[] ar => ar.Length == 0,
            _            => !source.Any()
        };
    }

    /// <summary>
    /// A less hideous alias for <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>.
    /// <i>(<inheritdoc cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>)</i>
    /// </summary>
    /// <param name="source"><inheritdoc cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/></param>
    /// <typeparam name="TSource"><inheritdoc cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/></typeparam>
    [Pure]
    public static bool IsNotEmpty<TSource>([NotNullWhen(true)] [InstantHandle] this IEnumerable<TSource>? source) {
        return source.IsEmpty() == false;
    }

    /// <summary>
    /// Negation of <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>.
    /// </summary>
    /// <inheritdoc cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>
    /// <returns>true if the <paramref name="source"/> contains 0 elements.</returns>
    public static bool None<T>(this IEnumerable<T> source) {
        return source.IsEmpty();
    }

    /// <summary>
    /// Negation of <see cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource},Func{TSource,bool})"/>.
    /// </summary>
    /// <inheritdoc cref="Enumerable.Any{TSource}(System.Collections.Generic.IEnumerable{TSource}, Func{TSource,bool})"/>
    /// <returns>true if <b>none</b> of the items in <paramref name="source"/> satisfy <paramref name="predicate"/>.</returns>
    [Pure]
    public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
        return !source.Any(predicate);
    }

    #endregion
}