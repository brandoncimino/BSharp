using System;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Memory;
using FowlFever.Testing;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BSharp.Tests.Memory.RoMultiSpan;

public static partial class Assertions {
    public static TSelf And<TSelf, T>(
        this IMultipleAsserter<TSelf>                  ass,
        RoMultiSpan<T>.ElementCoord                    actual,
        RoMultiSpan<T>.ElementCoord                    expected,
        [CallerArgumentExpression("actual")]   string? _actual   = default,
        [CallerArgumentExpression("expected")] string? _expected = default
    ) where TSelf : IMultipleAsserter<TSelf> {
        return ass.And(actual == expected, Is.True, $"{_actual} {actual.ToString()} == {_expected} {expected.ToString()}");
    }

    public static T AndIsDefault<T>(this IMultipleAsserter<T> asserter, RoMultiSpan<char>.ElementCoord coord, IResolveConstraint constraint, [CallerArgumentExpression("coord")] string? _coord = default) where T : IMultipleAsserter<T> {
        var subAsserter = Asserter.WithHeading(_coord)
                                  .And(coord.IsDefault, constraint);

        if (constraint == Is.True) {
            subAsserter = subAsserter.And(coord.Span, Is.EqualTo(0))
                                     .And(coord.Element, Is.EqualTo(0))
                                     .And(coord.Tuple(), Is.EqualTo((0, 0)));
        }

        return asserter.And(subAsserter);
    }

    public static Index AsIndex(this int value) => value < 0 ? Index.FromEnd(-value) : value;
}

public class RoMultiSpan_ElementCoord {
    [Test]
    [TestCase(new[] { "abc", "yolo", "dd" }, 0,  0, 0)]
    [TestCase(new[] { "", "", "a" },         0,  2, 0)]
    [TestCase(new[] { "abc", "", "yolo" },   -5, 0, 2)]
    public void GetCoord(string[] strings, int elementIndex, int coordSpan, int coordElement) {
        var index = elementIndex.AsIndex();
        var spans = FowlFever.BSharp.Memory.RoMultiSpan.Of(strings);
        var coord = spans.GetCoord(index);
        Asserter.WithHeading(coord.ToString())
                .And(coord.Tuple(), Is.EqualTo((coordSpan, coordElement)))
                .Invoke();
    }

    [Test]
    public void DefaultCoordIsDefault() {
        Asserter.WithHeading()
                .AndIsDefault(default,                                                   Is.True)
                .AndIsDefault(new RoMultiSpan<char>.ElementCoord(),                      Is.False)
                .AndIsDefault(new RoMultiSpan<char>.ElementCoord(0, 0),                  Is.False)
                .AndIsDefault(default(RoMultiSpan<char>.ElementCoord) with { Span = 0 }, Is.False)
                .AndIsDefault(default(RoMultiSpan<char>.ElementCoord) with { },          Is.True)
                .AndIsDefault(new RoMultiSpan<char>.ElementCoord() { },                  Is.False)
                .AndIsDefault(new RoMultiSpan<char>.ElementCoord() { Span = 0 },         Is.False)
                .Invoke();
    }
}