using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;

using Spectre.Console;

namespace FowlFever.BSharp.Strings.Spectral;

public static class SpectralExtensions {
    #region Table

    public static Table AddLabelled<T, TLabel>(
        this Table                                  table,
        T                                           value,
        TLabel                                      label,
        Palette?                                    palette     = default,
        [CallerArgumentExpression("value")] string? _expression = default
    ) {
        return table.AddRow(Renderwerks.GetLabelled(value, label, palette, _expression).Enumerate());
    }

    public static Table AddLabelled<T>(this Table table, T value, Palette? palette = default, [CallerArgumentExpression("value")] string? _expression = default) {
        return table.AddRow(Renderwerks.GetLabelled(value, palette, _expression).Enumerate());
    }

    #endregion
}