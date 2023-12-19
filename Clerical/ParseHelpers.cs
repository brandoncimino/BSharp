using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FowlFever.Clerical;

internal static class ParseHelpers {
    [Pure]
    public static string RequireNonNull([NotNull] this string? s, [CallerArgumentExpression("s")] string? _s = default) {
        if (s == null) {
            throw new ArgumentNullException(_s);
        }

        return s;
    }

    public static CharValidationResult ValidateChar(char c) {
        // If `c` is ASCII, which it almost always will be, then we can use simpler numeric checks
        if (IsAscii(c)) {
            return c switch {
                >= 'a' and <= 'z' => CharValidationResult.Perfect,
                '.'               => CharValidationResult.ExtraPeriod,
                '/' or '\\'       => CharValidationResult.DirectorySeparator,
                ' '               => CharValidationResult.WhiteSpace,
                >= 'A' and <= 'Z' => CharValidationResult.Uppercase,
                < ' '             => CharValidationResult.Control,
                _                 => CharValidationResult.Perfect
            };
        }

        // The following checks handle all Unicode `char`s.
        if (char.IsUpper(c)) {
            return CharValidationResult.Uppercase;
        }

        // TODO: 🙋‍♀️ Should we allow the zero-width joiner?
        if (char.IsWhiteSpace(c)) {
            return CharValidationResult.WhiteSpace;
        }

        if (char.IsControl(c)) {
            return CharValidationResult.Control;
        }

        return CharValidationResult.Perfect;
    }

    /// <summary>
    /// 📎 Check copied from .NET 7+ `char.IsAscii`
    /// <br/>
    /// 🤔 I have no idea why they have the explicit cast to `(uint)`; it seems completely redundant to me 🤷‍♀️
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAscii(char c) => (uint)c <= '\x007f';

    /// <summary>
    /// 📎 Stolen from .NET 7+ `char.IsBetween`
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsBetween(char c, char minInclusive, char maxInclusive) =>
        (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);

    public enum CharValidationResult {
        Perfect,
        Uppercase,
        WhiteSpace,
        Control,
        ExtraPeriod,
        DirectorySeparator,
    }

    public static SpanOrSegment TrimLeadingDirectorySeparator(SpanOrSegment s) {
        return s switch {
            ['/' or '\\', .. var after] => after,
            _                           => s
        };
    }

    public static SpanOrSegment TrimTrailingDirectorySeparator(SpanOrSegment s) {
        return s switch {
            [.. var before, '/' or '\\'] => before,
            _                            => s
        };
    }

    public static SpanOrSegment TrimBookendDirectorySeparators(SpanOrSegment s) {
        return s switch {
            ['/' or '\\', .. var inner, '/' or '\\'] => inner,
            ['/' or '\\', .. var after]              => after,
            [.. var before, '/' or '\\']             => before,
            _                                        => s,
        };
    }

    public static string CreateFormatExceptionMessage(string parsedTypeName, SpanOrSegment input, ClericalStyles styles, string message) {
        // TODO: I'm sure there's something fancy I can do with this
        return $"Unable to parse a {parsedTypeName} from the input {input.ToString()} using the {nameof(ClericalStyles)} [{styles}]: {message}";
    }
}