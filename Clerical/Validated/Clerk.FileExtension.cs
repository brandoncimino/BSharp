using System.Collections.Immutable;
using System.Diagnostics.Contracts;

using FowlFever.BSharp.Memory;
using FowlFever.BSharp.Strings;
using FowlFever.Clerical.Validated.Atomic;

namespace FowlFever.Clerical.Validated;

public static partial class Clerk {
    /// <summary>
    /// Extracts each <b>individual</b> <see cref="FileExtension"/> from a path.
    /// </summary>
    /// <remarks>This uses the <see cref="SingleExtensionGroup"/> <see cref="RegexGroup"/> for matching.</remarks>
    /// <param name="path">a path or file name</param>
    /// <returns><b>all</b> of the extensions at the end of the <paramref name="path"/></returns>
    /// <remarks>This method is similar to <see cref="Path.GetExtension(System.ReadOnlySpan{char})"/>, except that it can retrieve multiple extensions, i.e. <c>game.sav.json</c> -> <c>[.sav, .json]</c></remarks>
    [Pure]
    public static ImmutableArray<FileExtension> GetExtensions(string? path) {
        var builder = ImmutableArray.CreateBuilder<FileExtension>();
        var ct      = 0;
        foreach (var span in EnumerateExtensions(path)) {
            ct += 1;
            builder.Add(new FileExtension('.' + span.ToString()));
        }

        builder.Capacity = ct;
        return builder.MoveToImmutable();
    }

    [Pure] internal static SpanSpliterator<char> EnumerateExtensions(ReadOnlySpan<char> path) => GetFullExtension(path).Spliterate('.') with { Options = StringSplitOptions.RemoveEmptyEntries };

    [Pure]
    public static ReadOnlySpan<char> GetFullExtension(ReadOnlySpan<char> path) {
        var fileName    = Path.GetFileName(path);
        var firstPeriod = fileName.IndexOf('.');
        return firstPeriod < 0 || firstPeriod >= (fileName.Length - 1) ? ReadOnlySpan<char>.Empty : fileName[firstPeriod..];
    }
}