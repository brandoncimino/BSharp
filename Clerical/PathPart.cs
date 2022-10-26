using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Strings;
using FowlFever.Clerical.Validated;
using FowlFever.Implementors;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <b>single section</b> <i>(i.e., without any <see cref="Clerk.DirectorySeparatorChars"/>)</i> of a <see cref="FileSystemInfo.FullPath"/>, such as a <see cref="FileSystemInfo.Name"/>.
/// </summary>
public readonly record struct PathPart {
    /// <summary>
    /// The <see cref="Value"/> of a <c>default(PathPart)</c>, which corresponds to <see cref="SpecialPathPart.CurrentDirectory"/>.
    /// </summary>
    private const string DefaultPart = ".";

    [MaybeNull] private readonly string _value;
    public                       string Value  => _value ?? DefaultPart;
    public                       int    Length => Value.Length;

    private PathPart(string value) {
        _value = value;
    }

    private static ReadOnlySpan<char> TrimDirectorySeparators(ReadOnlySpan<char> pathPart) {
        return pathPart.Trim(Clerk.DirectorySeparatorChars.AsSpan());
    }

    private static string TrimDirectorySeparators(string pathPart) {
        var trimmed = TrimDirectorySeparators(pathPart.AsSpan());
        return pathPart.Length == trimmed.Length ? pathPart : trimmed.ToString();
    }

    public int CompareTo(string? other, StringComparison comparisonType = StringComparison.Ordinal)                          => string.Compare(Value, other, comparisonType);
    public int CompareTo<T>(T?   other, StringComparison comparisonType = StringComparison.Ordinal) where T : IHas<string?>? => CompareTo(other?.Value, comparisonType);

    public override string ToString() => Value;
    public          bool   IsEmpty    => Value.IsEmpty();

    #region SpecialPathParts

    public static readonly PathPart CurrentDirectory = default; //new(".");
    public static readonly PathPart ParentDirectory  = new("..");
    public static readonly PathPart HomeDirectory    = new("~");

    public static SpecialPathPart? GetSpecialPathPart(ReadOnlySpan<char> pathPart) {
        return pathPart switch {
            { Length: 1 } when pathPart[0] == '.'        => SpecialPathPart.CurrentDirectory,
            { Length: 1 } when pathPart[0] == '~'        => SpecialPathPart.HomeDirectory,
            { Length: 2 } when pathPart.StartsWith("..") => SpecialPathPart.ParentDirectory,
            _                                            => null,
        };
    }

    /// <param name="pathPart">a string that might be a <see cref="SpecialPathPart"/></param>
    /// <returns><c>true</c> if the given <paramref name="pathPart"/> is one of the <see cref="SpecialPathPart"/>s</returns>
    public static bool IsSpecialPathPart(ReadOnlySpan<char> pathPart) {
        return (pathPart.Length    == 2 && pathPart[0] == '.' && pathPart[1] == '.')
               || (pathPart.Length == 1 && (pathPart[0] == '.' || pathPart[0] == '~'));
    }

    #endregion

    #region Conversions

    public static implicit operator PathPart(SpecialPathPart    specialPathPart) => specialPathPart.ToPathPart();
    public static implicit operator PathPart(ReadOnlySpan<char> part)            => Of(part);

    public                          ReadOnlySpan<char> AsSpan()       => Value;
    public static implicit operator ReadOnlySpan<char>(PathPart part) => part.AsSpan();

    #endregion

    public static ValueArray<PathPart> operator +(PathPart a, PathPart b) => ValueArray.Of(a, b);
    public        Onerator<PathPart> GetEnumerator() => new(this);

    /// <summary>
    /// Constructs a new <see cref="PathPart"/>.
    /// </summary>
    /// <param name="partString">the <see cref="Value"/> of the new <see cref="PathPart"/></param>
    /// <returns>the new <see cref="PathPart"/></returns>
    public static PathPart Of(ReadOnlySpan<char> partString) => _Of(TrimDirectorySeparators(partString).ToString());

    /// <inheritdoc cref="Of(System.ReadOnlySpan{char})"/>
    public static PathPart Of(string partString) => _Of(TrimDirectorySeparators(partString));

    private static PathPart _Of(string partString) {
        return partString.Length switch {
            0                                                   => default,
            1 when partString[0] == '.'                         => CurrentDirectory,
            1 when partString[0] == '~'                         => HomeDirectory,
            2 when partString[0] == '.' && partString[0] == '.' => ParentDirectory,
            _                                                   => new PathPart(JustValidate(partString))
        };
    }

    private static string JustValidate(string partString) {
        var exc = _tryValidate(partString);
        if (exc != null) {
            throw exc;
        }

        return partString;
    }

    private static Exception? _tryValidate(ReadOnlySpan<char> pathPart) {
        if (GetSpecialPathPart(pathPart).HasValue) {
            return default;
        }

        return Must.Try(pathPart.IsEmpty, false, "cannot be empty") ??
               Must.Try(pathPart[0].IsWhitespace(), false, "cannot start with whitespace") ??
               Must.Try(pathPart[^1].IsWhitespace(), false, "cannot end with whitespace") ??
               Must.Try(pathPart[^1] != '.', true, $"cannot end in a period (unless the entire {nameof(PathPart)} is '.' or '..')") ??
               Must.Try(pathPart.Length == 1 && pathPart[0] == ':', false, "cannot be a single colon (colons are special thingies for drive indicators)") ??
               Must.Try(pathPart.IndexOf("..") < 0, true, details: $"cannot contain double-periods ('..') (unless the entire {nameof(PathPart)} is '..')") ??
               BadCharException.TryAssert(pathPart, Clerk.InvalidPathPartChars.AsSpan())
            ;
    }
}