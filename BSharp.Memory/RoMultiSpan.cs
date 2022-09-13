using System;
using System.Text;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// A "collection" of up to <see cref="RoMultiSpan.MaxSpans"/> <see cref="ReadOnlySpan{T}"/>s.
/// </summary>
/// <remarks>
/// This "collection" has semantics similar to <a href="https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable">System.Collections.Immutable</a>, and so methods like <see cref="Add"/> will return a <b>new</b> <see cref="RoMultiSpan{T}"/>.
/// </remarks>
/// <typeparam name="T">the type of the elements in the individual <see cref="ReadOnlySpan{T}"/>s</typeparam>
public readonly ref partial struct RoMultiSpan<T> {
    /// <inheritdoc cref="SpanCount"/>
    [UsedImplicitly]
    [ValueRange(0, RoMultiSpan.MaxSpans)]
    public int Count => SpanCount;

    /// <summary>
    /// <c>true</c> if <see cref="SpanCount"/> &gt; 0.
    /// </summary>
    public bool HasSpans => SpanCount != 0;

    /// <summary>
    /// <c>true</c> if <see cref="ElementCount"/> &gt; 0.
    /// </summary>
    public bool HasElements => HasSpans &&
                               _a.Length > 0 ||
                               _b.Length > 0 ||
                               _c.Length > 0 ||
                               _d.Length > 0 ||
                               _e.Length > 0 ||
                               _f.Length > 0 ||
                               _g.Length > 0 ||
                               _h.Length > 0;

    /// <summary>
    /// The number of <b><see cref="ReadOnlySpan{T}"/>s</b> in this <see cref="RoMultiSpan{T}"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="Count"/> property is supported for consistency with standard collections and for <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges#implicit-range-support">implicit Range and Index support</a>,
    /// but it is generally better to use the explicit <see cref="SpanCount"/> to prevent confusion with <see cref="ElementCount"/>. 
    /// </remarks>
    [ValueRange(0, RoMultiSpan.MaxSpans)]
    public int SpanCount { get; private init; }

    /// <summary>
    /// The total <see cref="ReadOnlySpan{T}.Length"/> of all of the contained <see cref="ReadOnlySpan{T}"/>s.
    /// </summary>
    [NonNegativeValue]
    public int ElementCount => _a.Length +
                               _b.Length +
                               _c.Length +
                               _d.Length +
                               _e.Length +
                               _f.Length +
                               _g.Length +
                               _h.Length;

    #region Storage

    private ReadOnlySpan<T> _a { get; init; }
    private ReadOnlySpan<T> _b { get; init; }
    private ReadOnlySpan<T> _c { get; init; }
    private ReadOnlySpan<T> _d { get; init; }
    private ReadOnlySpan<T> _e { get; init; }
    private ReadOnlySpan<T> _f { get; init; }
    private ReadOnlySpan<T> _g { get; init; }
    private ReadOnlySpan<T> _h { get; init; }

    #endregion

    /// <summary>
    /// Retrieves a subset of this <see cref="RoMultiSpan{T}"/>'s <see cref="ReadOnlySpan{T}"/>s.
    /// </summary>
    /// <param name="start">the <see cref="GetSpan"/> to start from</param>
    /// <param name="length">the number of <see cref="ReadOnlySpan{T}"/>s to include in the result</param>
    /// <returns>a new <see cref="RoMultiSpan{T}"/></returns>
    /// <remarks>
    /// The <see cref="Slice"/> method, in combination with the <see cref="Count"/> property, enables <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges#implicit-range-support">implicit Range support</a>.
    /// </remarks>
    [Pure]
    public RoMultiSpan<T> Slice([ValueRange(0, RoMultiSpan.MaxSpans)] int start, [ValueRange(0, RoMultiSpan.MaxSpans)] int length) {
        RoMultiSpan<T> result = default;

        for (int i = start; i < start + length; i++) {
            result = result.Add(this[i]);
        }

        return result;
    }

    #region Adding more spans

    [Pure] public static RoMultiSpan<T> operator +(RoMultiSpan<T> multiSpan, ReadOnlySpan<T> newSpan)   => multiSpan.Add(newSpan);
    [Pure] public static RoMultiSpan<T> operator +(RoMultiSpan<T> multiSpan, RoMultiSpan<T>  moreSpans) => multiSpan.AddRange(moreSpans);

    [Pure]
    public RoMultiSpan<T> Add(ReadOnlySpan<T> newSpan) {
        if (SpanCount >= RoMultiSpan.MaxSpans) {
            throw new InvalidOperationException($"Cannot add a new {nameof(ReadOnlySpan<T>)} to this {nameof(RoMultiSpan<T>)}: it already contains {RoMultiSpan.MaxSpans} spans!");
        }

        return SpanCount switch {
            0 => this with { _a = newSpan, SpanCount = SpanCount + 1 },
            1 => this with { _b = newSpan, SpanCount = SpanCount + 1 },
            2 => this with { _c = newSpan, SpanCount = SpanCount + 1 },
            3 => this with { _d = newSpan, SpanCount = SpanCount + 1 },
            4 => this with { _e = newSpan, SpanCount = SpanCount + 1 },
            5 => this with { _f = newSpan, SpanCount = SpanCount + 1 },
            6 => this with { _g = newSpan, SpanCount = SpanCount + 1 },
            7 => this with { _h = newSpan, SpanCount = SpanCount + 1 },
            _ => throw new InvalidOperationException("This code should be unreachable!")
        };
    }

    [Pure]
    public RoMultiSpan<T> AddRange(RoMultiSpan<T> newSpans) {
        if (SpanCount + newSpans.SpanCount > RoMultiSpan.MaxSpans) {
            throw new ArgumentOutOfRangeException(nameof(newSpans), $"Cannot add {newSpans.SpanCount} spans to this {nameof(RoMultiSpan<T>)}: it already contains {nameof(SpanCount)} spans, and {SpanCount + newSpans.SpanCount} would exceed the {nameof(RoMultiSpan.MaxSpans)} limit of {RoMultiSpan.MaxSpans}!");
        }

        var builder = ToBuilder();
        builder.AddRange(newSpans);

        return builder.Build();
    }

    #endregion

    #region Removing spans

    public ReadOnlySpan<T> RemoveAt(int spanIndex, out ReadOnlySpan<T> removed) {
        var result = this[..spanIndex];
        throw new NotImplementedException();
    }

    #endregion

    #region Equality

    private static bool Op_Equality(RoMultiSpan<T> a, RoMultiSpan<T> b) {
        if (a.SpanCount != b.SpanCount) {
            return false;
        }

        for (int i = 0; i < a.SpanCount; i++) {
            if (a[i] != b[i]) {
                return false;
            }
        }

        return true;
    }

    [Pure] public static bool operator ==(RoMultiSpan<T> a, RoMultiSpan<T> b) => Op_Equality(a, b);
    [Pure] public static bool operator !=(RoMultiSpan<T> a, RoMultiSpan<T> b) => !(a == b);

    #endregion

    #region Object methods

    /// <inheritdoc cref="object.GetType"/>
    [Pure]
    public new Type GetType() => typeof(RoMultiSpan<T>);

    /// <inheritdoc cref="ReadOnlySpan{T}.Equals(object?)"/>
    [Obsolete($"{nameof(RoMultiSpan<T>)}.{nameof(Equals)}() will always throw an exception. Use the equality operator instead. (See {nameof(ReadOnlySpan<T>)}.{nameof(ReadOnlySpan<T>.Equals)}())")]
    public override bool Equals(object? obj) {
        return _a.Equals(obj);
    }

    /// <inheritdoc cref="ReadOnlySpan{T}.GetHashCode"/>
    [Obsolete($"{nameof(ReadOnlySpan<T>)}.{nameof(ReadOnlySpan<T>.GetHashCode)}() will always throw an exception. (See {nameof(ReadOnlySpan<T>)}.{nameof(ReadOnlySpan<T>.GetHashCode)}())")]
    public override int GetHashCode() {
        return _a.GetHashCode();
    }

    [Pure]
    public override string ToString() {
        var sb = new StringBuilder();
        sb.Append(nameof(RoMultiSpan<T>))
          .Append($"<{typeof(T).Name}>[");

        for (int i = 0; i < SpanCount; i++) {
            if (i > 0) {
                sb.Append(", ");
            }

            sb.Append(this[i].Length);
        }

        sb.Append(']');
        return sb.ToString();
    }

    #endregion
}