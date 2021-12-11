using System;
using System.Diagnostics.Contracts;

using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using System.Diagnostics.Contracts;
using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace BrandonUtils.Testing {
    public class Asserter<T> : MultipleAsserter<Asserter<T>, T> {
        public override void ResolveFunc<T1>(
            ActualValueDelegate<T1> actual,
            IResolveConstraint      constraint,
            Func<string>            message
        ) {
            var msg = message?.Invoke();
            if (msg.IsBlank()) {
                Assert.That(actual, constraint);
            }
            else {
                Assert.That(actual, constraint, msg);
            }
        }

        public override void ResolveAction(
            TestDelegate       action,
            IResolveConstraint constraint,
            Func<string>       message
        ) {
            if (message == null) {
                Assert.That(action, constraint);
            }
            else {
                Assert.That(action, constraint, message);
            }
        }

        protected override void OnFailure(string results) => Assert.Fail(results);

        public Asserter() { }
        public Asserter(T                      actual) : base(actual) { }
        public Asserter(ActualValueDelegate<T> actualValueDelegate) : base(actualValueDelegate) { }
    }

    [PublicAPI]
    public static class Asserter {
        [Pure]
        public static Asserter<T> Against<T>(T actual) {
            return new Asserter<T>(actual);
        }

        [Pure]
        public static Asserter<T> Against<T>(ActualValueDelegate<T> actualValueDelegate) {
            return new Asserter<T>(actualValueDelegate);
        }

        [Pure]
        public static Asserter<object> WithHeading(string? heading) => new Asserter<object>().WithHeading(heading);
    }
}