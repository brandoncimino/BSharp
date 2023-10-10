using System.Diagnostics;

using CommunityToolkit.HighPerformance.Buffers;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <a href="https://en.wikipedia.org/wiki/Filename_extension">file extension</a>, which always starts with a period <i>(a la <see cref="Path.GetExtension(System.ReadOnlySpan{char})"/>)</i>.
/// </summary>
public readonly partial struct FileExtension :
    IEquatable<FileExtension>,
    IEquatable<string?> {
    #region Actual string value

    internal readonly StringSegment _valueWithPeriod;

    [Pure] public int LengthWithPeriod => _valueWithPeriod.Length;

    #endregion

    #region Constructors & factories

    /// <summary>
    /// "Primary" constructor, with <b>no input validation</b> <i>(outside of <see cref="Debug"/> mode)</i>.
    /// <p/>
    /// Actual instantiation should be done via factory methods such as <see cref="Parse(System.ReadOnlySpan{char},System.IFormatProvider?)"/>.
    /// </summary>
    /// <param name="value"><see cref="_valueWithPeriod"/>, which <i>should</i> be <see cref="string.ToLowerInvariant"/></param>
    internal FileExtension(StringSegment value) {
        Debug.Assert(IsPerfectExtension(value));
        _valueWithPeriod = value;
    }

    internal static string GetOrCreateExtensionString(ReadOnlySpan<char> perfectExtensionSpan) {
        Debug.Assert(IsPerfectExtension(perfectExtensionSpan));
        return TryGetCommonExtensionString(perfectExtensionSpan, out var result) ? result : GetUncommonExtensionString(perfectExtensionSpan);
    }

    private static readonly StringPool ExtensionPool = new();

    /// <summary>
    /// This method handles "uncommon" extension strings - i.e. ones that aren't covered by <see cref="TryGetCommonExtensionString"/>.
    /// </summary>
    /// <param name="perfectExtensionSpan"></param>
    /// <returns></returns>
    private static string GetUncommonExtensionString(ReadOnlySpan<char> perfectExtensionSpan) {
        Debug.Assert(IsPerfectExtension(perfectExtensionSpan));
        Debug.Assert(TryGetCommonExtensionString(perfectExtensionSpan, out var result) == false, $"Common extensions like `{result}` should have already been handled!");
        return ExtensionPool.GetOrAdd(perfectExtensionSpan);
    }

    #endregion

    #region Equality

    [Pure] public bool Equals(FileExtension other) => AsSpan().Equals(other.AsSpan(), StringComparison.Ordinal);

    /// <inheritdoc/>
    /// <remarks>
    /// This mimics the implementation of <see cref="StringSegment"/>.<see cref="StringSegment.Equals(object?)"/>, where:
    /// <code><![CDATA[
    /// segment.Equals(segment);                        // => true
    /// segment.Equals((object)segment);                // => true
    /// segment.Equals(segment.ToString());             // => true
    /// segment.Equals((object)(segment.ToString());    // => false
    /// ]]></code>
    /// </remarks>
    [Pure]
    public override bool Equals(object? other) => other is FileExtension ext && Equals(ext);

    [Pure] public bool Equals(string? other) => IsEquivalentTo(other);

    [Pure] public static                   bool operator ==(FileExtension a, FileExtension b) => a._valueWithPeriod == b._valueWithPeriod;
    [Pure] public static                   bool operator !=(FileExtension a, FileExtension b) => !(a == b);
    [Pure] public static implicit operator FileExtension(string           s) => Parse(s);

    [Pure] public override int GetHashCode() => _valueWithPeriod.GetHashCode();

    private bool IsEquivalentTo(ReadOnlySpan<char> other) {
        if (_valueWithPeriod.Length == 0) {
            return other.IsEmpty;
        }

        return other switch {
            []        => _valueWithPeriod.Length == 0,
            ['.']     => false /* a lone period can never be a valid extension! */,
            ['.', ..] => _valueWithPeriod.AsSpan().Equals(other, StringComparison.OrdinalIgnoreCase),
            _         => _valueWithPeriod.AsSpan(1).Equals(other, StringComparison.OrdinalIgnoreCase)
        };
    }

    #endregion

    [Pure] public override string ToString() => _valueWithPeriod.ToString();

    [Pure] public ReadOnlySpan<char> AsSpan() => _valueWithPeriod.AsSpan();

    [Pure] public static FileName operator +(PathPart baseName, FileExtension extension) => new(baseName, extension);
}