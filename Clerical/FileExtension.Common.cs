using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.HighPerformance.Buffers;

namespace FowlFever.Clerical;

public readonly partial struct FileExtension {
    #region Common File Extensions

    public static readonly FileExtension Json = new("json");
    public static readonly FileExtension Csv  = new("csv");
    public static readonly FileExtension Yaml = new("yaml");
    public static readonly FileExtension Xml  = new("xml");
    public static readonly FileExtension Txt  = new("txt");
    public static readonly FileExtension Html = new("html");
    public static readonly FileExtension Jpg  = new("jpg");
    public static readonly FileExtension Bmp  = new("bmp");
    public static readonly FileExtension Png  = new("png");
    public static readonly FileExtension Mpg  = new("mpg");
    public static readonly FileExtension Mp3  = new("mp3");
    public static readonly FileExtension Mp4  = new("mp4");

    #endregion

    /// <summary>
    /// A <see cref="StringPool"/> specifically for <see cref="FileExtension"/> strings.
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>This pool should only be used if <see cref="TryGetCommonExtensionString"/> returned <c>false</c>.</li>
    /// <li>This pool should only contain strings that <c>start with a period</c> and are shorter than <see cref="MaxExtensionLengthIncludingPeriod"/>.</li>
    /// </ul>
    /// </remarks>
    private static readonly StringPool FileExtensionPool = new();

    internal static bool IsAllLowercase(ReadOnlySpan<char> span) {
        foreach (var c in span) {
            if (char.IsUpper(c)) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lowercaseExtension_withPeriod"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <remarks>
    /// The 10 most common file extensions according to <a href="https://chat.openai.com/">ChatGPT</a> are:
    /// <code>
    ///  1.  .txt - Text document
    ///  2.  .jpg or .jpeg - JPEG image
    ///  3.  .png - PNG image
    ///  4.  .doc or .docx - Microsoft Word document
    ///  5   .pdf - Portable Document Format
    ///  6.  .xlsx - Microsoft Excel spreadsheet
    ///  7.  .ppt or .pptx - Microsoft PowerPoint presentation
    ///  8.  .zip - Compressed archive
    ///  9.  .mp3 - MP3 audio file
    ///  10. .mp4 - MP4 video file
    /// </code>
    /// But, ChatGPT couldn't tell me how it determined those numbers. My guess is it's going by "files that people interact with directly" in some way, which is why it includes all of these proprietary formats like <c>.doc</c>, and excludes serialized formats like <c>.json</c> and <c>.xml</c>.
    /// </remarks>
    private static bool TryGetCommonExtensionString(scoped ReadOnlySpan<char> lowercaseExtension_withPeriod, [NotNullWhen(true)] out string? result) {
        Debug.Assert(lowercaseExtension_withPeriod.IsEmpty == false, "must NOT be empty");
        Debug.Assert(lowercaseExtension_withPeriod[0]      == '.',   "MUST start with period");
        Debug.Assert(IsAllLowercase(lowercaseExtension_withPeriod),  "MUST be all lowercase");

        if (lowercaseExtension_withPeriod.Length is not (4 or 5)) {
            result = default;
            return false;
        }

        result = lowercaseExtension_withPeriod switch {
            "json"          => ".json",
            "csv"           => ".csv",
            "yaml"          => ".yaml",
            "xml"           => ".xml",
            "txt"           => ".txt",
            "html"          => ".html",
            "jpg" or "jpeg" => ".jpg",
            "bmp"           => ".bmp",
            "png"           => ".png",
            "mpg" or "mpeg" => ".mpg",
            "mp3"           => ".mp3",
            "mp4"           => ".mp4",
            _               => null
        };

        return result != null;
    }

    private static string FinalizeExtensionString(ReadOnlySpan<char> lowercaseExtension_withPeriod) {
        Debug.Assert(lowercaseExtension_withPeriod.IsEmpty == false);
        Debug.Assert(lowercaseExtension_withPeriod[0]      == '.');
        Debug.Assert(IsAllLowercase(lowercaseExtension_withPeriod));

        return TryGetCommonExtensionString(lowercaseExtension_withPeriod, out var result)
                   ? result
                   : FileExtensionPool.GetOrAdd(lowercaseExtension_withPeriod);
    }
}