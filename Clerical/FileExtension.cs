using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

using FowlFever.BSharp.Memory;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <a href="https://en.wikipedia.org/wiki/Filename_extension">file extension</a>, which:
/// <ul>
/// <li>Cannot contain <see cref="char.IsWhiteSpace(char)"/></li>
/// <li>Cannot contain <see cref="Path.GetInvalidFileNameChars"/></li>
/// <li>Cannot contain a period (unless it is the first character)</li>
/// </ul>
/// </summary>
/// <remarks>
/// Internally, the actual <see cref="string"/> <see cref="WithPeriod"/> of the extension is stored <i>without</i> a period.
/// This allows us to create a <see cref="FileExtension"/> from a <see cref="string"/> with or without a period without allocating anything.
///
/// Special cases:
/// <ul>
/// <li><i>(On Windows)</i> If a file name ends with a period, then the period is stripped, i.e. <c>"file."</c> becomes <c>"file"</c>. In other words, there is no "empty" extension, but there is "no" extension.</li>
/// </ul>
/// </remarks>
public readonly partial struct FileExtension : IEquatable<FileExtension> {
    #region Actual string value

    [MaybeNull] private readonly string _value;

    /// <summary>
    /// This <see cref="FileExtension"/>, <i>including</i> the leading period that separates it from the file name, e.g. <c>".json"</c>.
    /// </summary>
    [Pure]
    public string WithPeriod => _value?.AsSpan().PrependToString('.') ?? "";
    /// <summary>
    /// This <see cref="FileExtension"/>, <i>without</i> the leading period that separates it from the file name, e.g. <c>"json"</c>.
    /// </summary>
    [Pure]
    public ReadOnlySpan<char> WithoutPeriod => _value ?? "";

    [Pure] public int Length => _value?.Length ?? 0;

    #endregion

    #region Constructors & factories

    /// <summary>
    /// "Primary" constructor, with <b>no input validation</b>.
    /// <br/>
    /// Actual instantiation should be done via factory methods such as <see cref="Parse(System.ReadOnlySpan{char})"/>.
    /// </summary>
    /// <param name="value"><see cref="_value"/>, which will be <see cref="string.ToLowerInvariant"/></param>
    private FileExtension(string value) {
        _value = value;
    }

    #endregion

    #region Equality

    [Pure] public          bool Equals(FileExtension other) => _value == other._value;
    [Pure] public override bool Equals(object?       other) => Equals(_value, other);

    [Pure] public static bool operator ==(FileExtension a, FileExtension b) => string.Equals(a._value, b._value);
    [Pure] public static bool operator !=(FileExtension a, FileExtension b) => !(a == b);

    [Pure]
    public override int GetHashCode() {
#if NET7_0_OR_GREATER
        return string.GetHashCode(_value);
#else
        return (_value ?? "").GetHashCode();
#endif
    }

    #endregion

    /// <remarks>
    /// ⚠ Returns the internal <see cref="_value"/>, which may change between <see cref="WithoutPeriod"/> and <see cref="WithPeriod"/> in future versions.
    /// </remarks>
    [Pure]
    public override string ToString() => _value ?? "";

    [Pure] public static implicit operator ReadOnlySpan<char>(FileExtension self) => self._value;

    [Pure] public static FileName operator +(PathPart baseName, FileExtension extension) => new(baseName, extension);
}