using System;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Strings;
using FowlFever.Testing;

using NUnit.Framework;

using Is = FowlFever.Testing.Is;

namespace BSharp.Tests.Strings {
    public class RegexGroupTests {
        [TestCase(
            "number",
            @"\d+",
            "I got 99 problems but a switch ain't one",
            @"(?<number>\d+)",
            "99"
        )]
        public void NamedRegexGroup_Match(
            string name,
            string subexpression,
            string input,
            string expectedPattern,
            string expectedMatch
        ) {
            var group = RegexGroup.Named(name, subexpression);
            Asserter.Against(group)
                    .And(Has.Property(nameof(group.Name)).EqualTo(name))
                    .And(Has.Property(nameof(group.Subexpression)).EqualTo(subexpression))
                    .And(Has.Property(nameof(group.Pattern)).EqualTo(expectedPattern))
                    .And(it => it.Regex.ToString(), Is.EqualTo(expectedPattern))
                    .AndAgainst(
                        tf => tf?.Regex.Match(input),
                        asserter => asserter
                                    .And(Has.Property(nameof(Match.Success)).True)
                                    .AndAgainst(
                                        m => m?.Groups[name],
                                        grp => grp
                                               .And(Has.Property(nameof(Group.Success)).True)
                                               .And(Has.Property(nameof(Group.Value)).EqualTo(expectedMatch))
                                    )
                    )
                    .Invoke();
        }

        [TestCase(
            "number",
            @"\d+",
            "abc"
        )]
        public void NamedRegexGroup_NoMatch(
            string name,
            string subexpression,
            string input
        ) {
            var group = RegexGroup.Named(name, subexpression);

            Asserter.Against(group)
                    .And(Has.Property(nameof(group.Name)).EqualTo(name))
                    .And(Has.Property(nameof(group.Subexpression)).EqualTo(subexpression))
                    .AndAgainst(
                        it => it?.Regex.Match(input).Groups,
                        it => it
                              .And(Is.Not.Null)
                              .And(Is.Not.Empty)
                              .And(Contains.Key(name))
                              .AndAgainst(
                                  gc => gc?[name],
                                  grp => grp
                                         .And(Has.Property(nameof(Group.Success)).False)
                                         .And(Has.Property(nameof(Group.Value)).Empty)
                              )
                    )
                    .Invoke();
        }

        [Test]
        public void Lookahead_Positive_Match() => throw new NotImplementedException("Needs to be written");

        [Test]
        public void Lookahead_Positive_NoMatch() => throw new NotImplementedException("Needs to be written");

        [Test]
        public void Lookahead_Negative_Match() => throw new NotImplementedException("Needs to be written");

        [Test]
        public void Lookahead_Negative_NoMatch() => throw new NotImplementedException("Needs to be written");

        [Test]
        public void Lookbehind_Positive_Match() => throw new NotImplementedException("Needs to be written");

        [Test]
        public void Lookbehind_Positive_NoMatch() => throw new NotImplementedException("Needs to be written");

        [Test]
        public void Lookbehind_Negative_Match() => throw new NotImplementedException("Needs to be written");

        [Test]
        public void Lookbehind_Negative_NoMatch() => throw new NotImplementedException("Needs to be written");
    }
}