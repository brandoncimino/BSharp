using System.Collections.Generic;
using System.Text;

namespace FowlFever.BSharp.Strings;

static internal class StringBuilderExtensions {
    /// <summary>
    /// <see cref="StringBuilder.Append(string)"/>s <paramref name="stringToJoin"/> if it <see cref="IsNotBlank(string?)"/>
    /// </summary>
    /// <param name="stringBuilder">this <see cref="StringBuilder"/></param>
    /// <param name="stringToJoin">the <see cref="string"/> being <see cref="StringBuilder.Append(string)"/>ed</param>
    /// <param name="separator">an optional <see cref="string"/> interposed betwixt the <paramref name="stringBuilder"/> and the <paramref name="stringToJoin"/></param>
    /// <returns>this <see cref="StringBuilder"/></returns>
    public static StringBuilder AppendNonBlank(this StringBuilder stringBuilder, string? stringToJoin, string? separator = "") {
        if (stringToJoin.IsNotBlank()) {
            stringBuilder.Append(separator)
                         .Append(stringToJoin);
        }

        return stringBuilder;
    }

    /// <returns>true if <c>null</c> or <see cref="StringBuilder.Length"/> == 0</returns>
    public static bool IsEmpty(this StringBuilder? stringBuilder) => stringBuilder is not { Length: > 0 };

    /// <summary>
    /// Similar to the vanilla <see cref="StringBuilder.AppendJoin(char,object?[])"/>, but conditionally applies a <paramref name="prefix"/> and/or <paramref name="suffix"/>
    /// if any joining actually happens.
    /// </summary>
    /// <param name="stringBuilder"></param>
    /// <param name="values"></param>
    /// <param name="separator"></param>
    /// <param name="prefix"></param>
    /// <param name="suffix"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static StringBuilder AppendJoin<T>(
        this StringBuilder stringBuilder,
        IEnumerable<T>     values,
        string?            separator = default,
        string?            prefix    = default,
        string?            suffix    = default
    ) {
        if (prefix.IsEmpty() && suffix.IsEmpty()) {
            return stringBuilder.AppendJoin(separator, values);
        }

        var sbJoin = new StringBuilder();
        sbJoin.AppendJoin(separator, values);

        if (sbJoin.IsEmpty()) {
            return stringBuilder;
        }

        return stringBuilder.Append(prefix)
                            .Append(sbJoin)
                            .Append(suffix);
    }
}