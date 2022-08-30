using Spectre.Console;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// A <see cref="Palette"/>-style equivalent to <see cref="Spectre.Console.ExceptionStyle"/>.
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

    public ExceptionPalette() { }

    public ExceptionPalette(ExceptionStyle style) {
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
    }

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

    public static implicit operator ExceptionPalette(ExceptionStyle style)   => new(style);
    public static implicit operator ExceptionStyle(ExceptionPalette palette) => palette.ToExceptionStyle();
}