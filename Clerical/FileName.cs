namespace FowlFever.Clerical;

/// <summary>
/// Represents a <see cref="FileSystemInfo"/>'s <see cref="FileSystemInfo.Name"/>, which consists of a <see cref="BaseName"/> + <see cref="Extension"/>.
/// </summary>
/// <param name="BaseName">The file name without <i>any</i> <see cref="Extension"/>, e.g. <c>"yolo"</c> in <c>"yolo.swag"</c></param>
/// <param name="Extension"><i>All</i> of the <see cref="FileSystemInfo.Extension"/>s, e.g. <c>["swag","txt"]</c> in <c>"yolo.swag.txt"</c></param>
public readonly record struct FileName(PathPart BaseName, FileExtension Extension) {
    /// <summary>
    /// The full length of the equivalent <see cref="FileSystemInfo.Name"/>, including periods.
    /// </summary>
    public int Length => BaseName.Length + Extension.Length;

    #region Construction & factories

    public static bool TryParse(string fileName, out FileName result) {
        if (fileName.AsSpan().IndexOfAny(Clerk.DirectorySeparatorChars) >= 0) {
            result = default;
            return false;
        }

        var periodIndex = fileName.AsSpan().LastIndexOf('.');
        result = periodIndex switch {
            < 0 => new FileName(fileName, default),
            _ => new FileName(
                new PathPart(new Substring(fileName,      0, periodIndex)),
                new FileExtension(new Substring(fileName, periodIndex))
            )
        };

        return true;
    }

    public static FileName Parse(string fileName) {
        if (TryParse(fileName, out var result)) {
            return result;
        }

        throw new FormatException();
    }

    #endregion

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