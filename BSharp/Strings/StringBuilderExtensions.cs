using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using FowlFever.BSharp.Memory;

namespace FowlFever.BSharp.Strings;

internal static class StringBuilderExtensions {
    /// <returns>true if <c>null</c> or <see cref="StringBuilder.Length"/> == 0</returns>
    public static bool IsEmpty([NotNullWhen(false)] this StringBuilder? stringBuilder) => stringBuilder is not { Length: > 0 };

    #region AppendNonBlank

    /// <summary>
    /// <see cref="StringBuilder.Append(ReadOnlySpan{char})"/>s <paramref name="stringToJoin"/> if it isn't <see cref="string.IsNullOrWhiteSpace"/>.
    /// </summary>
    /// <remarks>
    /// <paramref name="prefix"/> and <paramref name="suffix"/> are only included if <paramref name="stringToJoin"/> <see cref="StringUtils.IsNotBlank(ReadOnlySpan{char})"/>.
    /// </remarks>
    /// <param name="stringBuilder">this <see cref="StringBuilder"/></param>
    /// <param name="stringToJoin">the <see cref="string"/> being <see cref="StringBuilder.Append(string)"/>ed</param>
    /// <param name="prefix">appended <i>before</i> <paramref name="stringToJoin"/></param>
    /// <param name="suffix">appended <i>after</i> <paramref name="stringToJoin"/></param>
    /// <returns>this <see cref="StringBuilder"/></returns>
    public static StringBuilder AppendNonBlank(
        this StringBuilder stringBuilder,
        ReadOnlySpan<char> stringToJoin,
        ReadOnlySpan<char> prefix = default,
        ReadOnlySpan<char> suffix = default
    ) {
        if (stringToJoin.IsBlank()) {
            return stringBuilder;
        }

        return stringBuilder.Append(prefix)
                            .Append(stringToJoin)
                            .Append(suffix);
    }

    #endregion

    #region AppendJoin

    /// <summary>
    /// Similar to the vanilla <see cref="StringBuilder.AppendJoin(char,string?[])"/>, but conditionally adds a <paramref name="prefix"/> and/or <paramref name="suffix"/>
    /// if any joining actually happens.
    /// </summary>
    /// <param name="stringBuilder">this <see cref="StringBuilder"/></param>
    /// <param name="values">the stuff that will be added</param>
    /// <param name="separator">interposed betwixt each entry in <paramref name="values"/></param>
    /// <param name="prefix">added <i>before all</i> of the <paramref name="values"/></param>
    /// <param name="suffix">added <i>after all</i> of the <paramref name="values"/></param>
    /// <returns>this <see cref="StringBuilder"/></returns>
    public static StringBuilder AppendJoin(
        this StringBuilder    stringBuilder,
        IEnumerable<string?>? values,
        ReadOnlySpan<char>    separator = default,
        ReadOnlySpan<char>    prefix    = default,
        ReadOnlySpan<char>    suffix    = default
    ) {
        if (values == null) {
            return stringBuilder;
        }

        using var erator = values.GetEnumerator();

        bool isFirst = true;

        while (erator.MoveNext()) {
            if (isFirst) {
                isFirst = false;
                stringBuilder.Append(prefix);
            }
            else {
                stringBuilder.Append(separator);
            }

            stringBuilder.Append(erator.Current);
        }

        if (isFirst == false) {
            stringBuilder.Append(suffix);
        }

        return stringBuilder;
    }

    /// <inheritdoc cref="AppendJoin(System.Text.StringBuilder,System.Collections.Generic.IEnumerable{string?}?,System.ReadOnlySpan{char},System.ReadOnlySpan{char},System.ReadOnlySpan{char})"/>
    public static StringBuilder AppendJoin<T>(
        this StringBuilder stringBuilder,
        IEnumerable<T>?    values,
        ReadOnlySpan<char> separator = default,
        ReadOnlySpan<char> prefix    = default,
        ReadOnlySpan<char> suffix    = default
    ) {
        return stringBuilder.AppendJoin(values, static it => Convert.ToString(it) ?? "", separator, prefix, suffix);
    }

    /// <summary>
    /// <inheritdoc cref="AppendJoin(System.Text.StringBuilder,System.Collections.Generic.IEnumerable{string?}?,System.ReadOnlySpan{char},System.ReadOnlySpan{char},System.ReadOnlySpan{char})"/>
    /// </summary>
    /// <param name="stringBuilder">this <see cref="StringBuilder"/></param>
    /// <param name="values">the stuff that will be added</param>
    /// <param name="toStringFunction">converts each <typeparamref name="T"/> instance to a <see cref="string"/> before adding it</param>
    /// <param name="separator">interposed betwixt each of the <paramref name="values"/></param>
    /// <param name="prefix">added <i>before all</i> of the <paramref name="values"/></param>
    /// <param name="suffix">added <i>after all</i> of the <paramref name="values"/></param>
    /// <typeparam name="T">the type of the <paramref name="values"/></typeparam>
    /// <returns>this <see cref="StringBuilder"/></returns>
    public static StringBuilder AppendJoin<T>(
        this StringBuilder stringBuilder,
        IEnumerable<T>?    values,
        Func<T, string>    toStringFunction,
        ReadOnlySpan<char> separator = default,
        ReadOnlySpan<char> prefix    = default,
        ReadOnlySpan<char> suffix    = default
    ) {
        return stringBuilder.AppendJoin(values?.Select(toStringFunction), separator, prefix, suffix);
    }

    /// <inheritdoc cref="AppendJoin(System.Text.StringBuilder,System.Collections.Generic.IEnumerable{string?}?,System.ReadOnlySpan{char},System.ReadOnlySpan{char},System.ReadOnlySpan{char})"/>
    public static StringBuilder AppendJoin(
        this StringBuilder stringBuilder,
        RoMultiSpan<char>  spans,
        ReadOnlySpan<char> separator = default,
        ReadOnlySpan<char> prefix    = default,
        ReadOnlySpan<char> suffix    = default
    ) {
        if (spans.HasSpans) { }

        var erator = spans.GetEnumerator();

        bool isFirst = true;

        stringBuilder.Append(prefix);

        while (erator.MoveNext()) {
            if (isFirst) {
                isFirst = false;
            }
            else {
                stringBuilder.Append(separator);
            }

            stringBuilder.Append(erator.Current);
        }

        stringBuilder.Append(suffix);
        return stringBuilder;
    }

    #endregion
}