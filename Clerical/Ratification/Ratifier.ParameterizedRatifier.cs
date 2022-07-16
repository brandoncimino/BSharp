using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace FowlFever.Clerical.Ratification;

public static partial class Ratifier<T> {
    public record ParameterizedRatifier<TArg>(
        [RequireStaticDelegate]
        Func<T, TArg, bool> Predicate,
        [CallerArgumentExpression("Predicate")]
        string? Description = default
    ) : IRatifier<T, TArg>, IPredicateRatifier<(T target, TArg args)> {
        public string? Description { get; } = Description;

        bool IPredicateRatifier<(T target, TArg args)>._evaluate((T target, TArg args) target) {
            return Predicate(target.target, target.args);
        }
    }

    public static IRatifier<T, TArgs> Create<TArgs>(
        [RequireStaticDelegate]
        Func<T, TArgs, bool> predicate,
        [CallerArgumentExpression("predicate")]
        string? description = default
    ) {
        return new ParameterizedRatifier<TArgs>(predicate, description);
    }
}