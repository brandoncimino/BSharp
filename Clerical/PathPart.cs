using System.Collections.Immutable;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <b>single section</b> <i>(i.e., without any <see cref="Clerk.DirectorySeparatorChars"/>)</i> of a <see cref="FileSystemInfo.FullPath"/>, such as a <see cref="FileSystemInfo.Name"/>.
/// </summary>
public readonly partial struct PathPart :
    IEquatable<PathPart>
#if NET7_0_OR_GREATER
    ,
    IEqualityOperators<PathPart, PathPart, bool>,
    IAdditionOperators<PathPart, PathPart, DirectoryPath>,
    IAdditionOperators<PathPart, FileName, FilePath>,
    IAdditionOperators<PathPart, DirectoryPath, DirectoryPath>,
    IAdditionOperators<PathPart, FileExtension, FileName>
#endif
{
    private readonly StringSegment _value;
    public           int           Length => _value.Length;

    #region Constructors & Factories

    private PathPart(StringSegment value) {
        _value = value;
    }

    #endregion

    public override string ToString() => _value.ToString();

    #region SpecialPathParts

    public static readonly PathPart CurrentDirectory = new(".");

    public static readonly PathPart ParentDirectory = new("..");

    public static readonly PathPart HomeDirectory = new("~");

    #endregion

    #region Conversions

    public ReadOnlySpan<char> AsSpan() => _value.AsSpan();

    #endregion

    #region Equality

    public bool Equals(PathPart other) {
        return AsSpan().SequenceEqual(other.AsSpan());
    }

    public override bool Equals(object? obj) {
        return obj is PathPart other && Equals(other);
    }

    public override int GetHashCode() {
        return _value.GetHashCode();
    }

    public static bool operator ==(PathPart left, PathPart right) => left.Equals(right);
    public static bool operator !=(PathPart left, PathPart right) => !(left == right);

    #endregion

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