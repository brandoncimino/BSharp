using System.Diagnostics;

namespace FowlFever.Clerical;

public readonly partial record struct PathPart {
    #region Parsing

    private static bool TryParse_Private(SpanOrSegment s, bool strict, bool throwOnFailure, out PathPart result) {
        if (strict == false) {
            s = s.Trim();
        }

        if (s.Length == 0) {
            result = default;
            return true;
        }

        if (char.IsWhiteSpace(s[0]) || char.IsWhiteSpace(s[^1])) {
            result = default;
            return throwOnFailure switch {
                true  => throw new FormatException("Cannot start or end with whitespace!"),
                false => false
            };
        }

        if (s.AsSpan().IndexOfAny(Clerk.DirectorySeparatorChars) >= 0) {
            result = default;
            return throwOnFailure switch {
                true  => throw new FormatException("Cannot contain directory separators!"),
                false => false
            };
        }

        result = CreateUnsafe(s.ToStringSegment());
        return true;
    }

    private static PathPart Parse_Internal(SpanOrSegment s) {
        var success = TryParse_Private(s, false, true, out var result);
        Debug.Assert(success);
        return result;
    }

    internal static bool TryParse_Internal(SpanOrSegment s, out PathPart result) => TryParse_Private(s, false, false, out result);

    #endregion
}