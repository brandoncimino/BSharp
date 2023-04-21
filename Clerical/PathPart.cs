using FowlFever.BSharp;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.Clerical.Validated;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <b>single section</b> <i>(i.e., without any <see cref="Clerk.DirectorySeparatorChars"/>)</i> of a <see cref="FileSystemInfo.FullPath"/>, such as a <see cref="FileSystemInfo.Name"/>.
/// </summary>
public readonly record struct PathPart {
    private readonly Substring _value;
    public           int       Length => _value.Length;

    internal PathPart(Substring value) {
        _value = value;
    }

    private static Substring TrimDirectorySeparators(string pathPart) {
        var trimmed = pathPart.AsSpan().Trim(Clerk.DirectorySeparatorChars);
        if (trimmed.IsEmpty) {
            return default;
        }

        if (trimmed.Length == pathPart.Length) {
            return pathPart;
        }

        return Substring.CreateFromSpan(trimmed, pathPart);
    }

    public override string ToString() => _value.ToString();

    #region SpecialPathParts

    public static readonly PathPart CurrentDirectory = new(".");
    public static readonly PathPart ParentDirectory  = new("..");
    public static readonly PathPart HomeDirectory    = new("~");

    public static SpecialPathPart? GetSpecialPathPart(ReadOnlySpan<char> pathPart) {
        return pathPart switch {
            "."  => SpecialPathPart.CurrentDirectory,
            "~"  => SpecialPathPart.HomeDirectory,
            ".." => SpecialPathPart.ParentDirectory,
            _    => null,
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

    public static implicit operator PathPart(SpecialPathPart specialPathPart) => specialPathPart.ToPathPart();
    public static implicit operator PathPart(string          partString)      => Of(partString);

    public                          ReadOnlySpan<char> AsSpan()       => _value;
    public static implicit operator ReadOnlySpan<char>(PathPart part) => part.AsSpan();

    #endregion

    public static ValueArray<PathPart> operator +(PathPart a,        PathPart      b)         => ValueArray.Of(a, b);
    public static FileName operator +(PathPart             baseName, FileExtension extension) => new(baseName, extension);

    /// <summary>
    /// Constructs a new <see cref="PathPart"/>.
    /// </summary>
    /// <param name="partString">the <see cref="string"/> of the new <see cref="PathPart"/></param>
    /// <returns>the new <see cref="PathPart"/></returns>
    public static PathPart Of(string partString) {
        var str = TrimDirectorySeparators(partString);

        BadCharException.Assert(str, Clerk.DirectorySeparatorChars);

        return str.AsSpan() switch {
            "."  => CurrentDirectory,
            "~"  => HomeDirectory,
            ".." => ParentDirectory,
            _    => new PathPart(str)
        };
    }

    /// <summary>
    /// Exhaustively validates that the given <see cref="ReadOnlySpan{T}"/> is a valid <see cref="PathPart"/> string.
    /// </summary>
    /// <param name="partString"></param>
    /// <exception cref="Exception"></exception>
    internal static void Validate(ReadOnlySpan<char> partString) {
        var exc = _tryValidate(partString);
        if (exc != null) {
            throw exc;
        }
    }

    private static Exception? _tryValidate(ReadOnlySpan<char> pathPart) {
        if (GetSpecialPathPart(pathPart).HasValue) {
            return default;
        }

        return Must.Try(pathPart.IsEmpty, false, "cannot be empty") ??
               Must.Try(pathPart[0].IsWhitespace(), false, "cannot start with whitespace") ??
               Must.Try(pathPart[^1].IsWhitespace(), false, "cannot end with whitespace") ??
               Must.Try(pathPart[^1] != '.', true, $"cannot end in a period (unless the entire {nameof(PathPart)} is '.' or '..')") ??
               Must.Try(pathPart is ":", false, "cannot be a single colon (colons are special thingies for drive indicators)") ??
               Must.Try(pathPart.IndexOf("..") < 0, true, details: $"cannot contain double-periods ('..') (unless the entire {nameof(PathPart)} is '..')") ??
               BadCharException.TryAssert(pathPart, Clerk.InvalidPathPartChars.AsSpan())
            ;
    }
}