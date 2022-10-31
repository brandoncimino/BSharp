using FowlFever.BSharp;
using FowlFever.BSharp.Memory;

namespace FowlFever.Clerical;

/// <summary>
/// Represents a <see cref="FileSystemInfo"/>'s <see cref="FileSystemInfo.Name"/>, which consists of a <see cref="BaseName"/> + any number of <see cref="Extensions"/>.
/// </summary>
/// <param name="BaseName">The file name without <i>any</i> <see cref="Extensions"/>, e.g. <c>"yolo"</c> in <c>"yolo.swag.txt"</c></param>
/// <param name="Extensions"><i>All</i> of the <see cref="FileSystemInfo.Extension"/>s, e.g. <c>["swag","txt"]</c> in <c>"yolo.swag.txt"</c></param>
public readonly record struct FileName(PathPart BaseName, ValueArray<FileExtension> Extensions) {
    /// <summary>
    /// The full length of the equivalent <see cref="FileSystemInfo.Name"/>, including periods.
    /// </summary>
    public int Length => BaseName.Length + Extensions.Sum(static it => it.Length) + 1;

    #region Construction & factories

    public FileName(PathPart baseName, FileExtension extension) : this(baseName, ValueArray.Of(extension)) { }
    public FileName(PathPart baseName, FileExtension first, FileExtension second) : this(baseName, ValueArray.Of(first,                      second)) { }
    public FileName(PathPart baseName, FileExtension first, FileExtension second, FileExtension third) : this(baseName, ValueArray.Of(first, second, third)) { }

    /// <summary>
    /// Extracts a <see cref="FileName"/> from a <see cref="FileSystemInfo.FullPath"/> <i>(which may or may not contain parent directories, etc.)</i>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static FileName GetFromPath(ReadOnlySpan<char> path) {
        var extensions = Clerk.GetExtensions(path, out var withoutExtensions);
        var baseName   = Path.GetFileName(withoutExtensions);
        return new FileName(baseName, extensions);
    }

    #endregion

    public override string ToString() {
        Span<char> span = stackalloc char[Length];

        span.Start(BaseName, out var pos);

        foreach (var ext in Extensions) {
            span.WriteJoin(ext, '.', ref pos);
        }

        return span.Finish(in pos);
    }

    public static FileName operator +(FileName name, FileExtension extension) {
        return name with {
            Extensions = name.Extensions + extension
        };
    }
}