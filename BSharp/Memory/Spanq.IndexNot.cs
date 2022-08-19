using System;

using FowlFever.BSharp.Enums;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region IndexNot

    /// <returns>the first index of the <paramref name="span"/> that <b>ISN'T</b> <paramref name="value"/></returns>
    [Pure]
    public static int IndexNot<T>(this ReadOnlySpan<T> span, T value)
        where T : IEquatable<T> {
        for (int i = 0; i < span.Length; i++) {
            if (span[i].Equals(value) == false) {
                return i;
            }
        }

        return -1;
    }

    /// <returns>either <see cref="MemoryExtensions.IndexOf{T}(System.ReadOnlySpan{T},T)"/> or <see cref="IndexNot{T}"/></returns>
    [Pure]
    public static int IndexOf<T>(this ReadOnlySpan<T> span, T value, Polarity polarity)
        where T : IEquatable<T> => polarity switch {
        Polarity.Positive => span.IndexOf(value),
        Polarity.Negative => span.IndexNot(value),
        _                 => throw BEnum.UnhandledSwitch(polarity)
    };

    /// <returns>the last index of <paramref name="span"/> that <b>ISN'T</b> <paramref name="value"/></returns>
    [Pure]
    public static int LastIndexNot<T>(this ReadOnlySpan<T> span, T value)
        where T : IEquatable<T> {
        for (int i = span.Length - 1; i >= 0; i--) {
            if (span[i].Equals(value) == false) {
                return i;
            }
        }

        return -1;
    }

    /// <returns>either <see cref="MemoryExtensions.LastIndexOf{T}(System.ReadOnlySpan{T},T)"/> or <see cref="LastIndexNot{T}"/></returns>
    [Pure]
    public static int LastIndexOf<T>(this ReadOnlySpan<T> span, T value, Polarity polarity)
        where T : IEquatable<T> => polarity switch {
        Polarity.Positive => span.IndexOf(value),
        Polarity.Negative => span.LastIndexNot(value),
        _                 => throw BEnum.UnhandledSwitch(polarity)
    };

    #endregion
}