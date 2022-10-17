using System;
using System.Linq;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region First / Last

    #region First

    public static T First<T>(this ReadOnlySpan<T> span) => span.RequireNotEmpty()[0];

    public static T First<T, TExpected>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, TExpected> selector,  TExpected                                       expected, [CallerArgumentExpression("selector")] string? _selector = default) where TExpected : IEquatable<TExpected> => span.RequireFound(span.IndexWhere(selector, expected), _selector, expected);
    public static T First<T>(this            ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool>      predicate, [CallerArgumentExpression("predicate")] string? _predicate = default) => span.First(predicate, true, _predicate);

    public static T? FirstOrDefault<T>(this ReadOnlySpan<T> span)                 => span.GetOrDefault(0);
    public static T  FirstOrDefault<T>(this ReadOnlySpan<T> span, T defaultValue) => span.GetOrDefault(0, defaultValue);

    public static T? FirstOrDefault<T, TExpected>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, TExpected> selector, TExpected expected) where TExpected : IEquatable<TExpected>                 => span.GetOrDefault(span.IndexWhere(selector, expected));
    public static T  FirstOrDefault<T, TExpected>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, TExpected> selector, TExpected expected, T defaultValue) where TExpected : IEquatable<TExpected> => span.GetOrDefault(span.IndexWhere(selector, expected), defaultValue);
    public static T? FirstOrDefault<T>(this            ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool>      predicate)                 => span.FirstOrDefault(predicate, true);
    public static T  FirstOrDefault<T>(this            ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool>      predicate, T defaultValue) => span.FirstOrDefault(predicate, true, defaultValue);

    #endregion

    #region Last

    /// <summary>
    /// Returns the final entry in this <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <remarks>
    /// Analogous to <see cref="Enumerable.Last{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>.
    /// </remarks>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>the final entry in this <see cref="ReadOnlySpan{T}"/></returns>
    /// <inheritdoc cref="RequireNotEmpty{T}"/>
    public static T Last<T>(this ReadOnlySpan<T> span) => span.RequireNotEmpty()[^1];

    /// <summary>
    /// Returns the latter-most entry in this <see cref="ReadOnlySpan{T}"/> for whom <paramref name="selector"/> returns <paramref name="expected"/>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="selector">applied to each entry in this span</param>
    /// <param name="expected">the desired output of the <paramref name="selector"/></param>
    /// <param name="_selector">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <typeparam name="TExpected">the <paramref name="selector"/> output type</typeparam>
    /// <returns>the latter-most satisfactory <typeparamref name="T"/> element</returns>
    public static T Last<T, TExpected>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, TExpected> selector, TExpected expected, [CallerArgumentExpression("selector")] string? _selector = default) where TExpected : IEquatable<TExpected> => span.RequireFound(span.IndexWhere(selector, expected), _selector, expected);

    public static T Last<T>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool> predicate, [CallerArgumentExpression("predicate")] string? _predicate = default) => span.First(predicate, true, _predicate);

    public static T? LastOrDefault<T>(this ReadOnlySpan<T> span)                 => span.GetOrDefault(^1);
    public static T  LastOrDefault<T>(this ReadOnlySpan<T> span, T defaultValue) => span.GetOrDefault(^1, defaultValue);

    public static T? LastOrDefault<T, TExpected>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, TExpected> selector, TExpected expected) where TExpected : IEquatable<TExpected>                 => span.GetOrDefault(span.IndexWhere(selector, expected));
    public static T  LastOrDefault<T, TExpected>(this ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, TExpected> selector, TExpected expected, T defaultValue) where TExpected : IEquatable<TExpected> => span.GetOrDefault(span.IndexWhere(selector, expected), defaultValue);
    public static T? LastOrDefault<T>(this            ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool>      predicate)                 => span.LastOrDefault(predicate, true);
    public static T  LastOrDefault<T>(this            ReadOnlySpan<T> span, [RequireStaticDelegate] Func<T, bool>      predicate, T defaultValue) => span.LastOrDefault(predicate, true, defaultValue);

    #endregion

    /// <summary>
    /// Retrieves the (<see cref="First{T}(System.ReadOnlySpan{T})">First</see>, <see cref="Last{T}(System.ReadOnlySpan{T})">Last</see>) items from a <see cref="ReadOnlySpan{T}"/>.
    /// <br/>
    /// Requires a <see cref="ReadOnlySpan{T}.Length"/> of <b><i>at least</i></b> 2.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>(<see cref="First{T}(System.ReadOnlySpan{T})">First</see>, <see cref="Last{T}(System.ReadOnlySpan{T})">Last</see>)</returns>
    /// <exception cref="InvalidOperationException">if <see cref="ReadOnlySpan{T}.Length"/> &lt; 2</exception>
    public static (T first, T last) FirstAndLast<T>(this ReadOnlySpan<T> span) => (span.RequireLength(2)[0], span[^1]);

    /// <summary>
    /// Retrieves the everything <i>except</i> the <see cref="FirstAndLast{T}"/>.
    /// <br/>
    /// Requires a <see cref="ReadOnlySpan{T}.Length"/> of <b><i>at least</i></b> 3.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns><c><![CDATA[span[1..^2]]]></c></returns>
    /// <exception cref="InvalidOperationException">if <see cref="ReadOnlySpan{T}.Length"/> is &lt; 3</exception>
    public static ReadOnlySpan<T> Inner<T>(this ReadOnlySpan<T> span) => span.RequireLength(3)[1..^2];

    /// <summary>
    /// Retrieves the <see cref="FirstAndLast{T}"/> and sets the <c>out inner</c> parameter to <see cref="Inner{T}"/>.
    /// <br/>
    /// Requires a <see cref="ReadOnlySpan{T}.Length"/> of <b><i>at least</i></b> 3.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="inner">will hold the <see cref="Inner{T}"/> entries</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>(<see cref="First{T}(System.ReadOnlySpan{T})">First</see>, <see cref="Last{T}(System.ReadOnlySpan{T})">Last</see>)</returns>
    /// <exception cref="InvalidOperationException">if <see cref="ReadOnlySpan{T}.Length"/> is &lt; 3</exception>
    public static (T first, T last) FirstInnerLast<T>(this ReadOnlySpan<T> span, out ReadOnlySpan<T> inner) {
        inner = span.Inner();
        return (span[0], span[^1]);
    }

    #endregion
}