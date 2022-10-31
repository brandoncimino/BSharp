using System.Collections.Immutable;

using FowlFever.BSharp.Memory;
using FowlFever.Clerical.Validated;

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
public readonly record struct FileExtension : IEquatable<string> {
    private static readonly ImmutableArray<char> InvalidChars = Clerk.InvalidPathChars.Add('.');

    private static readonly Func<char, bool> CharPredicate = c => InvalidChars.Contains(c) ||
                                                                  char.IsWhiteSpace(c);

    #region Common Extensions

    /// <summary>
    /// Represents <b>no</b> file extension, which usually indicates a <see cref="Directory"/> or an ugly linux-style config file like <a href="https://www.ssh.com/academy/ssh/config">ssh_config</a>.
    /// </summary>
    public static readonly FileExtension None = default;
    public static readonly FileExtension Json = new("json");
    public static readonly FileExtension Txt  = new("txt");
    public static readonly FileExtension Exe  = new("exe");
    public static readonly FileExtension Csv  = new("csv");
    public static readonly FileExtension Log  = new("log");
    public static readonly FileExtension Yaml = new("yaml");
    public static readonly FileExtension Cs   = new("cs");
    public static readonly FileExtension Html = new("html");

    #endregion

    #region Actual string value

    private readonly string? _value;

    /// <summary>
    /// This <see cref="FileExtension"/>, <i>including</i> the leading period that separates it from the file name, e.g. <c>".json"</c>.
    /// </summary>
    public string WithPeriod => _value?.AsSpan().PrependToString('.') ?? "";
    /// <summary>
    /// This <see cref="FileExtension"/>, <i>without</i> the leading period that separates it from the file name, e.g. <c>"json"</c>.
    /// </summary>
    public ReadOnlySpan<char> WithoutPeriod => _value ?? "";
    private static ReadOnlySpan<char> _GetTrimmed(ReadOnlySpan<char> extension) => extension[0] == '.' ? extension[1..] : extension;

    #endregion

    #region Constructors & factories

    /// <summary>
    /// "Primary" constructor. Actual instantiation should be done via factory methods such as <see cref="Of(string?)"/>.
    /// </summary>
    /// <remarks>
    /// TODO: It MIGHT be efficient to use <see cref="string.IsInterned"/> + <see cref="object.ReferenceEquals"/> to check if the input <paramref name="value"/> is a common extension like <see cref="Json"/>, but this requires additional benchmarking. 
    /// </remarks>
    /// <param name="value"><see cref="_value"/>, which will be <see cref="string.ToLowerInvariant"/></param>
    private FileExtension(string value) {
        _value = value.ToLowerInvariant();
    }

    private static FileExtension _Of(string? stringy, ReadOnlySpan<char> spanny) {
        spanny = _GetTrimmed(spanny);

        if (spanny.IsEmpty) {
            return None;
        }

        BadCharException.Assert(spanny, CharPredicate);
        var str = spanny.Length != stringy?.Length ? spanny.ToString() : stringy;
        return new FileExtension(str);
    }

    /// <summary>
    /// Creates a new <see cref="FileExtension"/>.
    /// </summary>
    /// <remarks>
    /// The input value may be <see cref="WithPeriod"/> <i>or</i> <see cref="WithoutPeriod"/>, i.e. <c>Of("json")</c> and <c>Of(".json")</c> are equivalent.
    /// 
    /// <see cref="string.IsNullOrEmpty"/> becomes <see cref="None"/>.
    /// </remarks>
    /// <param name="extension">the string value of the <see cref="FileExtension"/> <i>(with or without a leading period)</i></param>
    /// <returns>a new <see cref="FileExtension"/></returns>
    public static FileExtension Of(string? extension) => _Of(extension, extension);

    /// <inheritdoc cref="Of(string?)"/>
    public static FileExtension Of(ReadOnlySpan<char> extension) => _Of(default, extension);

    #endregion

    #region Equality

    /// <inheritdoc cref="object.Equals(object?)"/>
    public bool Equals(ReadOnlySpan<char> other) => _value.AsSpan().Equals(_GetTrimmed(other), StringComparison.OrdinalIgnoreCase);

    public bool Equals(string? other) => Equals(other.AsSpan());

    /// <inheritdoc/>
    /// <remarks>
    /// We know that a <see cref="FileExtension"/> is always lowercase, so we can use `Ordinal` instead of `OrdinalIgnoreCase` _(NOTE: I have no evidence to support my hypothesis that `Ordinal` is faster)_
    /// </remarks>
    public bool Equals(FileExtension other) => _value.AsSpan().Equals(other, StringComparison.Ordinal);

    public override int GetHashCode() => HashCode.Combine(_value);

    #endregion

    /// <remarks>
    /// ⚠ Returns the internal <see cref="_value"/>, which may change between <see cref="WithoutPeriod"/> and <see cref="WithPeriod"/> in future versions.
    /// </remarks>
    public override string ToString() => _value ?? "";

    public static implicit operator ReadOnlySpan<char>(FileExtension self) => self._value;
}