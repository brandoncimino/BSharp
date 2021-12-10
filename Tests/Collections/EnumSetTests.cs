using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using BrandonUtils.Standalone.Enums;
using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BrandonUtils.Tests.Standalone.Collections {
    public class EnumSetTests {
        public enum Should {
            Pass,
            Fail
        }

        public enum EnumWithDuplicates {
            Red,
            Green,
            Blue,
            Crimson = Red
        }

        private static EnumSet<DayOfWeek> GetWeekend() => new EnumSet<DayOfWeek>(DayOfWeek.Saturday, DayOfWeek.Sunday);

        private static EnumSet<DayOfWeek> GetAllWeek() => new EnumSet<DayOfWeek>(
            DayOfWeek.Sunday,
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday
        );

        [Test]
        [TestCase(DayOfWeek.Tuesday)]
        [TestCase(DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday)]
        public void MustContainFail_Multiple(params DayOfWeek[] mustBeContained) {
            var set = GetWeekend();

            Assert.Throws<EnumNotInSetException<DayOfWeek>>(() => set.MustContain(mustBeContained));
        }

        [Test]
        [TestCase(DayOfWeek.Tuesday,  Should.Fail)]
        [TestCase(DayOfWeek.Saturday, Should.Pass)]
        public void MustContain_Single(DayOfWeek mustBeContained, Should should) {
            var set = GetWeekend();

            // Apparently, this is more performant than using a lambda?
            void Must() => set.MustContain(mustBeContained);

            Action<TestDelegate> mustResolver = should switch {
                Should.Pass => Assert.DoesNotThrow,
                Should.Fail => test => Assert.Throws<EnumNotInSetException<DayOfWeek>>(test),
                _           => throw new ArgumentException()
            };

            AssertAll.Of(
                () => mustResolver(Must),
                () => Assert.That(set.Contains(mustBeContained), Is.EqualTo(should == Should.Pass))
            );
        }

        [Test]
        public void MustContain_WithDuplicates() {
            var set = EnumSet.Of(EnumWithDuplicates.Crimson, EnumWithDuplicates.Blue);
            AssertAll.Of(
                () => Assert.DoesNotThrow(() => set.MustContain(EnumWithDuplicates.Crimson)),
                () => Assert.DoesNotThrow(() => set.MustContain(EnumWithDuplicates.Red))
            );
        }

        [Test]
        public void Of() {
            // NOTE: the order of the elements in the EnumSet matters, so [sat, sun] != [sun, sat]
            var set = EnumSet.Of(DayOfWeek.Saturday, DayOfWeek.Sunday);
            Assert.That(set, Is.EqualTo(GetWeekend()));
        }

        [Test]
        public void OfAllValues() {
            var expectedSet = GetAllWeek();
            var actualSet   = EnumSet.OfAllValues<DayOfWeek>();

            Assert.That(expectedSet, Is.EqualTo(actualSet));
        }

        [Test]
        public void EquivalencyWithMismatchedOrder() {
            var backwardsWeekend = new EnumSet<DayOfWeek>(DayOfWeek.Sunday, DayOfWeek.Saturday);
            AssertAll.Of(
                backwardsWeekend,
                Is.Not.EqualTo(GetWeekend()),
                Is.EquivalentTo(GetWeekend())
            );
        }

        [Test]
        public void OfAllValues_WithDuplicates() {
            var values         = Enum.GetValues(typeof(EnumWithDuplicates));
            var distinctValues = values.Cast<EnumWithDuplicates>().Distinct();

            var ofAllValues = EnumSet.OfAllValues<EnumWithDuplicates>();

            AssertAll.Of(
                ofAllValues,
                Has.Count.EqualTo(3),
                Is.EquivalentTo(distinctValues),
                Is.Not.EqualTo(values)
            );
        }

        #region ReadOnlyEnumSet

        [Test]
        public void enumExc() {
            throw new NotImplementedException("I don't think this test is finished...what was I trying to do?");
            var excepts = new Dictionary<string, string>() {
                ["arg(null, null)"]     = new ArgumentException(null,   (string)null).Message,
                ["arg(null, yolo)"]     = new ArgumentException(null,   "yolo").Message,
                ["arg(yolo, null)"]     = new ArgumentException("yolo", (string)null).Message,
                ["earg(null, -1, DoW)"] = new InvalidEnumArgumentException(null,   -1, typeof(DayOfWeek)).Message,
                ["earg(null,null)"]     = new InvalidEnumArgumentException(null,   null).Message,
                ["earg(yolo,null)"]     = new InvalidEnumArgumentException("yolo", null).Message,
                ["earg(null,npe)"]      = new InvalidEnumArgumentException(null,   new ArgumentNullException()).Message
            };
            foreach (var pair in excepts) {
                Console.WriteLine($"\n{pair.Key} = {pair.Value}");
            }
            // throw new ArgumentException(null, (string)null);
            // throw new InvalidEnumArgumentException(null, -1, typeof(DayOfWeek));
        }

        #endregion
    }
}