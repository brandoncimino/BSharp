using System.Diagnostics.Contracts;

namespace FowlFever.Clerical;

public static partial class Clerk {
    /// <summary>
    /// Gets <i>all</i> of the extension in a <see cref="Path"/>, i.e. <c>.sav.json</c> in <c>game.sav.json</c>.
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// a.txt       => .txt 
    /// a.sav.json  => .sav.json
    /// .txt        => .txt
    /// txt         => 
    /// ]]></code>
    /// </example>
    /// <param name="path">a <see cref="Path"/> string</param>
    /// <returns>everything after the first period in the <see cref="GetFileName(System.ReadOnlySpan{char})"/></returns>
    [Pure]
    public static ReadOnlySpan<char> GetFullExtension(ReadOnlySpan<char> path) {
        var fileName    = GetFileName(path);
        var firstPeriod = fileName.IndexOf('.');
        if (firstPeriod < 0 || firstPeriod == path.Length - 1) {
            return default;
        }

        return fileName[firstPeriod..];
    }

    /// <inheritdoc cref="Path.GetExtension(System.ReadOnlySpan{char})"/>
    /// <remarks>
    /// This is similar to the built-in <see cref="Path"/>.<see cref="Path.GetExtension(System.ReadOnlySpan{char})"/>, but uses <see cref="System.MemoryExtensions.LastIndexOf(System.ReadOnlySpan{char})"/> for almost equal performance in best-case scenarios <i>(the extension is less than 3 characters long)</i> and <i>much</i> better performance in the worst-case scenarios <i>(the input doesn't contain an extension at all)</i>
    /// </remarks>
    [Pure]
    public static ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> path) {
        var lastPeriodOrSlash = path.LastIndexOfAny('\\', '/', '.');
        if (lastPeriodOrSlash < 0) {
            return default;
        }

        if (lastPeriodOrSlash == path.Length - 1) {
            return default;
        }

        if (path[lastPeriodOrSlash] != '.') {
            return default;
        }

        return path[lastPeriodOrSlash..];
    }
}