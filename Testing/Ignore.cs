using System;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;

using JetBrains.Annotations;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FowlFever.Testing {
    /// <summary>
    /// The equivalent to <see cref="AssertAll"/>, but for <see cref="IgnoreException"/>s.
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
    /// The syntax of these methods is <i>slightly</i> different than that of <see cref="AssertAll"/> because of the grammar of the word "ignore".
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

        public static void If<T>(Func<T> actual, IResolveConstraint constraint) {
            constraint.Resolve().ApplyTo(actual);
        }

        public static void Unless<T>(
            Func<T>            actualValueDelegate,
            IResolveConstraint constraint,
            Func<string>?      messageProvider
        ) {
            var appliedConstraint = constraint.Resolve().ApplyTo(actualValueDelegate);
            HandleConstraintResult(appliedConstraint, messageProvider);
        }

        public static void Unless(
            Action             action,
            IResolveConstraint constraint,
            Func<string>?      messageProvider
        ) {
            var appliedConstraint = constraint.Resolve().ApplyTo(action);
            HandleConstraintResult(appliedConstraint, messageProvider);
        }

        public static void Unless(
            object             actual,
            IResolveConstraint constraint,
            Func<string>?      messageProvider
        ) {
            var appliedConstraint = constraint.Resolve().ApplyTo(actual);
            HandleConstraintResult(appliedConstraint, messageProvider);
        }

        private static void HandleConstraintResult(ConstraintResult result, Func<string>? messageProvider) {
            if (result.IsSuccess) {
                return;
            }

            var mParts = new[] {
                messageProvider?.Try().OrDefault(), result.Description
            };

            Assert.Ignore(mParts.NonNull().JoinLines());
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

        public static void Unless<T>(Func<T> actual, IResolveConstraint constraint) {
            var appliedConstraint = constraint.Resolve().ApplyTo(actual);
            if (appliedConstraint.IsSuccess == false) {
                Assert.Ignore(appliedConstraint.Description);
            }
        }

        public static void Unless(string? heading, Action ignoreAction, params Action[] moreActions) {
            Ignorer.WithHeading(heading)
                   .And(moreActions)
                   .Invoke();
        }

        public static void Unless(Action assertion, params Action[] moreAssertions) {
            Ignorer.WithHeading(default)
                   .And(assertion)
                   .And(moreAssertions)
                   .Invoke();
        }

        public static void Unless<T>(Supplied<string?> heading, T actual, IResolveConstraint constraint, params IResolveConstraint[] moreConstraints) {
            Ignorer.Against(actual)
                   .WithHeading(heading)
                   .And(constraint)
                   .And(moreConstraints)
                   .Invoke();
        }

        public static void Unless<T>(T actual, IResolveConstraint constraint, params IResolveConstraint[] moreConstraints) {
            Ignorer.Against(actual)
                   .And(constraint)
                   .And(moreConstraints)
                   .Invoke();
        }

        public static void Unless<T>(Func<T> actual, IResolveConstraint constraint, params IResolveConstraint[] moreConstraints) {
            Ignorer.Against(actual)
                   .And(constraint)
                   .And(moreConstraints)
                   .Invoke();
        }

        /// <inheritdoc cref="Assert.Ignore(string?)"/>
        /// <remarks>This is an alias for <see cref="Assert.Ignore(string?)"/>.</remarks>
        [ContractAnnotation("=> stop")]
        public static void This(string? message, Exception? inner = default) {
            Assert.Ignore(message, inner);
        }
    }
}