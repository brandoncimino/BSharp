using System.Collections.Immutable;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <b>single section</b> <i>(i.e., without any <see cref="Clerk.DirectorySeparatorChars"/>)</i> of a <see cref="FileSystemInfo.FullPath"/>, such as a <see cref="FileSystemInfo.Name"/>.
/// </summary>
public readonly partial record struct PathPart
#if NET7_0_OR_GREATER
    :
        System.Numerics.IAdditionOperators<PathPart, PathPart, DirectoryPath>,
        System.Numerics.IAdditionOperators<PathPart, FileName, FilePath>,
        System.Numerics.IAdditionOperators<PathPart, DirectoryPath, DirectoryPath>,
        System.Numerics.IAdditionOperators<PathPart, FileExtension, FileName>
#endif
{
    private readonly StringSegment _value;
    public           int           Length => _value.Length;

    #region Constructors & Factories

    private PathPart(StringSegment value) {
        _value = value;
    }

    /// <summary>
    /// Creates a <see cref="PathPart"/> <b>without any validation</b>.
    /// </summary>
    /// <remarks>
    /// While this is sorta redundant with the basic constructor, it's included for clarity and explicitness.
    /// Basically, it's easy to call a constructor willy-nilly, but the word "Unsafe" should make you pause.
    /// <br/>
    /// It also matches the corresponding methods in other classes, like <see cref="FileName"/>.<see cref="FileName.CreateUnsafe"/>, which aren't the same as their constructors.
    /// </remarks>
    internal static PathPart CreateUnsafe(StringSegment pathPart) {
        return new PathPart(pathPart);
    }

    /// <summary>
    /// Constructs a new <see cref="PathPart"/>.
    /// </summary>
    /// <param name="partString">the <see cref="string"/> of the new <see cref="PathPart"/></param>
    /// <returns>the new <see cref="PathPart"/></returns>
    public static PathPart Of(string partString) {
        return Parse(partString);
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