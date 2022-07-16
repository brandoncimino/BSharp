using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional;

public partial class Failables {
    #region Actions

    /// <summary>
    /// Attempts to <see cref="Action.Invoke"/> <paramref name="failableAction"/>, returning a <see cref="Failable"/>
    /// that (might) contain the <see cref="IFailableFunc{TValue}.Excuse"/> for failure.
    /// </summary>
    /// <param name="failableAction">the <see cref="Action"/> being executed</param>
    /// <param name="description">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="ignoredExceptionTypes"><see cref="Exception"/> types that won't be considered a <see cref="Failable.Failed"/> execution</param>
    /// <returns>a <see cref="Failable"/> containing information about execution of the <paramref name="failableAction"/></returns>
    public static Failable Try(
        [InstantHandle]
        this Action failableAction,
        [CallerArgumentExpression("failableAction")]
        string? description = default,
        params Type[] ignoredExceptionTypes
    ) {
        return Failable.Invoke(failableAction, ignoredExceptionTypes, description);
    }

    /// <inheritdoc cref="Try(System.Action,string?,System.Type[])"/>
    public static Failable Try<T>(
        [InstantHandle]
        this Action<T> failableAction,
        T arg,
        [CallerArgumentExpression("failableAction")]
        string? description = default,
        params Type[] ignoredExceptionTypes
    ) {
        return Failable.Invoke(failableAction.Reduce(arg), ignoredExceptionTypes, description);
    }

    /// <inheritdoc cref="Try(System.Action,string?,System.Type[])"/>
    public static Failable Try<A, B>(
        [InstantHandle] [RequireStaticDelegate]
        this Action<A, B> failableAction,
        A a,
        B b,
        [CallerArgumentExpression("failableAction")]
        string? description = default,
        params Type[] ignoredExceptionTypes
    ) {
        return Failable.Invoke(failableAction.Reduce(a, b), ignoredExceptionTypes, description);
    }

    /// <inheritdoc cref="Try(System.Action,string?,System.Type[])"/>
    public static Failable Try<A, B, C>(
        [InstantHandle]
        this Action<A, B, C> failableAction,
        A a,
        B b,
        C c,
        [CallerArgumentExpression("failableAction")]
        string? description = default,
        params Type[] ignoredExceptionTypes
    ) {
        return Failable.Invoke(failableAction.Reduce(a, b, c), ignoredExceptionTypes, description);
    }

    /// <inheritdoc cref="Try(System.Action,string?,System.Type[])"/>
    public static Failable Try<A, B, C, D>(
        [InstantHandle]
        this Action<A, B, C, D> failableAction,
        A a,
        B b,
        C c,
        D e,
        [CallerArgumentExpression("failableAction")]
        string? expression = default,
        params Type[] ignoredExceptionTypes
    ) => Failable.Invoke(failableAction.Reduce(a, b, c, e), ignoredExceptionTypes, expression);

    #region TryEach

    public static IEnumerable<Failable> TryEach<T>([InstantHandle] this IEnumerable<T> enumerable, [InstantHandle] Action<T>      action,          [CallerArgumentExpression("action")]          string? expression = default) => enumerable.Select(it => Try(action, it, expression));
    public static IEnumerable<Failable> TryEach<T>([InstantHandle] this IEnumerable<T> enumerable, [InstantHandle] Action<T, int> actionWithIndex, [CallerArgumentExpression("actionWithIndex")] string? expression = default) => enumerable.Select((it, i) => actionWithIndex.Try(it, i, expression));

    #endregion

    #region TryAll

    public static IEnumerable<Failable> TryAll([InstantHandle] this    IEnumerable<Action>    actions)          => actions.Select(it => it.Try());
    public static IEnumerable<Failable> TryAll<T>([InstantHandle] this IEnumerable<Action<T>> actions, T input) => actions.Select(it => it.Try(input));

    #endregion

    #endregion
}