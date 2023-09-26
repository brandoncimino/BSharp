using System.Runtime.CompilerServices;

using FowlFever.BSharp.Memory;

namespace BSharp.Memory.Tests;

public class SkipTakeTests {
    public enum Flavor { Span, RoSpan }

    public record SpanqFamilyMethod<T, ARG>(
        SpanTransformer<T, ARG, T>                SpanMethod,
        RoSpanTransformer<T, ARG, T>              RoSpanMethod,
        Func<IEnumerable<T>, ARG, IEnumerable<T>> EnumerableMethod,
        [CallerArgumentExpression("SpanMethod")]
        string? _MethodName = default
    ) {
        public T[] InvokeSpanMethod(T[] input, ARG arg, Flavor flavor) {
            return flavor switch {
                Flavor.Span   => SpanMethod(input, arg).ToArray(),
                Flavor.RoSpan => RoSpanMethod(input, arg).ToArray(),
                _             => throw new ArgumentOutOfRangeException(nameof(flavor), flavor, null)
            };
        }

        public override string ToString() {
            return _MethodName ?? "";
        }
    }

    public static readonly SpanqFamilyMethod<int, int>[] Methods = {
        new(Spanq.Skip, Spanq.Skip, Enumerable.Skip),
        new(Spanq.Take, Spanq.Take, Enumerable.Take),
        new(Spanq.SkipLast, Spanq.SkipLast, Enumerable.SkipLast),
        new(Spanq.TakeLast, Spanq.TakeLast, Enumerable.TakeLast),
    };

    public static int[][] Sources => new[] {
        Array.Empty<int>(),
        new[] { 1, 2, 3 }
    };

    public static int[] Args => Enumerable.Range(-5, 5).ToArray();

    [Test]
    public void Spanq_MatchesLinq(
        [ValueSource(nameof(Methods))] SpanqFamilyMethod<int, int> method,
        [ValueSource(nameof(Sources))] int[]                       source,
        [ValueSource(nameof(Args))]    int                         arg,
        [Values]                       Flavor                      flavor
    ) {
        var actual   = method.InvokeSpanMethod(source, arg, flavor);
        var expected = method.EnumerableMethod(source, arg);
        Assert.That(actual, Is.EqualTo(expected));
    }

    public static readonly Range[] Ranges = {
        ..,             // full range
        ..0,            // empty at start
        ^0..,           // empty at end
        ..1,            // first element
        ^1..,           // last element
        1..3,           // second element
        ..int.MaxValue, // (almost certainly) beyond the end
    };

    [Test]
    public void Spanq_TakeRange_MatchesLinq(
        [Values(new[] { 1, 2, 3 })]   int[]  source,
        [ValueSource(nameof(Ranges))] Range  takeRange,
        [Values]                      Flavor flavor
    ) {
        var method = new SpanqFamilyMethod<int, Range>(
            Spanq.Take,
            Spanq.Take,
            Enumerable.Take
        );
        var actual   = method.InvokeSpanMethod(source, takeRange, flavor);
        var expected = method.EnumerableMethod(source, takeRange);
        Assert.That(actual, Is.EqualTo(expected));
    }

    #region Empty results should reference specific endpoints of the source

    private static void Assert_StartOfSource<T>(ReadOnlySpan<T> actual, ReadOnlySpan<T> source) {
        var expected = source[..0];
        Assert.That(actual == expected, "Should reference an empty span at the START of the source");
    }

    private static void Assert_EndOfSource<T>(ReadOnlySpan<T> actual, ReadOnlySpan<T> source) {
        var expected = source[^0..];
        Assert.That(actual == expected, "Should reference an empty span at the END of the source");
    }

    #region Skip

    [Test]
    public void Span_Skip_IfResultIsEmpty_ReferencesEndOfSource([Values(0, 1, 2)] int countBeyondSize) {
        Span<char> source = stackalloc char[3];
        Assert_EndOfSource(source.Skip(source.Length + countBeyondSize), source.AsReadOnly());
    }

    [Test]
    public void RoSpan_Skip_IfResultIsEmpty_ReferencesEndOfSource([Values(0, 1, 2)] int countBeyondSize) {
        ReadOnlySpan<char> source = stackalloc char[3];
        Assert_EndOfSource(source.Skip(source.Length + countBeyondSize), source);
    }

    #endregion

    #region SkipLast

    [Test]
    public void Span_SkipLast_IfResultIsEmpty_ReferencesStartOfSource([Values(0, 1, 2)] int countBeyondSize) {
        Span<char> source = stackalloc char[3];
        Assert_EndOfSource(source.SkipLast(source.Length + countBeyondSize), source.AsReadOnly());
    }

    [Test]
    public void RoSpan_SkipLast_IfResultIsEmpty_ReferencesStartOfSource([Values(0, 1, 2)] int countBeyondSize) {
        ReadOnlySpan<char> source = stackalloc char[3];
        Assert_StartOfSource(source.SkipLast(source.Length + countBeyondSize), source);
    }

    #endregion

    #region Take

    [Test]
    public void Span_Take_IfResultIsEmpty_ReferencesStartOfSource([Values(-2, -1, 0)] int countToTake) {
        Span<char> source = stackalloc char[3];
        Assert_StartOfSource(source.Take(countToTake), source.AsReadOnly());
    }

    [Test]
    public void RoSpan_Take_IfResultIsEmpty_ReferencesStartOfSource([Values(-2, -1, 0)] int countToTake) {
        ReadOnlySpan<char> source = stackalloc char[3];
        Assert_StartOfSource(source.Take(countToTake), source);
    }

    #endregion

    #region TakeLast

    [Test]
    public void Span_TakeLast_IfResultIsEmpty_ReferencesEndOfSource([Values(-2, -1, 0)] int countToTake) {
        Span<char> source = stackalloc char[3];
        Assert_EndOfSource(source.TakeLast(countToTake), source.AsReadOnly());
    }

    [Test]
    public void RoSpan_TakeLast_IfResultIsEmpty_ReferencesEndOfSource([Values(-2, -1, 0)] int countToTake) {
        ReadOnlySpan<char> source = stackalloc char[3];
        Assert_EndOfSource(source.TakeLast(countToTake), source);
    }

    #endregion

    #endregion
}