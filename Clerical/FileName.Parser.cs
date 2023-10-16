using System.Diagnostics;

using JetBrains.Annotations;

using Microsoft.Extensions.Primitives;

namespace FowlFever.Clerical;

// This file contains the logic behind parsing. The publicly exposed methods are located in `FileName.Parse.Public`.
public readonly partial record struct FileName {
    #region Parsing

    [UsedImplicitly]
    internal class Parser : IClericalParser<FileName, bool> {
        internal static FileName CreateUnsafe(StringSegment baseName, StringSegment extension) {
            return new FileName(PathPart.CreateUnsafe(baseName), FileExtension.Parser.CreateUnsafe(extension));
        }

        public static FileName Parse_Internal(SpanOrSegment s, bool strict) {
            var success = TryParse_Internal(s, strict, true, out var result);
            Debug.Assert(success);
            return result;
        }

        public static bool TryParse_Internal(SpanOrSegment s, bool strict, bool throwOnFailure, out FileName result) {
            var lastSeparatorIndex = s.AsSpan().LastIndexOfAny('/', '\\', '.');
            if (lastSeparatorIndex < 0) {
                // The string contained no separators - 
                //  it's gross, but valid, like `config` in `~/.ssh/config` or `credentials` in `~/.aws/credentials`.
                result = CreateUnsafe(s.ToStringSegment(), default);
                return true;
            }

            if (FileExtension.Parser.TryParse_Internal(s[lastSeparatorIndex..].ToStringSegment(), strict, throwOnFailure, out var fileExtension) is false) {
                result = default;
                return false;
            }

            var before = s[..lastSeparatorIndex];

            if (PathPart.Parser.TryParse_Internal(before, strict, throwOnFailure, out var baseName) is false) {
                result = default;
                return false;
            }

            result = new FileName(baseName, fileExtension);
            return true;
        }
    }

    #endregion
}