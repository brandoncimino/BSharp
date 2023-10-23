using System.Diagnostics;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <a href="https://en.wikipedia.org/wiki/Filename_extension">file extension</a>, which always starts with a period <i>(a la <see cref="Path.GetExtension(System.ReadOnlySpan{char})"/>)</i>.
/// </summary>
public readonly partial struct FileExtension :
    IEquatable<FileExtension>,
    IEquatable<string?>
#if NET7_0_OR_GREATER
    ,
    IEqualityOperators<FileExtension, FileExtension, bool>
#endif
{
    #region Actual string value

    private readonly StringSegment _valueWithPeriod;

    [Pure] public int LengthWithPeriod => _valueWithPeriod.Length;

    #endregion

    #region Constructors & factories

    /// <summary>
    /// "Primary" constructor, with <b>no input validation</b> <i>(outside of <see cref="Debug"/> mode)</i>.
    /// <p/>
    /// Actual instantiation should be done via factory methods such as <see cref="Parse(System.ReadOnlySpan{char},System.IFormatProvider?)"/>.
    /// </summary>
    /// <param name="value"><see cref="_valueWithPeriod"/>, which <i>should</i> be <see cref="string.ToLowerInvariant"/></param>
    private FileExtension(StringSegment value) {
        DebugAssert_PerfectExtension(value);
        _valueWithPeriod = value;
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

    /// <inheritdoc cref="IsEquivalentTo"/>
    [Pure]
    public bool Equals(string? other) => IsEquivalentTo(other);

    [Pure] public static bool operator ==(FileExtension a, FileExtension b) => a._valueWithPeriod.AsSpan().SequenceEqual(b._valueWithPeriod);
    [Pure] public static bool operator !=(FileExtension a, FileExtension b) => !(a == b);

    // TODO: Should I include explicit == operator overloads to use the more efficient `Equals(string)`?
    //  ➕ Avoids any potential allocations
    //  ➕ Avoids needing to validate the `string` 
    //  ➕ It makes perfect sense to me
    //  ➖ Nobody else in the world seems to do it
    // TODO: Update from Brandon on 10/12/2023 - in preference of least-surprise > convenience, I'm preferring `IsEquivalentTo` for equality with unparsed stuff.
    // [Pure] public static                   bool operator ==(FileExtension a, string        b) => a.Equals(b);
    // public static                          bool operator !=(FileExtension a, string        b) => !(a == b);
    // [Pure] public static                   bool operator ==(string        a, FileExtension b) => b.Equals(a);
    // public static                          bool operator !=(string        a, FileExtension b) => !(a == b);

    [Pure] public override int GetHashCode() => _valueWithPeriod.GetHashCode();

    /// <summary>
    /// Determines if the given <see cref="string"/> is equivalent to this <see cref="FileExtension"/>, ignoring case and at most one leading period.
    /// </summary>
    /// <param name="other">some <see cref="string"/></param>
    /// <returns><c>true</c> if the given string is equivalent to this <see cref="FileExtension"/></returns>
    /// <example>
    /// <code><![CDATA[
    /// var json = FileExtension.Parse(".json");
    /// json.Equals(".json");   // => true
    /// json.Equals("json");    // => true
    /// json.Equals("JSON");    // => true
    ///
    /// // "aliases" are also handled:
    /// var jpeg = FileExtension.Parse(".jpeg");
    /// jpeg.Equals("JPG");     // => true
    /// ]]></code>
    /// </example>
    public bool IsEquivalentTo(ReadOnlySpan<char> other) {
        if (_valueWithPeriod.Length == 0) {
            return other.IsEmpty;
        }

        if (TryGetCommonExtensionString(other, out var result)) {
            // "common" extension strings are ALWAYS lowercase, so we can speed things up by using `StringComparison.Ordinal`
            return _valueWithPeriod.AsSpan().Equals(result, StringComparison.Ordinal);
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
}