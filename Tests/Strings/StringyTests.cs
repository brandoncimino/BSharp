using FowlFever.BSharp.Strings;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Strings;

public class StringyTests {
    [TestCase("a", "",  "a")]
    [TestCase("",  "b", "b")]
    [TestCase("",  "",  "")]
    [TestCase("a", "b", "ab")]
    public void Stringy_Concat(string a, string b, string expected) {
        Asserter.Against(Stringy.Concat(a, b))
                .And(Is.EqualTo(expected))
                .Invoke();
    }

    #region JoinNonBlank

    [Test]
    public void String_JoinNonBlank(
        [Values("a", null, " ", "")] string? a,
        [Values("b", null, " ", "")] string? b,
        [Values("-", null, " ", "")] string? joiner
    ) {
        var aBlank = string.IsNullOrWhiteSpace(a);
        var bBlank = string.IsNullOrWhiteSpace(b);
        var expected = (aBlank, bBlank) switch {
            (true, true) => "",
            (true, _)    => b!,
            (_, true)    => a!,
            _            => $"{a}{joiner}{b}"
        };

        Asserter.Against(Stringy.JoinNonBlank(a, b, joiner))
                .And(Is.EqualTo(expected))
                .Invoke();
    }

    #endregion
}