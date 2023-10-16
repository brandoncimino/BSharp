using System.Diagnostics;

namespace FowlFever.Clerical;

// This file contains the public `{Try}Parse` methods, which delegate to the logic defined in `FileName.Parse`.
public readonly partial record struct FileName
#if NET7_0_OR_GREATER
    : ISpanParsable<FileName>
#endif
{
#if NET7_0_OR_GREATER
    static FileName IParsable<FileName>.    Parse(string                s, IFormatProvider? provider)                      => Parse(s);
    static bool IParsable<FileName>.        TryParse(string?            s, IFormatProvider? provider, out FileName result) => TryParse(s, out result);
    static FileName ISpanParsable<FileName>.Parse(ReadOnlySpan<char>    s, IFormatProvider? provider)                      => Parse(s);
    static bool ISpanParsable<FileName>.    TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out FileName result) => TryParse(s, out result);
#endif

    public static FileName Parse(string s) {
        if (s == null) {
            throw new ArgumentNullException(nameof(s));
        }

        var success = Parser.TryParse_Internal(s, false, true, out var result);
        Debug.Assert(success);
        return result;
    }

    [Pure] public static bool TryParse(string? s, out FileName result) => Parser.TryParse_Internal(s, false, false, out result);

    [Pure]
    public static FileName Parse(ReadOnlySpan<char> s) {
        var success = Parser.TryParse_Internal(s, false, true, out var result);
        Debug.Assert(success);
        return result;
    }

    [Pure] public static bool TryParse(ReadOnlySpan<char> s, out FileName result) => Parser.TryParse_Internal(s, false, false, out result);
}