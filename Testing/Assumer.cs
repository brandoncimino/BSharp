using System;

using FowlFever.BSharp.Strings;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FowlFever.Testing {
    public class Assumer<T> : MultipleAsserter<Assumer<T>, T> {
        public Assumer() { }
        public Assumer(T actual) : base(actual) { }

        protected override void OnFailure(string results) => Assert.Inconclusive(results);

        public override void ResolveFunc<T1>(
            ActualValueDelegate<T1> actual,
            IResolveConstraint      constraint,
            Func<string>            message
        ) {
            var msg = message?.Invoke();
            if (msg.IsBlank()) {
                // 📝 NOTE: NUnit can't handle null message providers...
                Assume.That(actual, constraint);
            }
            else {
                Assume.That(actual, constraint, msg);
            }
        }

        public override void ResolveAction(
            TestDelegate       action,
            IResolveConstraint constraint,
            Func<string>       message
        ) {
            if (message == null) {
                Assume.That(action, constraint);
            }
            else {
                Assume.That(action, constraint, message);
            }
        }
    }

    public static class Assumer {
        public static Assumer<T> Against<T>(T actual) {
            return new Assumer<T>(actual);
        }

        public static Assumer<object> WithHeading(string heading) => new Assumer<object>().WithHeading(heading);
    }
}