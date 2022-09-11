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

    #endregion
}