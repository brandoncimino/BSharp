using System.Collections.Immutable;

namespace FowlFever.Clerical;

public static partial class Clerk {
    #region Constants

    /// <summary>
    /// The valid <see cref="DirectorySeparator"/>s; <b><c>/</c></b> and <b><c>\</c></b>.
    /// </summary>
    public const string DirectorySeparatorChars = "\\/";

    /// <summary>
    /// Combines <see cref="Path.GetInvalidPathChars"/> and <see cref="Path.GetInvalidFileNameChars"/>.
    /// </summary>
    /// <remarks>
    /// Colon <c>:</c> is explicitly excluded from this list because it has a special rule where it is allowed <i>inside the <see cref="Path.GetPathRoot(System.ReadOnlySpan{char})"/></i>,
    /// and we don't want to bother distinguishing the root for methods like <see cref="SplitPath(string?)"/>.
    /// </remarks>
    public static readonly ImmutableArray<char> InvalidPathChars = Path.GetInvalidPathChars()
                                                                       .Union(Path.GetInvalidFileNameChars())
                                                                       .ToImmutableArray()
                                                                       .Remove(':');
    /// <summary>
    /// Combines <see cref="InvalidPathChars"/> and <see cref="DirectorySeparatorChars"/>.
    /// </summary>
    public static readonly ImmutableArray<char> InvalidPathPartChars = InvalidPathChars.Union(DirectorySeparatorChars).ToImmutableArray();

    #endregion
}