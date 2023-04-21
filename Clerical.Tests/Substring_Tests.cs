using FowlFever.Clerical;

namespace Clerical.Tests;

public class Substring_Tests {
    [Test]
    [TestCase("abcde", 2, 2)]
    [TestCase("abcde", 0, 5)]
    public void Substring_TryCreateFromSpan_Valid(string input, int subStart, int subLength) {
        Assert.Multiple(
            () => {
                var subSpan = input.AsSpan(subStart, subLength);

                Assert.That(Substring.TryCreateFromSpan(subSpan, input, out var result));
                Assert.That(result,            Has.Property(nameof(Substring.Length)).EqualTo(subLength));
                Assert.That(result,            Has.Property(nameof(Substring.Start)).EqualTo(subStart));
                Assert.That(result,            Has.Property(nameof(Substring.Source)).SameAs(input));
                Assert.That(result.ToString(), Is.EqualTo(subSpan.ToString()));

                var actualSpan = result.AsSpan();
                Assert.That(actualSpan.Length, Is.EqualTo(subSpan.Length));
                Assert.That(actualSpan.Overlaps(subSpan, out var offset));
                Assert.That(offset, Is.Zero);
            }
        );
    }

    [TestCase("abcde", 2)]
    public void Substring_TryCreateFromSpan_Empty(string input, int sliceIndex) {
        Assert.Multiple(
            () => {
                var subSpan = input.AsSpan(sliceIndex, 0);
                Assert.That(Substring.TryCreateFromSpan(subSpan, input, out var result), Is.False);
                Assert.That(result.IsDefault);
            }
        );
    }

    [TestCase("abc", "xyz")]
    public void Substring_TryCreateFromSpan_WrongSource(string actualSource, string sliceSource) {
        Assert.That(actualSource, Is.Not.SameAs(sliceSource));

        Assert.Multiple(
            () => {
                var subSpan = sliceSource.AsSpan();
                Assert.That(Substring.TryCreateFromSpan(subSpan, actualSource, out var result), Is.False);
                Assert.That(result.IsDefault);
            }
        );
    }
}