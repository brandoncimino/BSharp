using BSharp.Tests.Collections;

using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings;
using FowlFever.Testing;

namespace BSharp.Tests {
    public static class MultipleAsserterExtensions {
        public static TSelf AndComparingFallbacks<TSelf, TActual, TFallback>(this MultipleAsserter<TSelf, TActual> asserter, Fallback<TFallback> actual, Fallback<TFallback> expected) where TSelf : MultipleAsserter<TSelf, TActual>, new() {
            return asserter.And(ComparingFallbacks(actual, expected, asserter.PrettificationSettings));
        }

        private static IMultipleAsserter ComparingFallbacks<T>(Fallback<T> actual, Fallback<T> expected, PrettificationSettings? asserterPrettificationSettings = default) {
            return Asserter.Against(actual)
                           .WithHeading($"Comparing {nameof(Fallback<T>)}s")
                           .WithPrettificationSettings(asserterPrettificationSettings)
                           .And(
                               Is.EqualTo(expected)
                                 .Using(new FallbackComparer()),
                               () => typeof(FallbackComparer).Prettify(asserterPrettificationSettings)
                           );
        }
    }
}