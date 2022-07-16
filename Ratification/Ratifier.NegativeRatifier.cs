using FowlFever.BSharp.Exceptions;

namespace Ratified;

public static partial class Ratifier<T> {
    private record NegativeRatifier : IPredicateRatifier<T> {
        public string       Description      { get; }
        public IRatifier<T> PositiveRatifier { get; }

        public NegativeRatifier(IRatifier<T> positiveRatifier) {
            if (positiveRatifier is NegativeRatifier) {
                throw Reject.BadType(positiveRatifier, "Not-not is silly!");
            }

            PositiveRatifier = positiveRatifier;
            Description      = $"NOT {positiveRatifier.Description}";
        }

        bool IPredicateRatifier<T>._evaluate(T? target) => PositiveRatifier.TryRatify(target).Failed;
    }

    /// <param name="ratifier">the <see cref="IRatifier{T}"/> that will be negated</param>
    /// <typeparam name="T">the type being ratified</typeparam>
    /// <returns>an <see cref="IRatifier{T}"/> that succeeds where this <see cref="IRatifier{T}"/> has failed</returns>
    public static IRatifier<T> Negate(IRatifier<T> ratifier) {
        if (ratifier is NegativeRatifier neg) {
            return neg.PositiveRatifier;
        }

        return new NegativeRatifier(ratifier);
    }
}