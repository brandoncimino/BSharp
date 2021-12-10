using System;
using System.Text.RegularExpressions;

using BrandonUtils.Standalone.Strings;
using BrandonUtils.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BrandonUtils.Tests.Standalone.Strings {
    public class SubstringTests {
        [Test]
        [TestCase("abc",              "b",        "a",    "c",     "a",    "c",               "a",               "c")]
        [TestCase("aabbcc",           "b",        "aa",   "cc",    "aa",   "bcc",             "aab",             "cc")]
        [TestCase("aabbcc",           "bc",       "aab",  "c",     "aab",  "c",               "aab",             "c")]
        [TestCase("yolo.swag",        ".",        "yolo", "swag",  "yolo", "swag",            "yolo",            "swag")]
        [TestCase("one.two.three",    ".",        "one",  "three", "one",  "two.three",       "one.two",         "three")]
        [TestCase("one!!two!!!three", "!!",       "one",  "three", "one",  "two!!!three",     "one!!two",        "three")]
        [TestCase("!extreme thirst!", "!",        "",     "",      "",     "extreme thirst!", "!extreme thirst", "")]
        [TestCase("buttz",            null,       "",     "",      null,   null,              null,              null)]
        [TestCase("asdfasdf",         "",         "",     "",      null,   null,              null,              null)]
        [TestCase(null,               "abc",      "",     "",      null,   null,              null,              null)]
        [TestCase("",                 "",         "",     "",      null,   null,              null,              null)]
        [TestCase(null,               null,       "",     "",      null,   null,              null,              null)]
        [TestCase("",                 "yolo",     "",     "",      null,   null,              null,              null)]
        [TestCase("yolo",             "yoloswag", "",     "",      null,   null,              null,              null)]
        [TestCase("#yolo",            "#",        "",     "yolo",  "",     "yolo",            "",                "yolo")]
        public void Substring_Simple(
            string original,
            string splitter,
            string substring_before,
            string substring_after,
            string bisect_before,
            string bisect_after,
            string bisectLast_before,
            string bisectLast_after
        ) {
            AssertAll.Of(
                () => Assert.That(original.SubstringBefore(splitter), Is.EqualTo(substring_before)),
                () => Assert.That(original.SubstringAfter(splitter),  Is.EqualTo(substring_after)),
                () => Assert.That(original.Bisect(splitter)?.Item1,   Is.EqualTo(bisect_before)),
                () => Assert.That(original.Bisect(splitter)?.Item2,   Is.EqualTo(bisect_after))
            );
        }

        [Test]
        [TestCase("yolo.swag",        ".",    "",    "")]
        [TestCase("b00bz",            @"\d",  "b",   "bz")]
        [TestCase("one%-two~!~three", @"\W+", "one", "three")]
        public void Substring_Regex(string original, string pattern, string expected_before, string expected_after) {
            AssertAll.Of(
                () => Assert.That(original.SubstringBefore(new Regex(pattern)), Is.EqualTo(expected_before)),
                () => Assert.That(original.SubstringAfter(new Regex(pattern)),  Is.EqualTo(expected_after))
            );
        }

        [Test]
        public void regFlag() {
            var regexOps = RegexOptions.Compiled | RegexOptions.Multiline;
            Console.WriteLine($"{nameof(regexOps)} = {regexOps}");
            Console.WriteLine($"is rtl = {regexOps.HasFlag(RegexOptions.RightToLeft)}");
            var ro2 = regexOps | RegexOptions.RightToLeft;
            Console.WriteLine($"{nameof(ro2)} = {ro2}, {ro2.HasFlag(RegexOptions.RightToLeft)}");
            var ro3 = ro2 | RegexOptions.RightToLeft;
            Console.WriteLine($"{nameof(ro3)} = {ro3}, {ro3.HasFlag(RegexOptions.RightToLeft)}");
        }
    }
}