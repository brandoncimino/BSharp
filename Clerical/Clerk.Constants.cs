using System.Collections.Immutable;

using FowlFever.BSharp.Clerical;

namespace FowlFever.Clerical;

public static partial class Clerk {
    #region Constants

    /// <summary>
    /// The valid <see cref="DirectorySeparator"/>s; <b><c>/</c></b> and <b><c>\</c></b>.
    /// </summary>
    public static readonly ImmutableArray<char> DirectorySeparatorChars = ImmutableArray.Create('\\', '/');

    /// <summary>
    /// Combines <see cref="Path.GetInvalidPathChars"/> and <see cref="Path.GetInvalidFileNameChars"/>.
    /// </summary>
    /// <remarks>
    /// Semicolon <c>:</c> is explicitly excluded from this list because it has a special rule where it is allowed <i>inside the <see cref="Path.GetPathRoot(System.ReadOnlySpan{char})"/></i>,
    /// and we don't want to bother distinguishing the root for methods like <see cref="SplitPath(string?)"/>.
    /// </remarks>
    public static readonly ImmutableArray<char> InvalidPathChars = Enumerable.Union(Path.GetInvalidPathChars(), Path.GetInvalidFileNameChars())
                                                                             .ToImmutableArray()
                                                                             .Remove(':');
    /// <summary>
    /// Combines <see cref="InvalidPathChars"/> and <see cref="DirectorySeparatorChars"/>.
    /// </summary>
    public static readonly ImmutableArray<char> InvalidPathPartChars = InvalidPathChars.Union(DirectorySeparatorChars).ToImmutableArray();
    /// <inheritdoc cref="InvalidPathChars"/>
    public static ImmutableArray<char> InvalidFileNameChars => InvalidPathChars;
    /// <summary>
    /// Combines <see cref="InvalidFileNameChars"/> with <c>.</c> <i>(period)</i>.
    /// </summary>
    public static ImmutableArray<char> InvalidFileNamePartChars = InvalidPathChars.Add('.');

    #endregion
}