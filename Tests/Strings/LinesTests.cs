using System;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests.Strings;

public class LinesTests {
    [Test]
    [TestCase(
        @"a
b
cd
ef\ng",
        new[] { "a", "b", "cd", "ef", "g" }
    )]
    public void LinesTest(string content, string[] expectedLines) {
        var lines = content.Lines();
        Console.WriteLine("LINES\n----");
        Console.WriteLine(lines);
        Console.WriteLine("----");
        Console.WriteLine(lines.JoinString("-"));
        Console.WriteLine("----");
        foreach (var ln in lines) {
            Console.WriteLine($"{ln.GetType().Name} => {ln}");
        }

        Console.WriteLine(lines.AsEnumerable().JoinString("-"));
        Assert.That(content.Lines().Select(it => it.Value).ToArray(), Is.EqualTo(expectedLines));
    }

    [Test]
    public void OneLine_Default_IsValid() {
        Asserter.Against(default(OneLine))
                .And(Is.EqualTo(OneLine.Create("")))
                .And(Is.EqualTo(new OneLine()))
                .And(it => it.ToString(),    Is.EqualTo(""))
                .And(it => it.Length,        Is.EqualTo(0))
                .And(it => it.Value,         Is.EqualTo(""))
                .And(it => it.IsBlank,       Is.True)
                .And(it => it.IsEmpty,       Is.True)
                .And(it => it.GetHashCode(), Is.EqualTo(OneLine.Create("").GetHashCode()))
                .Invoke();
    }
}