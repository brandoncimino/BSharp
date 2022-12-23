using System;

namespace FowlFever.BSharp;

public static partial class LambdaExtensions {
    #region Parameter "Reduction" (pretty sure "binding" is the correct word)

    #region Actions

    [Pure] public static Action    Reduce<T>(this       Action<T>       action, T arg)    => () => action(arg);
    [Pure] public static Action    Reduce<A, B>(this    Action<A, B>    action, A a, B b) => () => action(a, b);
    [Pure] public static Action<A> Reduce<A, B>(this    Action<A, B>    action, B b)           => a => action(a,  b);
    [Pure] public static Action    Reduce<A, B, C>(this Action<A, B, C> action, A a, B b, C c) => () => action(a, b, c);
    [Pure] public static Action<A> Reduce<A, B, C>(this Action<A, B, C> action, B b, C c) => a => action(a, b, c);

    [Pure]
    public static Action Reduce<A, B, C, D>(
        this Action<A, B, C, D> action,
        A                       a,
        B                       b,
        C                       c,
        D                       d
    ) => () => action(a, b, c, d);

    [Pure] public static Action<A> Reduce<A, B, C, D>(this Action<A, B, C, D> action, B b, C c, D d) => a => action(a, b, c, d);

    #endregion

    #region Functions

    [Pure] public static Func<TOut>    Reduce<A, TOut>(this       Func<A, TOut>       func, A a)      => () => func(a);
    [Pure] public static Func<TOut>    Reduce<A, B, TOut>(this    Func<A, B, TOut>    func, A a, B b) => () => func(a, b);
    [Pure] public static Func<A, TOut> Reduce<A, B, TOut>(this    Func<A, B, TOut>    func, B b)           => a => func(a,  b);
    [Pure] public static Func<TOut>    Reduce<A, B, C, TOut>(this Func<A, B, C, TOut> func, A a, B b, C c) => () => func(a, b, c);
    [Pure] public static Func<A, TOut> Reduce<A, B, C, TOut>(this Func<A, B, C, TOut> func, B b, C c) => a => func(a, b, c);

    #endregion

    #region Discard (i.e. Func -> Action)

    [Pure] public static Action             Discard<TOut>(this             Func<TOut>             func) => () => func.Invoke();
    [Pure] public static Action<A>          Discard<A, TOut>(this          Func<A, TOut>          func) => a => func.Invoke(a);
    [Pure] public static Action<A, B>       Discard<A, B, TOut>(this       Func<A, B, TOut>       func) => (a, b) => func.Invoke(a,       b);
    [Pure] public static Action<A, B, C>    Discard<A, B, C, TOut>(this    Func<A, B, C, TOut>    func) => (a, b, c) => func.Invoke(a,    b, c);
    [Pure] public static Action<A, B, C, D> Discard<A, B, C, D, TOut>(this Func<A, B, C, D, TOut> func) => (a, b, c, d) => func.Invoke(a, b, c, d);

    #endregion

    #region Action -> Func<Void> (📎 System.Void can't actually be used, so we use System.ValueTuple instead)

    private static readonly ValueTuple Nothing = default;

    public static Func<ValueTuple> ToFunction(this Action action) => () => {
        action();
        return Nothing;
    };

    public static Func<A, ValueTuple> ToFunction<A>(this Action<A> action) => a => {
        action(a);
        return Nothing;
    };

    public static Func<A, B, ValueTuple> ToFunction<A, B>(this Action<A, B> action) => (a, b) => {
        action(a, b);
        return Nothing;
    };

    public static Func<A, B, C, ValueTuple> ToFunction<A, B, C>(this Action<A, B, C> action) => (a, b, c) => {
        action(a, b, c);
        return Nothing;
    };

    #endregion

    #region ToArgFunction

    public static Func<ValueTuple, ValueTuple> ToArgFunction(this Action action) => _ => {
        action();
        return Nothing;
    };

    public static Func<ValueTuple<A>, ValueTuple> ToArgFunction<A>(this Action<A> action) => args => {
        action(args.Item1);
        return Nothing;
    };

    public static Func<(A a, B b), ValueTuple> ToArgFunction<A, B>(this Action<A, B> action) => tuple => {
        action(tuple.a, tuple.b);
        return Nothing;
    };

    public static Func<(A a, B b, C c), ValueTuple> ToArgFunction<A, B, C>(this Action<A, B, C> action) =>
        tuple => {
            action.Invoke(tuple.a, tuple.b, tuple.c);
            return Nothing;
        };

    public static Func<ValueTuple, OUT> ToArgFunction<OUT>(this Func<OUT> func) => _ => func();

    #endregion

    #region Condensation: fn(a, b, c) -> fn((abc))

    [Pure] public static Action<(A, B)>              Condense<A, B>(this                Action<A, B>              action) => action.Invoke;
    [Pure] public static Action<(A, B, C)>           Condense<A, B, C>(this             Action<A, B, C>           action) => action.Invoke;
    [Pure] public static Action<(A, B, C, D)>        Condense<A, B, C, D>(this          Action<A, B, C, D>        action) => action.Invoke;
    [Pure] public static Func<(A, B), TResult>       Condense<A, B, TResult>(this       Func<A, B, TResult>       func)   => func.Invoke;
    [Pure] public static Func<(A, B, C), TResult>    Condense<A, B, C, TResult>(this    Func<A, B, C, TResult>    func)   => func.Invoke;
    [Pure] public static Func<(A, B, C, D), TResult> Condense<A, B, C, D, TResult>(this Func<A, B, C, D, TResult> func)   => func.Invoke;

    #endregion

    #region Expansion: fn((abc)) -> fn(a, b, c)

    [Pure] public static Action<A, B>              Expand<A, B>(this                Action<(A, B)>              action) => action.Invoke;
    [Pure] public static Action<A, B, C>           Expand<A, B, C>(this             Action<(A, B, C)>           action) => action.Invoke;
    [Pure] public static Action<A, B, C, D>        Expand<A, B, C, D>(this          Action<(A, B, C, D)>        action) => action.Invoke;
    [Pure] public static Func<A, B, TResult>       Expand<A, B, TResult>(this       Func<(A, B), TResult>       func)   => func.Invoke;
    [Pure] public static Func<A, B, C, TResult>    Expand<A, B, C, TResult>(this    Func<(A, B, C), TResult>    func)   => func.Invoke;
    [Pure] public static Func<A, B, C, D, TResult> Expand<A, B, C, D, TResult>(this Func<(A, B, C, D), TResult> func)   => func.Invoke;

    #endregion

    #endregion
}