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
    /// Concatenates a bunch of <see cref="ReadOnlySpan{T}"/>s of <see cref="char"/>s into a single <see cref="string"/>.
    /// </summary>
    /// <param name="a"><see cref="char"/>s</param>
    /// <param name="b"><see cref="char"/>s</param>
    /// <param name="c"><see cref="char"/>s</param>
    /// <param name="d"><see cref="char"/>s</param>
    /// <param name="e"><see cref="char"/>s</param>
    /// <param name="f"><see cref="char"/>s</param>
    /// <param name="g"><see cref="char"/>s</param>
    /// <param name="h"><see cref="char"/>s</param>
    /// <returns>a new <see cref="string"/></returns>
    public static string Concat(
        ReadOnlySpan<char> a,
        ReadOnlySpan<char> b,
        ReadOnlySpan<char> c = default,
        ReadOnlySpan<char> d = default,
        ReadOnlySpan<char> e = default,
        ReadOnlySpan<char> f = default,
        ReadOnlySpan<char> g = default,
        ReadOnlySpan<char> h = default
    ) {
        return stackalloc char[
                   a.Length +
                   b.Length +
                   c.Length +
                   d.Length +
                   e.Length +
                   f.Length +
                   g.Length +
                   h.Length
               ]
               .Start(a, out var pos)
               .Write(b, ref pos)
               .Write(c, ref pos)
               .Write(d, ref pos)
               .Write(e, ref pos)
               .Write(f, ref pos)
               .Write(g, ref pos)
               .Write(h, ref pos)
               .ToString();
    }

    /// <summary>
    /// Joins <paramref name="a"/> and <paramref name="b"/> <b>IF</b> they aren't <see cref="M:FowlFever.BSharp.Strings.StringUtils.IsBlank(System.ReadOnlySpan{System.Char})"/>
    /// </summary>
    /// <param name="joiner">the string interposed betwixt <paramref name="a"/> and <paramref name="b"/></param>
    /// <param name="a">the first string</param>
    /// <param name="b">the second string</param>
    /// <returns>a new <see cref="string"/></returns>
    public static string JoinNonBlank(ReadOnlySpan<char> a, ReadOnlySpan<char> b, ReadOnlySpan<char> joiner = default) {
        if (a.IsBlank()) {
            return b.ToString();
        }

        if (b.IsBlank()) {
            return a.ToString();
        }

        return Concat(a, joiner, b);
    }
}