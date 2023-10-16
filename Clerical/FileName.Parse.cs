using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

// This file contains the logic behind parsing. The publicly exposed methods are located in `FileName.Parse.Public`.
public readonly partial record struct FileName {
    #region Parsing

    internal static FileName CreateUnsafe(StringSegment baseName, StringSegment extension) {
        return new FileName(PathPart.CreateUnsafe(baseName), FileExtension.CreateUnsafe(extension));
    }

    private static FileName Parse_Internal(SpanOrSegment s) {
        return TryParse_Internal(s, out var result) switch {
            true  => result,
            false => throw new FormatException()
        };
    }

    private static bool TryParse_Internal(SpanOrSegment s, out FileName result) {
        var lastSeparatorIndex = s.AsSpan().LastIndexOfAny(FileExtension.ExtensionSeparatorChars);
        if (lastSeparatorIndex < 0) {
            // The string contained no separators - 
            //  it's gross, but valid, like `~/.ssh/config` or `~/.aws/credentials`.
            result = CreateUnsafe(s.ToStringSegment(), default);
            return true;
        }

        var fileExtension = FileExtension.CreateUnsafe(s[lastSeparatorIndex..].ToStringSegment());

        if (lastSeparatorIndex == 0) {
            // The string starts with a period or directory separator -
            //  in this case, only strings starting with periods are valid,
            //  which is a common use case for "hidden" files like Android's `.NOMEDIA`
            if (s[lastSeparatorIndex] is '.') {
                result = new FileName(default, fileExtension);
                return true;
            }

            result = default;
            return false;
        }

        var before = s[..lastSeparatorIndex];

        if (PathPart.TryParse_Internal(before, out var baseName)) {
            result = new FileName(baseName, fileExtension);
            return true;
        }

        result = default;
        return false;
    }

    #endregion
}