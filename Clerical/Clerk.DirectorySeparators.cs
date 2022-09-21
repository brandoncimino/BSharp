using System.Diagnostics.Contracts;

using FowlFever.BSharp.Clerical;
using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Strings;

namespace FowlFever.Clerical;

public static partial class Clerk {
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

    /// <summary>
    /// Appends a <see cref="DirectorySeparator"/> to a <paramref name="path"/> if it doesn't end in one.
    /// </summary>
    /// <remarks>
    /// If the <paramref name="path"/> ends in a different <see cref="DirectorySeparator"/>, then that separator is <b>not modified</b>:
    /// <code><![CDATA[
    /// EnsureEndingDirectorySeparator("a",  DirectorySeparator.Universal); // => a/
    /// EnsureEndingDirectorySeparator("a/", DirectorySeparator.Universal); // => a/
    /// EnsureEndingDirectorySeparator("a\", DirectorySeparator.Universal); // => a\
    /// ]]></code>
    /// </remarks>
    /// <param name="path">the <see cref="FileSystemInfo.OriginalPath"/></param>
    /// <param name="separator">the <see cref="DirectorySeparator"/> that will be added if the <paramref name="path"/> doesn't already end in one</param>
    /// <returns>the <paramref name="path"/>, ending with a <see cref="DirectorySeparator"/></returns>
    [Pure]
    public static ReadOnlySpan<char> EnsureEndingDirectorySeparator(ReadOnlySpan<char> path, DirectorySeparator separator = DirectorySeparator.Universal) {
        if (path.Length == 0) {
            return separator.ToCharString();
        }

        if (path[^1].IsDirectorySeparator()) {
            return path;
        }

        return stackalloc char[path.Length + 1].Start(path, out var pos)
                                               .Write(separator.ToChar(), ref pos)
                                               .ToString();
    }

    /// <inheritdoc cref="EnsureEndingDirectorySeparator(System.ReadOnlySpan{char},FowlFever.BSharp.Clerical.DirectorySeparator)"/>
    [Pure]
    public static string EnsureEndingDirectorySeparator(string? path, DirectorySeparator separator = DirectorySeparator.Universal) {
        if (path == null) {
            return separator.ToCharString();
        }

        var str = EnsureEndingDirectorySeparator(path.AsSpan(), separator);
        return str.Length == path.Length ? path : str.ToString();
    }
}