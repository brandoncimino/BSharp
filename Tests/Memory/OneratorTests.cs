using FowlFever.BSharp.Memory;

using NUnit.Framework;

namespace BSharp.Tests.Memory;

public class OneratorTests {
    [Test]
    public void Onerator_Default_IsEmpty() {
        Onerator<string> onerator = default;

        foreach (var it in onerator) {
            Assert.Fail("Should not have enumerated anything!");
        }
    }

    [Test]
    public void Onerator_NonDefault_HasValue() {
        Onerator<string> onerator = new("a");

        var done = false;

        foreach (var it in onerator) {
            Assert.That(it,   Is.EqualTo("a"));
            Assert.That(done, Is.False);
            done = true;
        }
    }

    [Test]
    public void Onerator_OfNull_EnumeratesOnce() {
        Onerator<string?> onerator = new(null);

        var done = false;

        foreach (var it in onerator) {
            Assert.That(it,   Is.Null);
            Assert.That(done, Is.False);
            done = true;
        }
    }
}