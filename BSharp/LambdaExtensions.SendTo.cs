using System;

namespace FowlFever.BSharp;

public static partial class LambdaExtensions {
    #region SendTo

    public static void SendTo<T>(this                T            arg,  Action<T>              action) => action(arg);
    public static TOut SendTo<T, TOut>(this          T            arg,  Func<T, TOut>          func)   => func(arg);
    public static void SendTo<A, B>(this             (A, B)       args, Action<A, B>           action) => action.Invoke(args);
    public static void SendTo<A, B, C>(this          (A, B, C)    args, Action<A, B, C>        action) => action.Invoke(args);
    public static void SendTo<A, B, C, D>(this       (A, B, C, D) args, Action<A, B, C, D>     action) => action.Invoke(args);
    public static TOut SendTo<A, B, TOut>(this       (A, B)       args, Func<A, B, TOut>       func)   => func.Invoke(args);
    public static TOut SendTo<A, B, C, TOut>(this    (A, B, C)    args, Func<A, B, C, TOut>    func)   => func.Invoke(args);
    public static TOut SendTo<A, B, C, D, TOut>(this (A, B, C, D) args, Func<A, B, C, D, TOut> func)   => func.Invoke(args);

    #endregion
}