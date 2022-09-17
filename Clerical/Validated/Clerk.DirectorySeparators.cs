using System.Collections.Immutable;
using System.Diagnostics.Contracts;

using FowlFever.BSharp.Clerical;

namespace FowlFever.Clerical.Validated;

public static partial class Clerk {
    /// <summary>
    /// The valid <see cref="DirectorySeparator"/>s; <b><c>/</c></b> and <b><c>\</c></b>.
    /// </summary>
    public static readonly ImmutableArray<char> DirectorySeparatorChars = ImmutableArray.Create('\\', '/');

    /// <param name="c">a <see cref="char"/></param>
    /// <returns><c>true</c> if <paramref name="c"/> is a <see cref="DirectorySeparator"/>: <c>/</c> or <c>\</c></returns>
    [Pure]
    public static bool IsDirectorySeparator(char c) => c is '\\' or '/';

    /// <remarks>
    /// Differs <b>very slightly</b> from the built-in <see cref="M:System.IO.Path.EndsInDirectorySeparator(System.ReadOnlySpan{System.Char})"/> (which isn't available in .NET Standard 2.1):
    /// this version will check for both <c>/</c> and <c>\</c> on <b>all</b> platforms (the built-in version checks for <see cref="Path.DirectorySeparatorChar"/> and <see cref="Path.AltDirectorySeparatorChar"/>, which, on Mac/Linux, are both <c>/</c>)
    /// </remarks>
    /// <param name="path">a file path</param>
    /// <returns><c>true</c> if the last <see cref="char"/> of the <paramref name="path"/> <see cref="IsDirectorySeparator"/></returns>
    [Pure]
    public static bool EndsInDirectorySeparator(ReadOnlySpan<char> path) => path.Length > 0 && IsDirectorySeparator(path[^1]);

    /// <inheritdoc cref="EndsInDirectorySeparator(System.ReadOnlySpan{char})"/>
    [Pure]
    public static bool EndsInDirectorySeparator(string? path) => EndsInDirectorySeparator(path.AsSpan());
}