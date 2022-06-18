using System;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FowlFever.Testing {
    /// <summary>
    /// Performs multiple assertions and returns all of the results.
    /// <p/>
    /// <b>NOTE:</b> This class will become <see cref="ObsoleteAttribute">Obsolete</see> once <a href="https://docs.nunit.org/articles/nunit/writing-tests/assertions/multiple-asserts.html">NUnit's Assert.Multiple()</a> is supported by Unity.
    /// <p/>
    ///
    /// TODO: Should this class be renamed to "Ass" and have semantics like "Ass.AllOf()" / "Ass.Any()"?
    ///     This would allow it to be combined with "Any" methods, e.g. "Ass.AnyOf()" / "Ass.Any()" - otherwise,
    ///     I would need a separate class named "AssertAny", with an equivalent "AssertAny.Of()"
    /// </summary>
    /// <remarks>
    /// Inspired by <a href="https://junit.org/junit5/docs/5.0.1/api/org/junit/jupiter/api/Assertions.html#assertAll-org.junit.jupiter.api.function.Executable...-">JUnit 5's <c>Assertions.assertAll()</c></a>,
    /// while using semantics similar to <a href="https://www.javadoc.io/doc/org.assertj/assertj-core/latest/org/assertj/core/api/Assertions.html#allOf(org.assertj.core.api.Condition...)">AssertJ's <c>Assertions.allOf()</c></a>.
    /// <p/>
    /// <b>NOTE:</b> This will catch <b>any</b> <see cref="Exception"/> throw by the <see cref="Action"/>s.
    /// This is unlike <a href="https://www.javadoc.io/doc/org.assertj/assertj-core/latest/org/assertj/core/api/AbstractAssert.html#satisfies(java.util.function.Consumer)">AssertJ's .satisfies()</a>,
    /// which will only catch <a href="https://docs.oracle.com/javase/8/docs/api/java/lang/AssertionError.html">Java's AssertionError</a>s (equivalent to C#'s <see cref="AssertionException"/>),
    /// because I think that's stupid.
    /// <p/>
    /// <b>NOTE 2:</b> I just checked, and thankfully <a href="https://junit.org/junit5/docs/5.0.1/api/org/junit/jupiter/api/Assertions.html#assertAll-java.lang.String-java.util.stream.Stream-">JUnit 5's <c>Assertions.assertAll()</c></a>
    /// catches any exception (thank goodness).
    /// <p/>
    /// <b>NOTE 3:</b> Maybe I will keep this, 'cus stupid <a href="https://docs.nunit.org/articles/nunit/writing-tests/assertions/multiple-asserts.html">NUnit Assert.Multiple()</a> doesn't handle non-<see cref="AssertionException"/>s!!
    /// </remarks>
    [Obsolete("Please use the Asserter class instead")]
    public static class AssertAll {
        /// <summary>
        /// <see cref="Action.Invoke"/>s each of the provided <see cref="Action"/>s, returning <b>all</b> of the failures.
        /// </summary>
        /// <param name="heading"></param>
        /// <param name="assertions"></param>
        /// <exception cref="AssertionException">if any of the <see cref="assertions"/> <see cref="Action"/>s throws an <see cref="Exception"/></exception>
        public static void Of(string heading, params Action[] assertions) {
            Asserter.WithHeading(heading)
                    .And(assertions)
                    .Invoke();
        }

        /**
         * <inheritdoc cref="Of(string,System.Action[])"/>
         */
        public static void Of(params Action[] assertions) {
            Of(null, assertions);
        }

        /// <summary>
        /// <see cref="Assert"/>s multiple <see cref="Constraint"/>s against <see cref="actual"/>, returning <b>all</b> of the results.
        /// </summary>
        /// <remarks>
        /// Constructs an <see cref="Action"/> for each <see cref="Constraint"/> and passes it to <see cref="Of(Action[])"/>
        /// </remarks>
        /// <param name="actual">the actual value being tested</param>
        /// <param name="assertions">an array of <see cref="Constraint"/>s to be applied as <b>individual <see cref="Assert"/>ions</b></param>
        /// <typeparam name="T">the <see cref="Type"/> of the `<paramref name="actual"/>` value</typeparam>
        public static void Of<T>(T actual, params Constraint[] assertions) {
            Asserter.Against(actual)
                    .And(assertions)
                    .Invoke();
        }

        /**
         * <param name="heading">a string displayed before the list of failures</param>
         * <inheritdoc cref ="Of{T}(T,Constraint[])"/>
         */
        [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
        public static void Of<T>(string heading, T actual, params Constraint[] assertions) {
            Asserter.WithHeading<T>(heading)
                    .Against(actual)
                    .And(assertions)
                    .Invoke();
        }
    }
}