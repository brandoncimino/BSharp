using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp.Enums;

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
        /// <summary>
        /// Corresponds to <see cref="Throws"/>.<see cref="Throws.Exception"/>
        /// </summary>
        ThrowException,
        /// <summary>
        /// Corresponds to <see cref="Throws"/>.<see cref="Throws.Exception"/>
        /// </summary>
        ThrowNothing,
    }

    public enum ShouldStyle {
        Default,
        Boolean,
        Exception,
        Nullity,
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

        public static IResolveConstraint ToNullConstraint(this Should should) {
            return should switch {
                Should.Pass      => Is.Not.Null,
                Should.Fail      => Is.Null,
                Should.BeNotNull => Is.Not.Null,
                Should.BeNull    => Is.Null,
                _                => throw BEnum.UnhandledSwitch(should),
            };
        }

        private static IResolveConstraint ToBooleanConstraint(this Should should) => should switch {
            Should.Pass => Is.True,
            Should.Fail => Is.False,
            _           => throw BEnum.UnhandledSwitch(should),
        };

        private static IResolveConstraint ToThrowsConstraint(this Should should) => should switch {
            Should.Pass           => Throws.Nothing,
            Should.Fail           => Throws.Exception,
            Should.ThrowNothing   => Throws.Nothing,
            Should.ThrowException => Throws.Exception,
            _                     => throw BEnum.UnhandledSwitch(should),
        };

        public static IResolveConstraint Constrain(this Should should, ShouldStyle style = ShouldStyle.Default) {
            IResolveConstraint DefaultConstraint() {
                return should switch {
                    Should.Pass           => Is.True,
                    Should.Fail           => Is.False,
                    Should.BeNull         => Is.Null,
                    Should.BeNotNull      => Is.Not.Null,
                    Should.ThrowException => Throws.Exception,
                    Should.ThrowNothing   => Throws.Nothing,
                    _                     => throw BEnum.InvalidEnumArgumentException(nameof(should), should)
                };
            }

            return style switch {
                ShouldStyle.Default   => DefaultConstraint(),
                ShouldStyle.Boolean   => should.ToBooleanConstraint(),
                ShouldStyle.Nullity   => should.ToNullConstraint(),
                ShouldStyle.Exception => should.ToThrowsConstraint(),
                _                     => throw BEnum.InvalidEnumArgumentException(style),
            };
        }

        public static Should Inverse(this Should should) {
            return should switch {
                Should.Pass           => Should.Fail,
                Should.Fail           => Should.Pass,
                Should.BeNull         => Should.BeNotNull,
                Should.BeNotNull      => Should.BeNull,
                Should.ThrowException => Should.ThrowNothing,
                Should.ThrowNothing   => Should.ThrowException,
                _                     => throw BEnum.InvalidEnumArgumentException(nameof(should), should),
            };
        }
    }
}