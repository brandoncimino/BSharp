using System.Diagnostics.Contracts;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <a href="https://en.wikipedia.org/wiki/Filename_extension">file extension</a>, which always starts with a period and is shorter than <see cref="MaxExtensionLengthIncludingPeriod"/>.
/// </summary>
public readonly partial struct FileExtension : IEquatable<FileExtension> {
    #region Actual string value

    private readonly Substring _value;

    [Pure] public int Length => _value.Length;

    #endregion

    #region Constructors & factories

    /// <summary>
    /// "Primary" constructor, with <b>no input validation</b>.
    /// <br/>
    /// Actual instantiation should be done via factory methods such as <see cref="Parse(System.ReadOnlySpan{char})"/>.
    /// </summary>
    /// <param name="value"><see cref="_value"/>, which will be <see cref="string.ToLowerInvariant"/></param>
    internal FileExtension(Substring value) {
        _value = value;
    }

    #endregion

    #region Equality

    [Pure] public          bool Equals(FileExtension other) => _value == other._value;
    [Pure] public override bool Equals(object?       other) => Equals(_value, other);

    [Pure] public static bool operator ==(FileExtension a, FileExtension b) => a._value == b._value;
    [Pure] public static bool operator !=(FileExtension a, FileExtension b) => !(a == b);

    [Pure] public override int GetHashCode() => _value.GetHashCode();

    #endregion

    [Pure] public override string ToString() => _value.ToString();

    [Pure] public                          ReadOnlySpan<char> AsSpan()                 => _value;
    [Pure] public static implicit operator ReadOnlySpan<char>(FileExtension self)      => self.AsSpan();
    [Pure] public static implicit operator FileExtension(string             extension) => Parse(extension);

    [Pure] public static FileName operator +(PathPart baseName, FileExtension extension) => new(baseName, extension);
}