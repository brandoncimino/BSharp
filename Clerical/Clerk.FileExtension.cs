using System.Collections.Immutable;
using System.Diagnostics.Contracts;

using FowlFever.BSharp;

namespace FowlFever.Clerical;

public static partial class Clerk {
    /// <summary>
    /// Extracts each <b>individual</b> <see cref="FileExtension"/> from a path.
    /// </summary>
    /// <param name="path">a path or file name</param>
    /// <returns><b>all</b> of the extensions at the end of the <paramref name="path"/></returns>
    [Pure]
    public static ValueArray<FileExtension> GetExtensions(ReadOnlySpan<char> path) {
        return GetExtensions(path, out _);
    }

    internal static ValueArray<FileExtension> GetExtensions(ReadOnlySpan<char> path, out ReadOnlySpan<char> remaining) {
        remaining = path;

        // most files have 0 or 1 extension, so we check for that scenario first
        if (!FileExtension.TryGetLastExtension(remaining, out remaining, out var first)) {
            // The input didn't have any extensions
            return default;
        }

        if (!FileExtension.TryGetLastExtension(remaining, out remaining, out var second)) {
            // The input didn't have a *second* extension, so we just return the first
            return first;
        }

        // At this point, we have at least 2 extensions, so let's allocate a new ImmutableArray builder and start looping

        var extensions = ImmutableArray.CreateBuilder<FileExtension>();
        extensions.Add(first);
        extensions.Add(second);

        while (FileExtension.TryGetLastExtension(remaining, out remaining, out var ext)) {
            extensions.Add(ext);
        }

        extensions.Capacity = extensions.Count;
        return extensions.MoveToImmutable();
    }

    [Pure]
    public static ReadOnlySpan<char> GetFullExtension(ReadOnlySpan<char> path) {
        var fileName    = Path.GetFileName(path);
        var firstPeriod = fileName.IndexOf('.');
        return firstPeriod < 0 || firstPeriod >= (fileName.Length - 1) ? ReadOnlySpan<char>.Empty : fileName[firstPeriod..];
    }
}