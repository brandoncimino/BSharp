using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional;

public static partial class Failables {
    #region Functions

    /// <summary>
    /// Attempts to <see cref="Func{TResult}.Invoke"/> <see cref="functionThatMightFail"/>, returning a <see cref="FailableFunc{TValue}"/>
    /// that contains either the successful result or the <see cref="IFailableFunc{TValue}.Excuse"/> for failure.
    /// </summary>
    /// <param name="functionThatMightFail">the <see cref="Func{TResult}"/> being attempted</param>
    /// <param name="description">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <example>
    /// TODO: Add an example, but I'm tired right now and when I started writing one instead made <c>DayOfWeekExtensions.IsSchoolNight"</c>
    /// </example>
    public static FailableFunc<T> Try<T>(
        [InstantHandle]
        this Func<T> functionThatMightFail,
        [CallerArgumentExpression("functionThatMightFail")]
        string? description = default
    ) {
        return FailableFunc<T>.Invoke(functionThatMightFail, description);
    }

    /// <inheritdoc cref="FailableFunc{TValue}"/>
    public static FailableFunc<TOut> Try<TIn, TOut>(
        this Func<TIn, TOut> functionThatMightFail,
        TIn                  input,
        [CallerArgumentExpression("functionThatMightFail")]
        string? description = default
    ) => FailableFunc<TOut>.Invoke(() => functionThatMightFail.Invoke(input), description);

    /// <inheritdoc cref="FailableFunc{TValue}"/>
    public static FailableFunc<TOut> Try<A, B, TOut>(
        this Func<A, B, TOut> functionThatMightFail,
        A                     a,
        B                     b,
        [CallerArgumentExpression("functionThatMightFail")]
        string? description = default
    ) => FailableFunc<TOut>.Invoke(() => functionThatMightFail.Invoke(a, b), description);

    /// <inheritdoc cref="FailableFunc{TValue}"/>
    public static FailableFunc<TOut> Try<A, B, C, TOut>(
        this Func<A, B, C, TOut> functionThatMightFail,
        A                        a,
        B                        b,
        C                        c,
        [CallerArgumentExpression("functionThatMightFail")]
        string? description = default
    ) => FailableFunc<TOut>.Invoke(() => functionThatMightFail.Invoke(a, b, c), description);

    #region TryEach

    public static IEnumerable<FailableFunc<TOut>> TrySelect<TIn, TOut>([InstantHandle] this IEnumerable<TIn> enumerable, [InstantHandle] Func<TIn, TOut>      selector,          [CallerArgumentExpression("selector")]          string? expression = default) => enumerable.Select(it => selector.Try(it, expression));
    public static IEnumerable<FailableFunc<TOut>> TrySelect<TIn, TOut>([InstantHandle] this IEnumerable<TIn> enumerable, [InstantHandle] Func<TIn, int, TOut> selectorWithIndex, [CallerArgumentExpression("selectorWithIndex")] string? expression = default) => enumerable.Select((it, i) => selectorWithIndex.Try(it, i, expression));

    #endregion

    #endregion
}