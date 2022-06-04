using System;
using System.Runtime.CompilerServices;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FowlFever.Testing {
    public class Ignorer<T> : MultipleAsserter<Ignorer<T>, T> {
        public Ignorer() { }

        public Ignorer(T actual, [CallerArgumentExpression("actual")] string? alias = default) : base(actual, alias) { }

        protected override void OnFailure(string results) => Assert.Ignore(results);

        public override void ResolveFunc<T1>(
            Func<T1>           actual,
            IResolveConstraint constraint,
            Func<string>?      message
        ) => Ignore.Unless(actual, constraint, message);

        public override void ResolveAction(
            Action             action,
            IResolveConstraint constraint,
            Func<string>?      message
        ) => Ignore.Unless(action, constraint, message);

        public override void ResolveActual<T2>(
            T2                 actual,
            IResolveConstraint constraint,
            Func<string>?      message
        ) => Ignore.Unless(actual, constraint, message);
    }

    public static class Ignorer {
        public static Ignorer<T> Against<T>(T actual) {
            return new Ignorer<T>(actual);
        }

        public static Ignorer<object> WithHeading(string heading) => new Ignorer<object>().WithHeading(heading);
    }
}