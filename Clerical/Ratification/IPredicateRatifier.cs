using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Optional;

namespace FowlFever.Clerical.Ratification;

internal interface IPredicateRatifier<in T> : IRatifier<T>
    where T : notnull {
    protected internal bool _evaluate(T? target);

    IFailable IRatifier<T>._TryRatify(T? target, string? targetName, string? requiredBy) {
        Exception? err       = default;
        bool       isSuccess = default;
        try {
            isSuccess = _evaluate(target);
        }
        catch (Exception e) {
            err = e;
        }

        if (isSuccess == false || err != null) {
            return Failable.Failure(
                new RejectionException(target, parameterName: targetName, rejectedBy: requiredBy, reason: Description, innerException: err)
            );
        }

        return Failable.Success(Description);
    }
}