using System;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    public class Ignorer<T> : MultipleAsserter<Ignorer<T>, T> {
        public Ignorer() { }

        public Ignorer(T actual) : base(actual) { }

        protected override void OnFailure(string results) => Assert.Ignore(results);

        public override void ResolveFunc<T1>(
            ActualValueDelegate<T1> actual,
            IResolveConstraint      constraint,
            Func<string>            message
        ) => Ignore.Unless(actual, constraint, message);

        public override void ResolveAction(
            TestDelegate       action,
            IResolveConstraint constraint,
            Func<string>       message
        ) => Ignore.Unless(action, constraint, message);
    }

    public static class Ignorer {
        public static Ignorer<T> Against<T>(T actual) {
            return new Ignorer<T>(actual);
        }

        public static Ignorer<object> WithHeading(string heading) => new Ignorer<object>().WithHeading(heading);
    }
}