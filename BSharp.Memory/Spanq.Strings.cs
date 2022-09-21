using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Strings (i.e. ReadOnlySpan<char>)

    /// <returns><c>true</c> if this <paramref name="span"/> contains only <see cref="char.IsWhiteSpace(char)"/></returns>
    public static bool IsBlank(this ReadOnlySpan<char> span) {
        foreach (char c in span) {
            if (char.IsWhiteSpace(c) == false) {
                return false;
            }
        }

        return true;
    }

    /// <returns>the inverse of <see cref="IsBlank"/></returns>
    public static bool IsNotBlank(this ReadOnlySpan<char> span) => !span.IsBlank();

    #region Append / Prepend

    /// <summary>
    /// Creates a <see cref="string"/> from this <see cref="ReadOnlySpan{T}"/> + <paramref name="suffix"/>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="suffix">added to the end of the <paramref name="span"/></param>
    /// <returns>a new <see cref="string"/></returns>
    public static string AppendToString(this ReadOnlySpan<char> span, char suffix) {
        return stackalloc char[span.Length + 1].Start(span, out var pos)
                                               .Write(suffix, ref pos)
                                               .BuildString(in pos);
    }

    /// <inheritdoc cref="AppendToString(System.ReadOnlySpan{char},char)"/>
    public static string AppendToString(this ReadOnlySpan<char> span, ReadOnlySpan<char> suffix) {
        return stackalloc char[span.Length + suffix.Length].Start(span, out var pos)
                                                           .Write(suffix, ref pos)
                                                           .BuildString(in pos);
    }

    /// <summary>
    /// Creates a <see cref="string"/> from <paramref name="prefix"/> + this <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="span">this <see cref="ReadOnlySpan{T}"/></param>
    /// <param name="prefix">added to the start of the <paramref name="span"/></param>
    /// <returns>a new <see cref="string"/></returns>
    public static string PrependToString(this ReadOnlySpan<char> span, char prefix) {
        return stackalloc char[span.Length + prefix].Start(span, out var pos)
                                                    .Write(prefix, ref pos)
                                                    .BuildString(in pos);
    }

    /// <inheritdoc cref="PrependToString(System.ReadOnlySpan{char},char)"/>
    public static string PrependToString(this ReadOnlySpan<char> span, ReadOnlySpan<char> prefix) {
        return stackalloc char[span.Length + prefix.Length].Start(span, out var pos)
                                                           .Write(prefix, ref pos)
                                                           .BuildString(in pos);
    }

    #endregion

    #endregion
}