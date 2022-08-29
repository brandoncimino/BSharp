using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

public static partial class CollectionUtils {
    #region Leftovers

    /// <summary>
    /// <see cref="Enumerable.Take{TSource}(System.Collections.Generic.IEnumerable{TSource},int)"/>s the first <paramref name="count"/> items and <see cref="Enumerable.Skip{TSource}"/>s to the rest into
    /// separate <see cref="IEnumerable{T}"/>s.
    /// </summary>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <param name="count"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Tests prove that this doesn't not enumerate multiple times")]
    public static (IEnumerable<T> taken, IEnumerable<T> leftovers) TakeLeftovers<T>([NoEnumeration] this IEnumerable<T> source, [NonNegativeValue] int count) {
        return (source.Take(count), source.Skip(count));
    }

    #region First

    /// <summary>
    /// Separates the <see cref="Enumerable.First{TSource}(System.Collections.Generic.IEnumerable{TSource})"/> entry from the rest of the <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <typeparam name="T">the element type</typeparam>
    /// <returns>(first, everything else)</returns>
    /// <seealso cref="TakeLeftovers{T}"/>
    /// <seealso cref="TakeLast{T}(System.Collections.Generic.IEnumerable{T})"/>
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = $"See {nameof(TakeLeftovers)}")]
    public static (T first, IEnumerable<T> rest) TakeFirst<T>(this IEnumerable<T> source) {
        return (source.First(), source.Skip(1));
    }

    /// <summary>
    /// Performs a different <see cref="Enumerable.Select{TSource,TResult}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,int,TResult})"/> operation on the
    /// <see cref="Enumerable.First{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="firstSelector"></param>
    /// <param name="restSelector"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TRest"></typeparam>
    /// <returns></returns>
    public static (TFirst first, IEnumerable<TRest> rest) SelectFirst<T, TFirst, TRest>(
        this IEnumerable<T> source,
        Func<T, TFirst>     firstSelector,
        Func<T, TRest>      restSelector
    ) {
        return source.TakeFirst().SelectEach(firstSelector, restSelector);
    }

    private static IEnumerable<T2> SelectIndexOffset<T, T2>(this IEnumerable<T> source, Func<T, int, T2> selector, int indexOffset) {
        var i = 0;
        foreach (var it in source) {
            yield return selector(it, i + indexOffset);
            i += 1;
        }
    }

    public static (TFirst first, IEnumerable<TRest> rest) SelectFirst<T, TFirst, TRest>(
        this IEnumerable<T>  source,
        Func<T, int, TFirst> firstSelector,
        Func<T, int, TRest>  restSelector
    ) {
        var (first, rest) = source.TakeFirst();
        return (firstSelector(first, 0), rest.SelectIndexOffset(restSelector, 1));
    }

    #endregion

    #region Last

    /// <summary>
    /// Separates the <see cref="Enumerable.Last{TSource}(System.Collections.Generic.IEnumerable{TSource})"/> entry from the rest of the <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IEnumerable{T}"/></param>
    /// <typeparam name="T">the element type</typeparam>
    /// <returns>(last, everything else)</returns>
    [LinqTunnel]
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = $"See {nameof(TakeLeftovers)}")]
    public static (T last, IEnumerable<T> rest) TakeLast<T>(this IEnumerable<T> source) {
        return (source.Last(), source.SkipLast(1));
    }

    [LinqTunnel]
    public static (TLast last, IEnumerable<TRest> rest) SelectLast<T, TLast, TRest>(this IEnumerable<T> source, Func<T, TLast> last, Func<T, TRest> rest) {
        return source.TakeLast().SelectEach(last, rest);
    }

    #endregion

    #region Bookends

    public static (T first, IEnumerable<T> inner, T last) TakeBookends<T>(this IEnumerable<T> source) {
        var (first, afterFirst) = source.TakeFirst();
        var (last, inner)       = afterFirst.TakeLast();
        return (first, inner, last);
    }

    public static (TFirst2 first, IEnumerable<TInner2> inner, TLast2 last) SelectBookends<TFirst, TInner, TLast, TFirst2, TInner2, TLast2>(
        this (TFirst first, IEnumerable<TInner> inner, TLast last) source,
        Func<TFirst, TFirst2>                                      first,
        Func<TInner, TInner2>                                      inner,
        Func<TLast, TLast2>                                        last
    ) {
        return (first(source.first), source.inner.Select(inner), last(source.last));
    }

    public static (TFirst first, IEnumerable<TInner> inner, TLast last) SelectBookends<T, TFirst, TInner, TLast>(
        this IEnumerable<T> source,
        Func<T, TFirst>     first,
        Func<T, TInner>     inner,
        Func<T, TLast>      last
    ) {
        return source.TakeBookends().SelectBookends(first, inner, last);
    }

    #endregion

    #endregion

    /// <summary>
    /// Executes <paramref name="firstAction"/> against <c>first</c>, then <paramref name="restAction"/> against each entry of <c>rest</c>.
    /// </summary>
    /// <remarks>
    /// Intended to be chained after a call to <see cref="CollectionUtils.TakeFirst{T}"/>.
    /// </remarks>
    /// <param name="source"></param>
    /// <param name="firstAction"></param>
    /// <param name="restAction"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public static void Each<T, T2>(this (T first, IEnumerable<T2> rest) source, Action<T> firstAction, Action<T2> restAction) {
        firstAction(source.first);
        foreach (var it in source.rest) {
            restAction(it);
        }
    }

    public static (A2 first, IEnumerable<B2> rest) SelectEach<A, B, A2, B2>(this (A first, IEnumerable<B> rest) source, Func<A, A2> firstSelector, Func<B, B2> restSelector) {
        var f2 = firstSelector(source.first);
        var r2 = source.rest.Select(restSelector);
        return (f2, r2);
    }

    public static void Each<A, B>(this (IEnumerable<A> A, IEnumerable<B> B) source, Action<A> aAction, Action<B> bAction) {
        foreach (var a in source.A) {
            aAction(a);
        }

        foreach (var b in source.B) {
            bAction(b);
        }
    }

    public static (IEnumerable<A2> A, IEnumerable<B2> B) SelectEach<A, B, A2, B2>(this (IEnumerable<A> A, IEnumerable<B> B) source, Func<A, A2> aSelector, Func<B, B2> bSelector) {
        var a2 = source.A.Select(aSelector);
        var b2 = source.B.Select(bSelector);
        return (a2, b2);
    }
}