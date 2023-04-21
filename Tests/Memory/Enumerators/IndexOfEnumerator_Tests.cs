using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Memory.Enumerators;

using NUnit.Framework;

namespace BSharp.Tests.Memory.Enumerators;

public class IndexOfEnumerator_Tests {
    private static IEnumerable<int> GetIndices<T>(IEnumerable<T> source, Func<T, bool> predicate) {
        return source.Select((t, i) => (t, i))
                     .Where(tp => predicate(tp.t))
                     .Select(tp => tp.i);
    }

    [TestCase("abcde",     'c')]
    [TestCase("a-------b", '-')]
    [TestCase("",          'a')]
    [TestCase("abc",       'z')]
    [TestCase("a",         'a')]
    [TestCase("ab",        'a')]
    [TestCase("ab",        'b')]
    public void IndexOfEnumeratorTest(string input, char indexOf) {
        var got = new List<int>();

        foreach (var index in new IndexOfEnumerator<char>(input, indexOf)) {
            got.Add(index);
        }

        Assert.That(got, Is.EqualTo(GetIndices(input, indexOf.Equals)));
    }

    [TestCase("abcde",    "ab")]
    [TestCase("abc",      "xyz")]
    [TestCase("aaaaaaaa", "b")]
    [TestCase("",         "abc")]
    public void IndexOfAnyEnumeratorTest(string input, string indexOfAny) {
        var got = new List<int>();

        foreach (var index in new IndexOfAnyEnumerator<char>(input, indexOfAny)) {
            got.Add(index);
        }

        Assert.That(got, Is.EqualTo(GetIndices(input, indexOfAny.Contains)));
    }
}