using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp.Enums;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FowlFever.Testing {
    /// <summary>
    /// Enum values that correspond to common NUnit <see cref="Constraint"/>s such as <see cref="NUnit.Framework.Is"/>.<see cref="NUnit.Framework.Is.Null"/>,
    /// enabling them to be referenced in <see cref="NUnit.Framework.TestCaseAttribute"/>s
    /// which only accept constant values.
    /// </summary>
    public enum Should {
        /// <summary>
        /// Corresponds to <see cref="NUnit.Framework.Is"/>.<see cref="NUnit.Framework.Is.True"/> (i.e. <see cref="TrueConstraint"/>).
        /// </summary>
        /// <remarks>
        /// TODO: Change this to represent <see cref="Throws.Nothing"/>, and create <c>Should.BeTrue</c> to represent <see cref="NUnit.Framework.Is.True"/>
        ///     This is too ambiguous, and it implies that the test method itself is expected to throw an exception, like some other test frameworks do.
        /// </remarks>
        Pass,
        /// <summary>
        /// Corresponds to <see cref="NUnit.Framework.Is"/>.<see cref="NUnit.Framework.Is.False"/> (i.e. <see cref="FalseConstraint"/>).
        /// </summary>
        /// <remarks>
        /// TODO: Change this to represent <see cref="Throws.Exception"/>, and create <c>Should.BeFalse</c>
        /// </remarks>
        Fail,
        /// <summary>
        /// Corresponds to <see cref="NUnit.Framework.Is"/>.<see cref="NUnit.Framework.Is.Null"/> (i.e. <see cref="NullConstraint"/>).
        /// </summary>
        BeNull,
        /// <summary>
        /// Corresponds to <see cref="NUnit.Framework.Is"/>.<see cref="NUnit.Framework.Is.Not"/>.<see cref="ConstraintExpression.Null"/>.
        /// </summary>
        BeNotNull,
    }

    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public static class ShouldExtensions {
        public static bool Boolean(this Should should) {
            return should switch {
                Should.Pass => true,
                Should.Fail => false,
                _           => throw BEnum.InvalidEnumArgumentException(nameof(should), should)
            };
        }

        public static Constraint Constrain(this Should should) {
            return should switch {
                Should.Pass      => Is.True,
                Should.Fail      => Is.False,
                Should.BeNull    => Is.Null,
                Should.BeNotNull => Is.Not.Null,
                _                => throw BEnum.InvalidEnumArgumentException(nameof(should), should)
            };
        }
    }
}