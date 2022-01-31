using System;
using System.Linq;

using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BSharp.Tests.Collections; 

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

        Assert.That(() => set.Contains(mustBeContained), Is.EqualTo(should == Should.Pass));
        Assert.That(() => mustResolver(Must),            Throws.Nothing);
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
        Asserter.Against(backwardsWeekend)
                .And(Is.Not.EqualTo(GetWeekend()))
                .And(Is.EquivalentTo(GetWeekend()))
                .Invoke();
    }

    [Test]
    public void OfAllValues_WithDuplicates() {
        var values         = Enum.GetValues(typeof(EnumWithDuplicates));
        var distinctValues = values.Cast<EnumWithDuplicates>().Distinct().ToArray();

        var ofAllValues = EnumSet.OfAllValues<EnumWithDuplicates>();

        Asserter.Against(ofAllValues)
                .And(Has.Count.EqualTo(3))
                .And(Is.EquivalentTo(distinctValues))
                .And(Is.Not.EqualTo(values))
                .Invoke();
    }

    #region ReadOnlyEnumSet

    //TODO: ???

    #endregion
}