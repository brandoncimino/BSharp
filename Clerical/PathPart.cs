using System.Collections.Immutable;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <b>single section</b> <i>(i.e., without any <see cref="Clerk.DirectorySeparatorChars"/>)</i> of a <see cref="FileSystemInfo.FullPath"/>, such as a <see cref="FileSystemInfo.Name"/>.
/// </summary>
public readonly record struct PathPart
#if NET7_0_OR_GREATER
    : System.Numerics.IAdditionOperators<PathPart, PathPart, DirectoryPath>,
      System.Numerics.IAdditionOperators<PathPart, FileName, FilePath>,
      System.Numerics.IAdditionOperators<PathPart, DirectoryPath, DirectoryPath>,
      System.Numerics.IAdditionOperators<PathPart, FileExtension, FileName>
#endif
{
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

    /// <summary>
    /// Constructs a new <see cref="PathPart"/>.
    /// </summary>
    /// <param name="partString">the <see cref="string"/> of the new <see cref="PathPart"/></param>
    /// <returns>the new <see cref="PathPart"/></returns>
    public static PathPart Of(string partString) {
        var str = TrimDirectorySeparators(partString);

        BadCharException.Validate(str, Clerk.DirectorySeparatorChars);

        return str.AsSpan() switch {
            "."  => CurrentDirectory,
            "~"  => HomeDirectory,
            ".." => ParentDirectory,
            _    => new PathPart(str)
        };
    }

    #region Operators

    public static DirectoryPath operator +(PathPart left, PathPart right) {
        return new DirectoryPath(ImmutableArray.Create(left, right));
    }

    public static FilePath operator +(PathPart left, FileName right) {
        return new FilePath(new DirectoryPath(ImmutableArray.Create(left)), right);
    }

    public static DirectoryPath operator +(PathPart left, DirectoryPath right) {
        return new DirectoryPath(right.Parts.Insert(0, left));
    }

    public static FileName operator +(PathPart left, FileExtension right) {
        return new FileName(left, right);
    }

    #endregion
}