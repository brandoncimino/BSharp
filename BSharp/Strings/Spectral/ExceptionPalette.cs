using Spectre.Console;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// A <see cref="Palette"/>-style equivalent to <see cref="Spectre.Console.ExceptionStyle"/> / <see cref="ExceptionSettings"/> / <see cref="ExceptionFormats"/>.
/// </summary>
public readonly record struct ExceptionPalette {
    private static readonly ExceptionStyle DefaultStyle = new();

    /// <inheritdoc cref="ExceptionStyle.Dimmed"/>
    public Stylist Dimmed { get; init; } = DefaultStyle.Dimmed;

    /// <inheritdoc cref="ExceptionStyle.Exception"/>
    public Stylist Exception { get; init; } = DefaultStyle.Exception;

    /// <inheritdoc cref="ExceptionStyle.Message"/>
    public Stylist Message { get; init; } = DefaultStyle.Message;

    /// <inheritdoc cref="ExceptionStyle.Method"/>
    public Stylist Method { get; init; } = DefaultStyle.Method;

    /// <inheritdoc cref="ExceptionStyle.Parenthesis"/>
    public Stylist Parenthesis { get; init; } = DefaultStyle.Parenthesis;

    /// <inheritdoc cref="ExceptionStyle.Path"/>
    public Stylist Path { get; init; } = DefaultStyle.Path;

    /// <inheritdoc cref="ExceptionStyle.LineNumber"/>
    public Stylist LineNumber { get; init; } = DefaultStyle.LineNumber;

    /// <inheritdoc cref="ExceptionStyle.NonEmphasized"/>
    public Stylist NonEmphasized { get; init; } = DefaultStyle.NonEmphasized;

    /// <inheritdoc cref="ExceptionStyle.ParameterName"/>
    public Stylist ParameterName { get; init; } = DefaultStyle.ParameterName;

    /// <inheritdoc cref="ExceptionStyle.ParameterType"/>
    public Stylist ParameterType { get; init; } = DefaultStyle.ParameterType;

    /// <inheritdoc cref="ExceptionSettings.Format"/>
    public ExceptionFormats Format { get; init; } = ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks;

    public ExceptionPalette() { }

    public ExceptionPalette(ExceptionStyle style, ExceptionFormats format = ExceptionFormats.Default) {
        Dimmed        = style.Dimmed;
        Exception     = style.Exception;
        Message       = style.Message;
        Method        = style.Method;
        Parenthesis   = style.Parenthesis;
        Path          = style.Path;
        LineNumber    = style.LineNumber;
        NonEmphasized = style.NonEmphasized;
        ParameterName = style.ParameterName;
        ParameterType = style.ParameterType;
        Format        = format;
    }

    public ExceptionPalette(ExceptionSettings settings) : this(settings.Style, settings.Format) { }

    [Pure]
    public ExceptionStyle ToExceptionStyle() {
        return new ExceptionStyle {
            Dimmed        = Dimmed,
            Exception     = Exception,
            Message       = Message,
            Method        = Method,
            Parenthesis   = Parenthesis,
            Path          = Path,
            LineNumber    = LineNumber,
            NonEmphasized = NonEmphasized,
            ParameterName = ParameterName,
            ParameterType = ParameterType,
        };
    }

    [Pure]
    public ExceptionSettings ToExceptionSettings() {
        return new ExceptionSettings {
            Format = Format,
            Style  = ToExceptionStyle(),
        };
    }

    #region Conversions

    public static implicit operator ExceptionPalette(ExceptionStyle    style)    => new(style);
    public static implicit operator ExceptionPalette(ExceptionSettings settings) => new(settings);

    public static implicit operator ExceptionStyle(ExceptionPalette    palette) => palette.ToExceptionStyle();
    public static implicit operator ExceptionFormats(ExceptionPalette  palette) => palette.Format;
    public static implicit operator ExceptionSettings(ExceptionPalette palette) => palette.ToExceptionSettings();

    #endregion
}