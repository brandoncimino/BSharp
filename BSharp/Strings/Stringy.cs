using System;

using FowlFever.BSharp.Memory;

namespace FowlFever.BSharp.Strings;

public static class Stringy {
    public static ReadOnlySpan<char> FirstNonBlank(
        ReadOnlySpan<char> a,
        ReadOnlySpan<char> b,
        ReadOnlySpan<char> c = default,
        ReadOnlySpan<char> d = default,
        ReadOnlySpan<char> e = default,
        ReadOnlySpan<char> f = default
    ) {
        if (a.IsNotBlank()) {
            return a;
        }

        if (b.IsNotBlank()) {
            return b;
        }

        if (c.IsNotBlank()) {
            return c;
        }

        if (d.IsNotBlank()) {
            return d;
        }

        if (e.IsNotBlank()) {
            return e;
        }

        if (f.IsNotBlank()) {
            return f;
        }

        return default;
    }

    /// <summary>
    /// Concatenates one or more <see cref="ReadOnlySpan{T}"/>s of <see cref="char"/>s into a single <see cref="string"/>.
    /// </summary>
    /// <param name="a"><see cref="char"/>s</param>
    /// <param name="b"><see cref="char"/>s</param>
    /// <param name="c"><see cref="char"/>s</param>
    /// <param name="d"><see cref="char"/>s</param>
    /// <param name="e"><see cref="char"/>s</param>
    /// <param name="f"><see cref="char"/>s</param>
    /// <returns>a new <see cref="string"/></returns>
    public static string Concat(
        ReadOnlySpan<char> a,
        ReadOnlySpan<char> b,
        ReadOnlySpan<char> c = default,
        ReadOnlySpan<char> d = default,
        ReadOnlySpan<char> e = default,
        ReadOnlySpan<char> f = default
    ) {
        var        length = MultiSpan.Length(a, b, c, d, e, f);
        Span<char> span   = stackalloc char[length];
        var        pos    = 0;
        span.Write(a, ref pos)
            .Write(b, ref pos)
            .Write(c, ref pos)
            .Write(d, ref pos)
            .Write(e, ref pos)
            .Write(f, ref pos);
        return new string(span);
    }

    /// <summary>
    /// Joins <paramref name="a"/> and <paramref name="b"/> <b>IF</b> they aren't <see cref="M:FowlFever.BSharp.Strings.StringUtils.IsBlank(System.ReadOnlySpan{System.Char})"/>
    /// </summary>
    /// <param name="joiner">the string interposed betwixt <paramref name="a"/> and <paramref name="b"/></param>
    /// <param name="a">the first string</param>
    /// <param name="b">the second string</param>
    /// <returns></returns>
    public static string JoinNonBlank(ReadOnlySpan<char> joiner, ReadOnlySpan<char> a, ReadOnlySpan<char> b) {
        if (a.IsBlank()) {
            return b.ToString();
        }

        if (b.IsBlank()) {
            return a.ToString();
        }

        return Concat(a, joiner, b);
    }

    /// <inheritdoc cref="JoinNonBlank(System.ReadOnlySpan{char},System.ReadOnlySpan{char},System.ReadOnlySpan{char})"/>
    public static string JoinNonBlank(
        ReadOnlySpan<char> joiner,
        ReadOnlySpan<char> a,
        ReadOnlySpan<char> b,
        ReadOnlySpan<char> c,
        ReadOnlySpan<char> d = default,
        ReadOnlySpan<char> e = default,
        ReadOnlySpan<char> f = default
    ) {
        throw new NotImplementedException();
    }
}