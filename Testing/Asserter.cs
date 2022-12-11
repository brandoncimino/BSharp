using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FowlFever.Testing {
    public class Asserter<T> : MultipleAsserter<Asserter<T>, T> {
        internal override void ResolveFunc<T1>(
            Func<T1>           actual,
            IResolveConstraint constraint,
            Func<string?>?     message
        ) {
            var msg = message?.Invoke();
            var del = new ActualValueDelegate<T1>(actual);
            if (msg.IsBlank()) {
                Assert.That(del, constraint);
            }
            else {
                Assert.That(del, constraint, msg);
            }
        }

        protected override void ResolveAction(
            Action             action,
            IResolveConstraint constraint,
            Func<string?>?     message
        ) {
            var del = new TestDelegate(action);
            if (message == null) {
                Assert.That(del, constraint);
            }
            else {
                Assert.That(del, constraint, message);
            }
        }

        protected override void ResolveActual<T1>(
            T1                 actual,
            IResolveConstraint constraint,
            Func<string?>?     message
        ) {
            if (message == null) {
                Assert.That(actual, constraint);
            }
            else {
                Assert.That(actual, constraint, message);
            }
        }

        protected override void OnFailure(string results) => Assert.Fail(results);

        public Asserter() { }
        public Asserter(T       actual,              [CallerArgumentExpression("actual")]              string? actualValueAlias = default) : base(actual, actualValueAlias) { }
        public Asserter(Func<T> actualValueDelegate, [CallerArgumentExpression("actualValueDelegate")] string? actualValueAlias = default) : base(actualValueDelegate, actualValueAlias) { }
    }

    [PublicAPI]
    public static class Asserter {
        internal static Asserter<TNew> Against<TSelf, TOld, TNew>(
            MultipleAsserter<TSelf, TOld> parent,
            Func<TOld?, TNew>             transformation
        )
            where TSelf : MultipleAsserter<TSelf, TOld>, new() {
            return new Asserter<TNew>(() => transformation(parent.Actual.Value));
        }

        [Pure]
        public static Asserter<T> Against<T>(T actual, [CallerArgumentExpression("actual")] string? actualValueAlias = default) {
            return new Asserter<T>(actual, actualValueAlias);
        }

        [Pure]
        public static Asserter<T> Against<T>(Func<T> actualValueDelegate, [CallerArgumentExpression("actualValueDelegate")] string? actualValueAlias = default) {
            return new Asserter<T>(actualValueDelegate, actualValueAlias);
        }

        /// <summary>
        /// Creates a new <see cref="Asserter{T}"/> without an <see cref="MultipleAsserter{TSelf,TActual}.Actual"/> value.
        /// </summary>
        /// <param name="heading">an explicit <see cref="MultipleAsserter{TSelf,TActual}.Heading"/>. Falls back to <paramref name="_caller"/></param>
        /// <param name="_caller">see <see cref="CallerMemberNameAttribute"/></param>
        /// <typeparam name="T">the type of the <see cref="MultipleAsserter{TSelf,TActual}.Actual"/> value, if we were to have one</typeparam>
        /// <returns>a new <see cref="Asserter{T}"/></returns>
        [Pure]
        public static Asserter<T> WithHeading<T>(string? heading = default, [CallerMemberName] string? _caller = default) {
            return new Asserter<T>().WithHeading(heading.IfBlank(_caller));
        }

        /// <inheritdoc cref="WithHeading{T}"/>
        [Pure]
        public static Asserter<ValueTuple> WithHeading(
            string?                    heading   = default,
            [CallerMemberName] string? _caller   = default,
            [CallerFilePath]   string? _filePath = default,
            [CallerLineNumber] int?    _lineNo   = default
        ) {
            return WithHeading<ValueTuple>(heading, _caller);
        }

        /// <summary>
        /// Shorthand that bypasses the need to call <see cref="IMultipleAsserter.Invoke"/> when you only have a single <see cref="IConstraint"/>.
        /// </summary>
        /// <param name="actual">the value being tested</param>
        /// <param name="constraint">an NUnit constraint</param>
        /// <param name="_actual">see <see cref="CallerArgumentExpressionAttribute"/></param>
        /// <param name="_constraint">see <see cref="CallerArgumentExpressionAttribute"/></param>
        /// <typeparam name="T">the type of <paramref name="actual"/></typeparam>
        public static void That<T>(
            T?                                           actual,
            IConstraint                                  constraint,
            [CallerArgumentExpression("actual")] string? _actual = default,
            [CallerArgumentExpression("constraint")]
            string? _constraint = default
        ) {
            Against(actual, _actual).And(constraint).Invoke();
        }

        #region Fail

        /// <summary>
        /// Throws an <see cref="AssertionException"/> that indicates that we wanted an <see cref="Exception"/> to be thrown, but none was.
        /// </summary>
        /// <param name="caller">see <see cref="CallerMemberNameAttribute"/></param>
        /// <exception cref="AssertionException"><i>always</i> thrown</exception>
        [DoesNotReturn]
        public static Exception FailBecauseNoException([CallerMemberName] string caller = "Test") {
            Assert.Fail($"{caller} failed because no exception was thrown!");
            throw Reject.Unreachable();
        }

        #endregion
    }
}