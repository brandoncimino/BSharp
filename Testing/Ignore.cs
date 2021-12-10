using System;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Optional;

using JetBrains.Annotations;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    /// <summary>
    /// The equivalent to <see cref="AssertAll"/> and <see cref="AssumeAll"/>, but for <see cref="IgnoreException"/>s.
    ///
    /// There are 2 main groups of methods: <see cref="If{T}(T,NUnit.Framework.Constraints.IResolveConstraint)"/> and <see cref="Unless{T}(T,NUnit.Framework.Constraints.IResolveConstraint)"/>.
    /// <p/>
    /// <b><see cref="Unless{T}(T,NUnit.Framework.Constraints.IResolveConstraint)"/></b>
    /// <br/>
    /// Describes an <see cref="IResolveConstraint"/> that, if <b>not</b> satisfied, will throw an <see cref="IgnoreException"/>.
    /// <p/>
    /// <b><see cref="If{T}(T,NUnit.Framework.Constraints.IResolveConstraint)"/></b>
    /// <br/>
    /// Describes an <see cref="IResolveConstraint"/> that, if satisfied, will throw an <see cref="IgnoreException"/>.
    /// </summary>
    /// <remarks>
    /// The syntax of these methods is <i>slightly</i> different than that of <see cref="AssertAll"/> and <see cref="AssumeAll"/> because of the grammar of the word "ignore".
    /// </remarks>
    [PublicAPI]
    public static class Ignore {
        /// <summary>
        /// Applies an <see cref="IResolveConstraint"/> that, <b>if satisfied</b>, will throw an <see cref="IgnoreException"/>.
        /// </summary>
        /// <remarks>
        /// This is the more idiomatic use of the word "ignore", but actually inverts (i.e. <see cref="NotConstraint"/>)
        /// the provided <paramref name="constraint"/>.
        /// </remarks>
        /// <param name="actual">the actual <typeparamref name="T"/> value</param>
        /// <param name="constraint">the <see cref="IResolveConstraint"/> applied to <paramref name="actual"/></param>
        /// <typeparam name="T">the type of <paramref name="actual"/></typeparam>
        public static void If<T>(T actual, IResolveConstraint constraint) {
            Unless(actual, new NotConstraint(constraint.Resolve()));
        }

        public static void If<T>(ActualValueDelegate<T> actual, IResolveConstraint constraint) {
            constraint.Resolve().ApplyTo(actual);
        }

        public static void Unless<T>(
            ActualValueDelegate<T> actualValueDelegate,
            IResolveConstraint     constraint,
            Func<string>?          messageProvider
        ) {
            var appliedConstraint = constraint.Resolve().ApplyTo(actualValueDelegate);
            HandleConstraintResult(appliedConstraint, messageProvider);
        }

        public static void Unless(
            TestDelegate       action,
            IResolveConstraint constraint,
            Func<string>?      messageProvider
        ) {
            var appliedConstraint = constraint.Resolve().ApplyTo(action);
            HandleConstraintResult(appliedConstraint, messageProvider);
        }

        private static void HandleConstraintResult(ConstraintResult result, Func<string>? messageProvider) {
            if (result.IsSuccess == false) {
                var mParts = new[] {
                    messageProvider?.Try().OrDefault(),
                    result.Description
                };

                Assert.Ignore(mParts.NonNull().JoinLines());
            }
        }

        /// <summary>
        /// Applies an <see cref="IResolveConstraint"/> that, if <b>not satisfied</b>, will throw an <see cref="IgnoreException"/>.
        /// </summary>
        /// <remarks>
        /// This follows the form of <see cref="Assert.That{T}(T,IResolveConstraint)"/>, but is less idiomatic than the inverse, <see cref="If{T}(T,NUnit.Framework.Constraints.IResolveConstraint)"/>.
        /// </remarks>
        /// <param name="actual">the actual <typeparamref name="T"/> value</param>
        /// <param name="constraint">the <see cref="IResolveConstraint"/> applied to <paramref name="actual"/></param>
        /// <typeparam name="T">the type of <paramref name="actual"/></typeparam>
        public static void Unless<T>(T actual, IResolveConstraint constraint) {
            var appliedConstraint = constraint.Resolve().ApplyTo(actual);
            if (appliedConstraint.IsSuccess == false) {
                Assert.Ignore(appliedConstraint.Description);
            }
        }

        public static void Unless<T>(ActualValueDelegate<T> actual, IResolveConstraint constraint) {
            var appliedConstraint = constraint.Resolve().ApplyTo(actual);
            if (appliedConstraint.IsSuccess == false) {
                Assert.Ignore(appliedConstraint.Description);
            }
        }

        public static void Unless(string heading, params Action[] ignoreActions) {
            Ignorer.WithHeading(heading)
                   .And(ignoreActions)
                   .Invoke();
        }

        public static void Unless(params Action[] assertions) {
            Unless(null, assertions);
        }

        public static void Unless<T>(string heading, T actual, params IResolveConstraint[] constraints) {
            Ignorer.Against(actual)
                   .WithHeading(heading)
                   .And(constraints)
                   .Invoke();
        }

        public static void Unless<T>(T actual, params IResolveConstraint[] constraints) {
            Ignorer.Against(actual)
                   .And(constraints)
                   .Invoke();
        }

        public static void Unless<T>(ActualValueDelegate<T> actual, params IResolveConstraint[] constraints) {
            Ignorer.Against(actual)
                   .And(constraints)
                   .Invoke();
        }
    }
}