using System.Diagnostics;

using JetBrains.Annotations;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

// This file contains the logic behind parsing. The publicly exposed methods are located in `FileName.Parse.Public`.
public readonly partial record struct FileName {
    #region Parsing

    [UsedImplicitly]
    internal partial class Parser : IClericalParser<FileName, ClericalStyles, string?> {
        internal static FileName CreateUnsafe(StringSegment baseName, StringSegment extension) {
            return new FileName(PathPart.Parser.CreateUnsafe(baseName), FileExtension.Parser.CreateUnsafe(extension));
        }

        public static FileName CreateUnsafe(SpanOrSegment input) {
            var lastPeriod = input.AsSpan().LastIndexOf('.');
            if (lastPeriod < 0) {
                var baseName = PathPart.Parser.CreateUnsafe(input);
                return new FileName(baseName, default);
            }
            else {
                var baseName  = PathPart.Parser.CreateUnsafe(input[..lastPeriod]);
                var extension = FileExtension.Parser.CreateUnsafe(input[lastPeriod..]);
                return new FileName(baseName, extension);
            }
        }

        public static FileName Parse_Internal(SpanOrSegment s, ClericalStyles styles) {
            return TryParse_Internal(s, styles, out var result) switch {
                null    => result,
                var msg => throw new FormatException(ParseHelpers.CreateFormatExceptionMessage(nameof(FileName), s, styles, msg)),
            };
        }

        public static string? TryParse_Internal(SpanOrSegment s, ClericalStyles styles, out FileName result) {
            // TODO: Is this really worth it? Yeah, it isn't very logical to have a separator at the end of a file name, but the extra check and extra complexity to the logic seems to outweigh the benefits
            // styles.RemoveUnsupportedStyles(ClericalStyles.AllowTrailingDirectorySeparator, nameof(FileName));

            if (styles.TryConsumeBookendTrimming(ref s) is { } failMessage) {
                result = default;
                return failMessage.GetMessage();
            }

            if (s.AsSpan().IsWhiteSpace()) {
                result = default;
                return "Cannot be empty or all whitespace!";
            }

            Debug.Assert(s.AsSpan().IsWhiteSpace() is false, "Should've already dealt with blank inputs!");

            var lastSeparatorIndex = s.AsSpan().LastIndexOfAny('/', '\\', '.');
            if (lastSeparatorIndex < 0) {
                // The string contained no separators - 
                //  it's gross, but valid, like `config` in `~/.ssh/config` or `credentials` in `~/.aws/credentials`.
                // TODO: Is it _really_ worth it to use `CreateUnsafe` here?
                result = CreateUnsafe(s.ToStringSegment(), default);
                return null;
            }

            // If the string contained a directory separator (before we found the last period), we can bail-out early
            if (s[lastSeparatorIndex] is '/' or '\\') {
                result = default;
                return "Cannot contain internal directory separators!";
            }

            var baseNameSegment = s[..lastSeparatorIndex];
            if (PathPart.Parser.TryParse_Internal(baseNameSegment, styles, out var baseName) is { } baseNameMsg) {
                result = default;
                // TODO: ⚠⚠⚠ This would cause allocations during `TryParse` failures! How could I be so foolish?!
                return $"The {nameof(BaseName)} was invalid: {baseNameMsg}";
                // return baseNameMsg;
            }

            var extensionSegment = s[lastSeparatorIndex..];
            if (FileExtension.Parser.TryParse_Internal(extensionSegment, styles, out var fileExtension) is { } extMsg) {
                result = default;
                // TODO: ⚠⚠⚠ This would cause allocations during `TryParse` failures! How could I be so foolish?!
                return $"The {nameof(Extension)} was invalid: {extMsg}";
                // return extMsg;
            }

            result = new FileName(baseName, fileExtension);
            return null;
        }
    }

    #endregion
}