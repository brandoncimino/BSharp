namespace FowlFever.Clerical;

public readonly partial struct FileExtension {
    #region Common File Extensions

    public static readonly FileExtension Json = Parser.CreateUnsafe(".json");
    public static readonly FileExtension Csv  = Parser.CreateUnsafe(".csv");
    /// <summary>
    /// See: <a href="https://en.wikipedia.org/wiki/YAML">YAML</a>
    /// </summary>
    /// <remarks>
    /// <c>.yaml</c> is <a href="https://yaml.org/faq.html">officially correct</a>. Don't use <c>.yml</c>.
    /// </remarks>
    public static readonly FileExtension Yaml = Parser.CreateUnsafe(".yaml");
    public static readonly FileExtension Xml = Parser.CreateUnsafe(".xml");
    public static readonly FileExtension Txt = Parser.CreateUnsafe(".txt");
    /// <summary>
    /// See: <a href="https://en.wikipedia.org/wiki/HTML">HTML</a>
    /// </summary>
    /// <remarks>
    /// Word on the street is some places use <a href="https://en.wikipedia.org/wiki/HTML#Naming_conventions">.htm</a>.
    /// Those places are wrong.
    /// </remarks>
    public static readonly FileExtension Html = Parser.CreateUnsafe(".html");
    /// <summary>
    /// See: <a href="https://en.wikipedia.org/wiki/JPEG">JPEG</a>
    /// </summary>
    /// <remarks>
    /// Many places say to prefer "<c>.jpg</c>" over "<c>.jpeg</c>", however:
    /// <ul>
    /// <li>".jpg" is the incorrect way to abbreviate <a href="https://en.wikipedia.org/wiki/Joint_Photographic_Experts_Group">Join Photographic Experts Group</a></li>
    /// <li>Only ".jpeg" has a corresponding MIME type, <a href="https://stackoverflow.com/a/54488403/18494923">"image/jpeg"</a></li>
    /// <li>Only ".jpeg" has a corresponding <a href="https://www.iso.org/standard/18902.html">ISO standard</a></li>
    /// </ul>
    /// </remarks>
    public static readonly FileExtension Jpeg = Parser.CreateUnsafe(".jpeg");
    public static readonly FileExtension Bmp = Parser.CreateUnsafe(".bmp");
    public static readonly FileExtension Png = Parser.CreateUnsafe(".png");
    /// <summary>
    /// See: <a href="https://en.wikipedia.org/wiki/MPEG-1">MPEG-1</a>.
    /// </summary>
    /// <remarks>See <see cref="Jpeg"/> for the justification of using ".mpeg" over ".mpg".</remarks>
    public static readonly FileExtension Mpeg = Parser.CreateUnsafe(".mpeg");
    public static readonly FileExtension Mp3 = Parser.CreateUnsafe(".mp3");
    public static readonly FileExtension Mp4 = Parser.CreateUnsafe(".mp4");

    #endregion

    /// <param name="perfectExtensionSpan">the input, which must be an <see cref="DebugAssert_PerfectExtension"/></param>
    /// <returns>a known, common file extension, if found; otherwise, <c>null</c></returns>
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
    private static string? TryGetCommonExtensionString(ReadOnlySpan<char> perfectExtensionSpan) {
        return perfectExtensionSpan switch {
            ""     => "", // 📎 You cannot have an "empty" extension; hence this being "" instead of `"."` 
            "json" => ".json",
            "csv"  => ".csv",
            // See: https://yaml.org/faq.html
            "yaml" or "yml" => ".yaml",
            "xml"           => ".xml",
            "txt"           => ".txt",
            "html"          => ".html",
            "jpg" or "jpeg" => ".jpeg",
            "bmp"           => ".bmp",
            "png"           => ".png",
            "mpg" or "mpeg" => ".mpeg",
            "mp3"           => ".mp3",
            "mp4"           => ".mp4",
            _               => null
        };
    }

    internal static bool TryGetCommonExtensionString(ReadOnlySpan<char> perfectExtensionSpan, out string? result) {
        result = TryGetCommonExtensionString(perfectExtensionSpan);
        return result != null;
    }
}