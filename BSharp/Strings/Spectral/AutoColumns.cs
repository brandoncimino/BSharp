using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.Implementors;
using FowlFever.Implementors.NonGeneric;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.BSharp.Strings.Spectral;

/// <summary>
/// Renders things in evenly-sized columns.
/// </summary>
/// <remarks>
/// The output will always be spaced as though it contains <b>at least</b> <see cref="AutoColumnCount"/> columns.
/// If there are more <see cref="_cells"/> than <see cref="AutoColumnCount"/>, they will still be evenly spaced, but fit to the <see cref="ConsoleWidth"/>.
/// </remarks>
public class AutoColumns : IHasList<IRenderable>, IHasRenderable {
    public static int       AutoColumnCount { get; set; } = 4;
    public static Func<int> ConsoleWidth    { get; set; } = () => AnsiConsole.Console.Profile.Width;
    public static int       AutoColumnWidth => ConsoleWidth.Invoke() / AutoColumnCount;

    private readonly IRenderable[]           _cells;
    IList IHasNonGenericList.                AsNonGenericList => _cells;
    IList<IRenderable> IHasList<IRenderable>.AsList           => _cells;
    public int                               ColumnCount      => Math.Max(AutoColumnCount, _cells.Length);

    public AutoColumns(IEnumerable<IRenderable> cells) : this(cells.AsArray()) { }

    public AutoColumns(params IRenderable[] cells) {
        _cells = cells;
    }

    public IRenderable GetRenderable() {
        var grid = new Grid();
        for (int i = 0; i < ColumnCount; i++) {
            grid.AddColumn(new GridColumn { Width = AutoColumnWidth });
        }

        grid.AddRow(_cells.ToArray());
        return grid;
    }
}