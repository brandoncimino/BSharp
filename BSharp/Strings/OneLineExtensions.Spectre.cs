using Spectre.Console;

namespace FowlFever.BSharp.Strings;

public static partial class OneLineExtensions {
    /// <inheritdoc cref="StringExtensions.EscapeMarkup"/>
    public static OneLine EscapeMarkup(this OneLine line) => OneLine.CreateRisky(line.Value.EscapeMarkup());
}