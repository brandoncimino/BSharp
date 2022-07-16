using FowlFever.BSharp.Exceptions;

namespace FowlFever.Clerical.Ratification;

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

    public static IRatifier<T> Negate(IRatifier<T> og) {
        if (og is NegativeRatifier neg) {
            return neg.PositiveRatifier;
        }

        return new NegativeRatifier(og);
    }
}