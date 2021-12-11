using System;

using FowlFever.BSharp.Clerical.Saving;
using FowlFever.BSharp.Strings;
using FowlFever.Testing;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Tests.Standalone.Clerical {
    public static class SaveFileAsserter {
        public static Asserter<ISaveFile<T>> Exists<T>(this Asserter<ISaveFile<T>> self, bool expectedExistence = true) where T : ISaveData {
            ConstraintExpression constraint = Has.Property(nameof(SaveFile<T>.FileSystemInfo));
            constraint = expectedExistence ? constraint : constraint.Not;
            return self.And(constraint.Exist);
        }

        public static Asserter<SaveFile<T>> Exists<T>(this Asserter<SaveFile<T>> self, bool expectedExistence = true) where T : ISaveData {
            ConstraintExpression constraint = Has.Property(nameof(SaveFile<T>.FileSystemInfo));
            constraint = expectedExistence ? constraint : constraint.Not;
            return self.And(constraint.Exist);
        }

        public static Asserter<ISaveFile<T>> Nicknamed<T>(this Asserter<ISaveFile<T>> self, string expectedNickname) where T : ISaveData {
            return self.And(Has.Property(nameof(ISaveFile<ISaveData>.Nickname)).EqualTo(expectedNickname));
        }

        public static Asserter<ISaveFile<T>> TimeStamped<T>(this Asserter<ISaveFile<T>> self, DateTime expectedTimeStamp) where T : ISaveData {
            return self.And(Has.Property(nameof(ISaveFile<ISaveData>.TimeStamp)).EqualTo(expectedTimeStamp));
        }

        public static void IsEquivalentTo<T>(this ISaveFile<T> actual, ISaveFile<T> expected) where T : ISaveData {
            Asserter.Against(actual)
                    .WithHeading($"First {actual.GetType().Prettify()} must be equivalent to the second {expected.GetType().Prettify()}")
                    .And(Has.Property(nameof(actual.Nickname)).EqualTo(expected.Nickname))
                    .And(Has.Property(nameof(actual.TimeStamp)).EqualTo(expected.TimeStamp))
                    .And(Has.Property(nameof(actual.Data)).EqualTo(expected.Data))
                    .Invoke();
        }
    }
}