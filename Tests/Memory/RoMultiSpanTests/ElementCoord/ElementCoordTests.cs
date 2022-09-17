using System;

using FowlFever.BSharp.Memory;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Memory.RoMultiSpanTests.ElementCoord;

public static partial class Assertions {
    public static Index AsIndex(this int value) => value < 0 ? Index.FromEnd(-value) : value;
}

public class RoMultiSpanTests {
    [Test]
    [TestCase(new[] { "abc", "yolo", "dd" }, 0,  0, 0)]
    [TestCase(new[] { "", "", "a" },         0,  2, 0)]
    [TestCase(new[] { "abc", "", "yolo" },   -5, 0, 2)]
    public void GetCoord(string[] strings, int elementIndex, int coordSpan, int coordElement) {
        var index = elementIndex.AsIndex();
        var spans = RoMultiSpan.Of(strings);
        var coord = spans.GetCoord(index);
        Asserter.WithHeading(coord.ToString())
                .And(coord, Is.EqualTo((coordSpan, coordElement)))
                .Invoke();
    }
}