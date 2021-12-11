using System;

using FowlFever.BSharp.Strings;

namespace FowlFever.BSharp.Optional {
    /// <summary>
    /// Static methods for building <see cref="Exception"/>s used by <see cref="IFailable{TExcuse}"/> implementations.
    /// </summary>
    internal static class FailableException {
        private static string DidNotFailMessage(IFailable failable) {
            return $"Unable to retrieve the {nameof(failable.Excuse)} from the {failable.GetType().Name} because {nameof(failable.Failed)} == {failable.Failed}!";
        }

        internal static InvalidOperationException DidNotFailException(IFailable failable) {
            return new InvalidOperationException(DidNotFailMessage(failable));
        }

        internal static InvalidOperationException DidNotFailException<TValue>(IFailableFunc<TValue> failableFunc, Optional<TValue> actualValue) {
            var msg = DidNotFailMessage(failableFunc);

            if (actualValue.HasValue) {
                msg += $"({nameof(actualValue)}: {actualValue})";
            }

            throw new InvalidOperationException(msg);
        }


        internal static InvalidOperationException FailedException<TValue>(IFailableFunc<TValue> failableFunc, Optional<Exception> actualExcuse) {
            return new InvalidOperationException($"Unable to retrieve the {typeof(TValue).Prettify()} {nameof(failableFunc.Value)} from the {failableFunc.GetType().Prettify()} because it failed!", actualExcuse.OrElse(default));
        }
    }
}