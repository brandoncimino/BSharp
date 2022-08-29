using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

public static partial class CollectionUtils {
    #region Padding

    private static IEnumerable<T> _GetPadding<T>(
        this IEnumerable<T>               source,
        int                               minLength,
        [RequireStaticDelegate] Delegate? delgato,
        T                                 value
    ) {
        int count = 0;
        foreach (var it in source) {
            yield return it;
            count += 1;
        }

        for (int i = count; i < minLength; i++) {
            yield return delgato switch {
                Func<int, T> fn => fn(i),
                Func<T> fn      => fn(),
                null            => value,
                _               => throw new ArgumentOutOfRangeException(nameof(delgato), delgato, "must be Func<T>, Func<int, T>, or null")
            };
        }
    }

    /// <summary>
    /// If <paramref name="source"/> has fewer than <paramref name="minLength"/> entries, invoke <paramref name="padderWithIndex"/> against each index until we have <paramref name="minLength"/> entries.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// var stuff = new string[]{"✅", "✅"};
    /// var padded = stuff.Pad(4, index => $"[{index}]");
    /// // "✅", "✅", "[2]", "[3]"
    /// ]]></code>
    /// </example>
    /// <param name="source">this <see cref="IEnumerable{T}"/></param>
    /// <param name="minLength">the minimum desired entry count</param>
    /// <param name="padderWithIndex">takes in the current index, and produces the values added until we reach <paramref name="minLength"/></param>
    /// <typeparam name="T">the entry type</typeparam>
    /// <returns>an <see cref="IEnumerable{T}"/> of <b>at least</b> <see cref="minLength"/> entries</returns>
    public static IEnumerable<T> Pad<T>(this IEnumerable<T> source, int minLength, Func<int, T> padderWithIndex) => source._GetPadding(minLength, padderWithIndex, default)!;

    /// <summary>
    /// Adds <paramref name="padder"/> to <paramref name="source"/> until we have <b>at least</b> <paramref name="minLength"/> entries.
    /// </summary>
    /// <param name="source">this <see cref="IEnumerable{T}"/></param>
    /// <param name="minLength">the minimum desired entry count</param>
    /// <param name="padder">the value to be added repeatedly until we reach <paramref name="minLength"/></param>
    /// <typeparam name="T">the entry type</typeparam>
    /// <returns>an <see cref="IEnumerable{T}"/> of <b>at least</b> <paramref name="minLength"/> entries</returns>
    public static IEnumerable<T> Pad<T>(this IEnumerable<T> source, int minLength, T padder) {
        return source._GetPadding(minLength, default, padder);
    }

    /// <summary>
    /// Adds <c>default</c> <typeparamref name="T"/> entries to <paramref name="source"/> until we have <b>at least</b> <paramref name="minLength"/> entries.
    /// </summary>
    /// <param name="source">this <see cref="IEnumerable{T}"/></param>
    /// <param name="minLength">the minimum desired entry count</param>
    /// <typeparam name="T">the entry type</typeparam>
    /// <returns>an <see cref="IEnumerable{T}"/> of <b>at least</b> <paramref name="minLength"/> entries</returns>
    public static IEnumerable<T?> Pad<T>(this IEnumerable<T> source, int minLength) => source._GetPadding(minLength, default, default);

    #endregion
}