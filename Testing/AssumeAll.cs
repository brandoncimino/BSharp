using System;

using JetBrains.Annotations;

using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    [PublicAPI]
    public class AssumeAll {
        public static void Of(string heading, params Action[] assumptions) {
            Assumer.WithHeading(heading)
                   .And(assumptions)
                   .Invoke();
        }

        public static void Of(params Action[] assumptions) {
            Of(null, assumptions);
        }

        public static void Of<T>(string heading, T actual, params IResolveConstraint[] assumptions) {
            Assumer.Against(actual)
                   .WithHeading(heading)
                   .And(assumptions)
                   .Invoke();
        }

        public static void Of<T>(T actual, params IResolveConstraint[] assumptions) {
            Of(null, actual, assumptions);
        }
    }
}