using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Optional;
using FowlFever.Testing;

using Newtonsoft.Json;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace BSharp.Tests.Collections; 

[TestOf(typeof(CollectionUtils))]
public class CollectionUtilsTests {
    private static Dictionary<int, string> ValidDictionary => new Dictionary<int, string> {
        { 1, "one" },
        { 2, "two" },
        { 3, "three" }
    };

    private static Dictionary<string, int> ValidDictionary_Inverse => new Dictionary<string, int> {
        { "one", 1 },
        { "two", 2 },
        { "three", 3 }
    };

    [Test]
    public void ValidDictionaryInverse() {
        Assert.That(ValidDictionary,           Is.Not.EqualTo(ValidDictionary_Inverse));
        Assert.That(ValidDictionary.Inverse(), Is.EqualTo(ValidDictionary_Inverse));
    }

    [Test]
    public void InvertInvalidDictionaryWithDuplicateValues() {
        var initialDictionary = new Dictionary<int, string> {
            { 1, "one" },
            { 2, "two" },
            { 3, "two" }
        };

        Assert.Throws<ArgumentException>(() => initialDictionary.Inverse());
    }

    [Test]
    public void InvertInvalidDictionaryWithNullValues() {
        var initialDictionary = new Dictionary<int, string> {
            { 1, "one" },
            { 2, null },
            { 3, "three" }
        };

        Assert.Throws<ArgumentNullException>(() => initialDictionary.Inverse());
    }

    [Test]
    public void InverseOfNonReadOnly() {
        var nonReadOnlyInverse = ValidDictionary.Inverse();
        Assert.That(nonReadOnlyInverse, Is.TypeOf<Dictionary<string, int>>());
        Assert.That(nonReadOnlyInverse, Is.Not.TypeOf<ReadOnlyDictionary<string, int>>());
    }

    [Test]
    public void InverseOfReadOnly() {
        var readOnly        = new ReadOnlyDictionary<int, string>(ValidDictionary);
        var readOnlyInverse = readOnly.Inverse();
        Assert.That(readOnlyInverse, Is.TypeOf<ReadOnlyDictionary<string, int>>());
        Assert.That(readOnlyInverse, Is.Not.TypeOf<Dictionary<string, int>>());
    }

    [Test]
    public void InverseInternal() {
        var inv = CollectionUtils.Inverse_Internal(ValidDictionary);
        PrintDictionary(inv);
        Assert.That(inv, Is.EqualTo(ValidDictionary_Inverse));
    }

    private static void PrintDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary) {
        Console.WriteLine(JsonConvert.SerializeObject(dictionary, Formatting.Indented));
    }

    #region Dictionary Joining

    [Test]
    public void JoinWithoutOverlap() {
        var dic1 = new Dictionary<int, int> {
            { 1, 1 },
            { 2, 1 }
        };

        var dic2 = new Dictionary<int, int> {
            { 3, 2 },
            { 4, 2 }
        };

        var expected = new Dictionary<int, int> {
            { 1, 1 },
            { 2, 1 },
            { 3, 2 },
            { 4, 2 }
        };

        Assert.That(dic1.JoinDictionaries(dic2), Is.EqualTo(expected));
    }

    private static Dictionary<int, int> DicOrigin => new Dictionary<int, int> {
        { 1, 1 },
        { 2, 1 },
        { 3, 1 }
    };

    private static Dictionary<int, int> DicOverlap => new Dictionary<int, int> {
        { 3, 2 },
        { 4, 2 },
        { 5, 2 }
    };

    private static Dictionary<int, int> ResultFavoringOriginal => new Dictionary<int, int> {
        { 1, 1 },
        { 2, 1 },
        { 3, 1 },
        { 4, 2 },
        { 5, 2 }
    };

    private static Dictionary<int, int> ResultFavoringNew => new Dictionary<int, int> {
        { 1, 1 },
        { 2, 1 },
        { 3, 2 },
        { 4, 2 },
        { 5, 2 }
    };

    [Test]
    public void JoinPreferOriginal_Extension() {
        var jointDic = DicOrigin.JoinDictionaries(DicOverlap, CollectionUtils.ConflictResolution.FavorOriginal);
        Console.WriteLine(
            new[] {
                $"Original: {JsonConvert.SerializeObject(DicOrigin)}",
                $"Overlap:  {JsonConvert.SerializeObject(DicOverlap)}",
                $"Actual:   {JsonConvert.SerializeObject(jointDic)}",
                $"Expected: {JsonConvert.SerializeObject(ResultFavoringOriginal)}"
            }
        );
        Assert.That(jointDic, Is.EqualTo(ResultFavoringOriginal));
    }

    [Test]
    public void JoinPreferOriginal_Collection() {
        var jointDic = CollectionUtils.JoinDictionaries(new[] { DicOrigin, DicOverlap }, CollectionUtils.ConflictResolution.FavorOriginal);
        Assert.That(jointDic, Is.EqualTo(ResultFavoringOriginal));
    }

    [Test]
    public void JoinPreferNew_Collection() {
        var jointDic = CollectionUtils.JoinDictionaries(new[] { DicOrigin, DicOverlap }, CollectionUtils.ConflictResolution.FavorNew);
        Assert.That(jointDic, Is.EqualTo(ResultFavoringNew));
    }

    [Test]
    public void JoinPreferNew_Extension() {
        var jointDic = DicOrigin.JoinDictionaries(DicOverlap, CollectionUtils.ConflictResolution.FavorNew);
        Console.WriteLine(
            new[] {
                $"Actual:   {JsonConvert.SerializeObject(jointDic)}",
                $"Expected: {JsonConvert.SerializeObject(ResultFavoringNew)}"
            }
        );
        Assert.That(jointDic, Is.EqualTo(ResultFavoringNew));
    }

    [Test]
    public void JoinFailure_Extension() {
        Assert.Throws<ArgumentException>(() => DicOrigin.JoinDictionaries(DicOverlap, CollectionUtils.ConflictResolution.Fail));
    }

    [Test]
    public void JoinFailure_Collection() {
        Assert.Throws<ArgumentException>(() => CollectionUtils.JoinDictionaries(new[] { DicOrigin, DicOverlap }, CollectionUtils.ConflictResolution.Fail));
    }

    [TestCase(CollectionUtils.ConflictResolution.FavorNew)]
    [TestCase(CollectionUtils.ConflictResolution.FavorOriginal)]
    public void JoiningDoesNotModify_Extension(CollectionUtils.ConflictResolution conflictResolution) {
        //calling "new Dictionary<>" isn't strictly required but I'm including it because I keep confusing myself with whether these things like DicOrigin are properties or methods or auto-properties or whatever so this way it won't break when I inevitable change them
        var dicOrigin  = new Dictionary<int, int>(DicOrigin);
        var dicOverlap = new Dictionary<int, int>(DicOverlap);

        dicOrigin.JoinDictionaries(dicOverlap, conflictResolution);

        Console.WriteLine(
            new[] {
                $"{nameof(dicOrigin)}:  {dicOrigin.JoinString()}",
                $"{nameof(DicOrigin)}:  {DicOrigin.JoinString()}",
                $"{nameof(dicOverlap)}: {dicOverlap.JoinString()}",
                $"{nameof(DicOverlap)}: {DicOverlap.JoinString()}"
            }
        );

        Assert.That(dicOrigin,  Is.EqualTo(DicOrigin));
        Assert.That(dicOverlap, Is.EqualTo(DicOverlap));
    }

    [TestCase(CollectionUtils.ConflictResolution.FavorNew)]
    [TestCase(CollectionUtils.ConflictResolution.FavorOriginal)]
    public void JoiningDoesNotModify_Collection(CollectionUtils.ConflictResolution conflictResolution) {
        //calling "new Dictionary<>" isn't strictly required but I'm including it because I keep confusing myself with whether these things like DicOrigin are properties or methods or auto-properties or whatever so this way it won't break when I inevitable change them
        var dicOrigin  = new Dictionary<int, int>(DicOrigin);
        var dicOverlap = new Dictionary<int, int>(DicOverlap);

        CollectionUtils.JoinDictionaries(new[] { dicOrigin, dicOverlap }, conflictResolution);

        Console.WriteLine(
            new[] {
                $"{nameof(dicOrigin)}:  {dicOrigin.JoinString()}",
                $"{nameof(DicOrigin)}:  {DicOrigin.JoinString()}",
                $"{nameof(dicOverlap)}: {dicOverlap.JoinString()}",
                $"{nameof(DicOverlap)}: {DicOverlap.JoinString()}"
            }.JoinLines()
        );

        Assert.That(dicOrigin,  Is.EqualTo(DicOrigin));
        Assert.That(dicOverlap, Is.EqualTo(DicOverlap));
    }

    #endregion

    #region Copy

    [Test]
    public void Copy_Array() {
        var original = new int[] { 1, 2, 3 };
        var dupe     = original.Copy();

        Asserter.Against(dupe)
                .And(Is.EqualTo(original))
                .And(Is.Not.SameAs(original))
                .Invoke();

        original[0]           = 99;
        dupe[dupe.Length - 1] = -99;

        Asserter.Against(dupe)
                .And(original, Is.EqualTo(new[] { 99, 2, 3 }))
                .And(Is.EqualTo(new[] { 1, 2, -99 }))
                .And(Is.Not.EqualTo(original))
                .And(Is.Not.SameAs(original))
                .Invoke();
    }

    #endregion

    #region Finding

    [Test]
    public void FindFirst() {
        Asserter.WithHeading("Find First")
                .And(() => _findFirst(new[] { 1, 2, 3 },            1))
                .And(() => _findFirst(Array.Empty<int>(),           default))
                .And(() => _findFirst(new int?[] { null, 1, 2 },    null))
                .And(() => _findFirst(Enumerable.Empty<string>(),   default))
                .And(() => _findFirst(new int?[] { 1, 2, null, 3 }, it => it <= 0,     default))
                .And(() => _findFirst(new[] { 1, 2, 3 },            it => it.IsEven(), 2))
                .Invoke();
    }

    private static void _findFirst<T>(IEnumerable<T> actual, Optional<T> expected) {
        Assert.That(actual.FindFirst(), Is.EqualTo(expected));
    }

    private static void _findFirst<T>(IEnumerable<T> actual, Func<T, bool> predicate, Optional<T> expected) {
        Assert.That(actual.FindFirst(predicate), Is.EqualTo(expected));
    }

    #endregion
}