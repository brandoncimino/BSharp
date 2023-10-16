namespace FowlFever.Clerical;

/// <summary>
/// Represents a <see cref="FileSystemInfo"/>'s <see cref="FileSystemInfo.Name"/>, which consists of a <see cref="BaseName"/> + <see cref="Extension"/>.
/// </summary>
/// <param name="BaseName">The file name without <i>any</i> <see cref="Extension"/>, e.g. <c>"yolo"</c> in <c>"yolo.swag"</c></param>
/// <param name="Extension"><i>All</i> of the <see cref="FileSystemInfo.Extension"/>s, e.g. <c>["swag","txt"]</c> in <c>"yolo.swag.txt"</c></param>
public readonly partial record struct FileName(PathPart BaseName, FileExtension Extension)
#if NET7_0_OR_GREATER
    : IEqualityOperators<FileName, FileName, bool>
#endif
{
    /// <summary>
    /// The full length of the equivalent <see cref="FileSystemInfo.Name"/>, including periods.
    /// </summary>
    public int Length => BaseName.Length + Extension.LengthWithPeriod;

    public override string ToString() {
        return string.Create(
            Length,
            this,
            (span, fn) => {
                fn.BaseName.AsSpan().CopyTo(span);
                fn.Extension.AsSpan().CopyTo(span[fn.BaseName.Length..]);
            }
        );
    }
}