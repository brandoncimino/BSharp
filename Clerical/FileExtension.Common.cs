using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FowlFever.Clerical;

public readonly partial struct FileExtension {
    #region Common File Extensions

    public const string Json = ".json";
    public const string Csv  = ".csv";
    public const string Yaml = ".yaml";
    public const string Xml  = ".xml";
    public const string Txt  = ".txt";
    public const string Html = ".html";
    /// <summary>
    /// See: <a href="https://en.wikipedia.org/wiki/JPEG">JPEG</a>
    /// </summary>
    /// <remarks>
    /// Many places say to prefer ".jpg" over ".jpeg", however:
    /// <ul>
    /// <li>"jpg" is the incorrect way to abbreviate <a href="https://en.wikipedia.org/wiki/Joint_Photographic_Experts_Group">Join Photographic Experts Group</a></li>
    /// <li>Only "jpeg" has a corresponding MIME type, <a href="https://stackoverflow.com/a/54488403/18494923">"image/jpeg"</a></li>
    /// <li>Only "jpeg" has a corresponding <a href="https://www.iso.org/standard/18902.html">ISO standard</a></li>
    /// </ul>
    /// </remarks>
    public const string Jpeg = ".jpeg";
    public const string Bmp = ".bmp";
    public const string Png = ".png";
    /// <summary>
    /// See <a href="https://en.wikipedia.org/wiki/MPEG-1">MPEG-1</a>.
    /// </summary>
    /// <remarks>See <see cref="Jpeg"/> for the justification of using ".mpeg" over ".mpg".</remarks>
    public const string Mpeg = ".mpeg";
    public const string Mp3 = ".mp3";
    public const string Mp4 = ".mp4";

    #endregion

    /// <param name="perfectExtensionSpan">the input, which must be an <see cref="IsPerfectExtension"/></param>
    /// <param name="result">the cached extension string</param>
    /// <returns>true if the input corresponded to a known, common file extension</returns>
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
    /// But, ChatGPT couldn't tell me how it determined those numbers.
    /// My guess is it's going by "files that people interact with directly" in some way, which is why it includes all of these proprietary formats like <c>.doc</c>,
    /// and excludes serialized formats like <c>.json</c> and <c>.xml</c>.
    /// </remarks>
    /// TODO: This is currently the LAST step when parsing, when it should probably be the FIRST step.
    private static bool TryGetCommonExtensionString(ReadOnlySpan<char> perfectExtensionSpan, [NotNullWhen(true)] out string? result) {
        Debug.Assert(IsPerfectExtension(perfectExtensionSpan));

        result = perfectExtensionSpan switch {
            ""                => "", // 📎 You cannot have an "empty" extension; hence this being "" instead of `"."` 
            ".json"           => Json,
            ".csv"            => Csv,
            ".yaml"           => Yaml,
            ".xml"            => Xml,
            ".txt"            => Txt,
            ".html"           => Html,
            ".jpg" or ".jpeg" => Jpeg,
            ".bmp"            => Bmp,
            ".png"            => Png,
            ".mpg" or ".mpeg" => Mpeg,
            ".mp3"            => Mp3,
            ".mp4"            => Mp4,
            _                 => null
        };

        return result is not null;
    }
}