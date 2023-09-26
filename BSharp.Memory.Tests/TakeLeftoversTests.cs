using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp.Memory;

namespace BSharp.Memory.Tests;

public class TakeLeftoversTests {
    [Test]
    [SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure")]
    public void TakeLeftovers(
        [Values(new int[] { 1, 2, 3 }, new int[] { })]
        int[] source,
        [Values(-2, -1, 0, 1, 2, 3, 4, 5)] int toTake
    ) {
        Assert.Multiple(
            () => {
                var expectedTaken     = source.AsSpan().Take(toTake);
                var expectedLeftovers = source.AsSpan().Skip(toTake);

                var (spanTaken, spanLeftovers) = source.AsSpan().TakeLeftovers(toTake);

                Assert.That(spanTaken     == expectedTaken);
                Assert.That(spanLeftovers == expectedLeftovers);

                var (roSpanTaken, roSpanLeftovers) = source.AsSpan().AsReadOnly().TakeLeftovers(toTake);

                Assert.That(roSpanTaken     == expectedTaken);
                Assert.That(roSpanLeftovers == expectedLeftovers);
            }
        );
    }
}