using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace FowlFever.BSharp.Strings;

internal static class StringBuilderExtensions {
    /// <summary>
    /// <see cref="StringBuilder.Append(string)"/>s <paramref name="stringToJoin"/> if it isn't <see cref="string.IsNullOrWhiteSpace"/>.
    /// </summary>
    /// <param name="stringBuilder">this <see cref="StringBuilder"/></param>
    /// <param name="stringToJoin">the <see cref="string"/> being <see cref="StringBuilder.Append(string)"/>ed</param>
    /// <param name="separator">an optional <see cref="string"/> interposed betwixt the <paramref name="stringBuilder"/> and the <paramref name="stringToJoin"/></param>
    /// <returns>this <see cref="StringBuilder"/></returns>
    public static StringBuilder AppendNonBlank(this StringBuilder stringBuilder, string? stringToJoin, string? separator = default) {
        if (stringToJoin.IsNotBlank()) {
            stringBuilder.Append(separator)
                         .Append(stringToJoin);
        }

        return stringBuilder;
    }

    /// <returns>true if <c>null</c> or <see cref="StringBuilder.Length"/> == 0</returns>
    public static bool IsEmpty([NotNullWhen(false)] this StringBuilder? stringBuilder) => stringBuilder is not { Length: > 0 };

    public static StringBuilder AppendNonBlank<T>(
        this StringBuilder    stringBuilder,
        IEnumerable<string?>? values,
        ReadOnlySpan<char>    separator = default,
        ReadOnlySpan<char>    prefix    = default,
        ReadOnlySpan<char>    suffix    = default
    ) {
        return stringBuilder.AppendJoin(values.NonBlank(), separator, prefix, suffix);
    }

    public static StringBuilder AppendJoin<T>(
        this StringBuilder stringBuilder,
        IEnumerable<T>?    values,
        ReadOnlySpan<char> separator = default,
        ReadOnlySpan<char> prefix    = default,
        ReadOnlySpan<char> suffix    = default
    ) {
        return stringBuilder.AppendJoin(values, static it => Convert.ToString(it), separator, prefix, suffix);
    }

    public static StringBuilder AppendJoin<T>(
        this StringBuilder stringBuilder,
        IEnumerable<T>?    values,
        Func<T, string>    toStringFunction,
        ReadOnlySpan<char> separator = default,
        ReadOnlySpan<char> prefix    = default,
        ReadOnlySpan<char> suffix    = default
    ) {
        return stringBuilder.AppendJoin(values.Select(toStringFunction), separator, prefix, suffix);
    }

    /// <summary>
    /// Similar to the vanilla <see cref="StringBuilder.AppendJoin(char,object?[])"/>, but conditionally applies a <paramref name="prefix"/> and/or <paramref name="suffix"/>
    /// if any joining actually happens.
    /// </summary>
    /// <param name="stringBuilder"></param>
    /// <param name="strings"></param>
    /// <param name="separator"></param>
    /// <param name="prefix"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static StringBuilder AppendJoin(
        this StringBuilder    stringBuilder,
        IEnumerable<string?>? strings,
        ReadOnlySpan<char>    separator = default,
        ReadOnlySpan<char>    prefix    = default,
        ReadOnlySpan<char>    suffix    = default
    ) {
        if (strings == null) {
            return stringBuilder;
        }

        using var erator = strings.GetEnumerator();

        bool isFirst = true;

        while (erator.MoveNext()) {
            if (isFirst) {
                isFirst = false;
                stringBuilder.Append(prefix);
                stringBuilder.Append(erator.Current);
                continue;
            }

            stringBuilder.Append(separator);
            stringBuilder.Append(erator.Current);
        }

        stringBuilder.Append(suffix);
        return stringBuilder;
    }
}