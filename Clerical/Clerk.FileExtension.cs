using System.Diagnostics.Contracts;

using FowlFever.BSharp;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Memory;

namespace FowlFever.Clerical;

public static partial class Clerk {
    /// <summary>
    /// Extracts each <b>individual</b> <see cref="FileExtension"/> from a path.
    /// </summary>
    /// <param name="path">a path or file name</param>
    /// <returns><b>all</b> of the extensions at the end of the <paramref name="path"/></returns>
    [Pure]
    public static ValueArray<FileExtension> GetExtensions(ReadOnlySpan<char> path) {
        return EnumerateExtensions(path).ToImmutableArray(static span => FileExtension.Of(span));
    }

    [Pure] internal static SpanSpliterator<char> EnumerateExtensions(ReadOnlySpan<char> path) => Path.GetFileName(path).Spliterate('.') with { Options = StringSplitOptions.RemoveEmptyEntries };

    [Pure]
    public static ReadOnlySpan<char> GetFullExtension(ReadOnlySpan<char> path) {
        var fileName    = Path.GetFileName(path);
        var firstPeriod = fileName.IndexOf('.');
        return firstPeriod < 0 || firstPeriod >= (fileName.Length - 1) ? ReadOnlySpan<char>.Empty : fileName[firstPeriod..];
    }
}