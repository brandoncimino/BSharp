using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace Ratified;

public static partial class Ratifier<T> {
    internal delegate bool Evaluator<in TDel, in TArg>(TDel delgato, T? target, TArg args);

    private sealed record DelegateRatifier<TDel, TArg>([RequireStaticDelegate] TDel Del, TArg Arg, [RequireStaticDelegate] Evaluator<TDel, TArg> Evaluator, string? Description) : IPredicateRatifier<T>
        where TDel : Delegate {
        bool IPredicateRatifier<T>._evaluate(T? actual) {
            return Evaluator(Del, actual, Arg);
        }
    }

    internal static IRatifier<T> Create<TDel, TArg>(
        [RequireStaticDelegate]
        TDel delgato,
        TArg args,
        [RequireStaticDelegate]
        Evaluator<TDel, TArg> evaluator,
        [CallerArgumentExpression("delgato")]
        string? description = default
    )
        where TDel : Delegate {
        return new DelegateRatifier<TDel, TArg>(delgato, args, evaluator, description);
    }

    public static IRatifier<T> Create(
        [RequireStaticDelegate]
        Func<T?, bool> predicate,
        [CallerArgumentExpression("predicate")]
        string? description = default
    ) {
        return Create(predicate, default(ValueTuple), static (delgato, target, _) => delgato(target), description);
    }

    public static IRatifier<T> Create<TArgs>(
        [RequireStaticDelegate]
        Func<T?, TArgs, bool> predicate,
        TArgs arguments,
        [CallerArgumentExpression("predicate")]
        string? description = default
    ) {
        return Create(predicate, arguments, static (delgato, target, args) => delgato(target, args), description);
    }
}