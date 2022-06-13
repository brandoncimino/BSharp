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
}