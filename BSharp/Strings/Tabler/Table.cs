using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings.Settings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Strings.Tabler;

[PublicAPI]
public record Table : IReadOnlyList<Row>, IPrettifiable {
    public IReadOnlyList<Row> Rows { get; }

    public readonly int ColCount;
    public          int RowCount => Rows.Count;
    public          int Count    => RowCount;

    public bool HasHeaderRow { get; init; }

    #region Constructors

    /// <summary>
    /// Constructs a new <see cref="Table"/> with an explicit <see cref="HasHeaderRow"/>.
    /// </summary>
    /// <param name="rows"><see cref="Rows"/></param>
    /// <param name="hasHeaderRow"><see cref="HasHeaderRow"/></param>
    /// <exception cref="ArgumentException">if the <see cref="Rows"/> are not all of the same <see cref="IReadOnlyCollection{T}.Count"/></exception>
    /// <seealso cref="Of(IEnumerable{Row})"/>
    public Table(IEnumerable<Row> rows, bool hasHeaderRow) {
        Rows         = rows.ToList();
        ColCount     = Rows[0].Count;
        HasHeaderRow = hasHeaderRow;

        if (Rows.Any(it => it.Count != ColCount)) {
            throw new ArgumentException("Did not all have the same number of columns!", nameof(rows));
        }
    }

    /// <summary>
    /// Constructs a new <see cref="Table"/>, with <see cref="HasHeaderRow"/> determined by the <see cref="Row.Style"/> of the first <see cref="Row"/>.
    /// </summary>
    /// <param name="rows"><see cref="Rows"/></param>
    public Table(params Row[] rows) : this(rows.AsEnumerable(), rows.First().Style == Row.RowStyle.Header) { }

    #endregion

    #region Factories

    /// <inheritdoc cref="Of(FowlFever.BSharp.Strings.Tabler.Row[])"/>
    public static Table Of(IEnumerable<Row> rows) {
        rows = rows.AsList();
        return new Table(rows, rows.First().Style == Row.RowStyle.Header);
    }

    /// <inheritdoc cref="Of(System.Collections.Generic.IEnumerable{FowlFever.BSharp.Strings.Tabler.Row})"/>
    public static Table Of(IEnumerable<IEnumerable<object>> rows) => Of(rows.Select(Row.Of));

    /// <summary>
    /// Creates a new <see cref="Table"/>, deriving <see cref="HasHeaderRow"/> from the <see cref="Row.RowStyle"/> of the first <see cref="Row"/>.
    /// </summary>
    /// <param name="rows">the <see cref="Rows"/></param>
    /// <returns>a new <see cref="Table"/></returns>
    public static Table Of(params Row[] rows) {
        return new Table(rows, rows.First().Style == Row.RowStyle.Header);
    }

    /// <inheritdoc cref="Of(System.Collections.Generic.IEnumerable{FowlFever.BSharp.Strings.Tabler.Row})"/>
    public static Table Of(params IEnumerable<object>[] rows) {
        return Of(rows.Select(Row.Of));
    }

    /// <summary>
    /// Creates a new <see cref="Table"/> where <see cref="HasHeaderRow"/> is <c>true</c>.
    /// </summary>
    /// <param name="headerRow">the <see cref="Row.RowStyle.Header"/> <see cref="Row"/> of the <see cref="Table"/></param>
    /// <param name="rows">the rest of the <see cref="Rows"/></param>
    /// <returns>a new <see cref="Table"/></returns>
    public static Table Of(Row headerRow, IEnumerable<Row> rows) {
        return WithHeaders(rows.Prepend(headerRow));
    }

    /// <inheritdoc cref="Of(System.Collections.Generic.IEnumerable{FowlFever.BSharp.Strings.Tabler.Row})"/>
    public static Table Of(IEnumerable<object> headerRow, IEnumerable<IEnumerable<object>> rows) => Of(Row.Of(headerRow), rows.Select(Row.Of));

    public static Table Of(object[,] data) {
        throw new NotImplementedException("Need to implement building a table out of a 2d array (maybe through the Grid class?)");
    }

    /// <summary>
    /// Creates a new <see cref="Table"/>, <b>forcing</b> <see cref="HasHeaderRow"/> to be <c>true</c>.
    /// </summary>
    /// <param name="rows">the <see cref="Table"/>'s <see cref="Rows"/>, the first of which will be considered a <see cref="Row.RowStyle.Header"/></param>
    /// <returns>a new <see cref="Table"/></returns>
    public static Table WithHeaders(IEnumerable<Row> rows) => new Table(rows, true);

    /// <inheritdoc cref="WithHeaders(System.Collections.Generic.IEnumerable{FowlFever.BSharp.Strings.Tabler.Row})"/>
    public static Table WithHeaders(IEnumerable<IEnumerable<object>> rows) => Of(rows.Select(Row.Of));

    /// <summary>
    /// Creates a new <see cref="Table"/>, <b>forcing</b> <see cref="HasHeaderRow"/> to be <c>true</c>.
    /// </summary>
    /// <remarks>
    /// This method avoids ambiguity when using the <c>params</c> version of <see cref="Of(FowlFever.BSharp.Strings.Tabler.Row[])"/>.
    /// </remarks>
    /// <param name="headers">the <see cref="Row.RowStyle.Header"/> <see cref="Row"/> of the <see cref="Table"/></param>
    /// <param name="rows">the rest of the <see cref="Rows"/></param>
    /// <returns>a new <see cref="Table"/></returns>
    public static Table WithHeaders(Row headers, params Row[] rows) {
        return WithHeaders(rows.Prepend(headers));
    }

    /// <inheritdoc cref="WithHeaders(System.Collections.Generic.IEnumerable{FowlFever.BSharp.Strings.Tabler.Row})"/>
    public static Table WithHeaders(IEnumerable<object> headers, params IEnumerable<object>[] rows) => Of(Row.Of(headers), rows.Select(Row.Of));

    #region Dictionaries

    /// <summary>
    /// Creates a new <see cref="Table"/> from an <see cref="IDictionary{TKey,TValue}"/>, where the <see cref="Row.RowStyle.Header"/>s are <typeparamref name="TKey"/> and <typeparamref name="TVal"/>.
    /// </summary>
    /// <param name="dictionary">the <see cref="IDictionary{TKey,TValue}"/> to make into a <see cref="Table"/></param>
    /// <typeparam name="TKey">the <see cref="Row.RowStyle.Header"/> of the <see cref="Dictionary{TKey,TValue}.Keys"/> column</typeparam>
    /// <typeparam name="TVal">the <see cref="Row.RowStyle.Header"/> of the <see cref="Dictionary{TKey,TValue}.Values"/> column</typeparam>
    /// <returns>a new <see cref="Table"/></returns>
    public static Table Of<TKey, TVal>(IDictionary<TKey, TVal> dictionary) {
        return Of(
            typeof(TKey),
            typeof(TVal),
            dictionary
        );
    }

    /// <summary>
    /// Creates a new <see cref="Table"/> from an <see cref="IDictionary{TKey,TValue}"/> with the given <see cref="Row.RowStyle.Header"/>s.
    /// </summary>
    /// <param name="keyHeader">the <see cref="Row.RowStyle.Header"/> of the <see cref="IDictionary{TKey,TValue}.Keys"/> column</param>
    /// <param name="valHeader">the <see cref="Row.RowStyle.Header"/> of the <see cref="IDictionary{TKey,TValue}.Values"/> column</param>
    /// <param name="dictionary">the <see cref="Rows"/> of the <see cref="Table"/></param>
    /// <typeparam name="TKey">the <see cref="Type"/> of the <see cref="IDictionary{TKey,TValue}.Keys"/></typeparam>
    /// <typeparam name="TVal">the <see cref="Type"/> of the <see cref="IDictionary{TKey,TValue}.Values"/></typeparam>
    /// <returns>a new <see cref="Table"/></returns>
    public static Table Of<TKey, TVal>(object keyHeader, object valHeader, IEnumerable<KeyValuePair<TKey, TVal>> dictionary) {
        return Of(
            Row.OfHeaders(keyHeader, valHeader),
            dictionary.Select(Row.Of)
        );
    }

    #endregion

    #endregion

    public IEnumerable<Cell> GetCol(int colIndex) {
        return Rows.Select(it => it[colIndex]);
    }

    public IEnumerable<int> GetWidths(PrettificationSettings? settings = default) {
        return ColCount.Select(GetCol)
                       .Select(cells => cells.Select(cell => cell.Prettify(settings)))
                       .Select(cells => cells.LongestLine());
    }

    private Row GetHeaderSeparatorRow(PrettificationSettings? settings = default) {
        settings ??= PrettificationSettings.Default;
        var separators = GetWidths().Select(it => settings.TableSettings.HeaderSeparator.RepeatToLength(it));
        return new Row(separators);
    }

    private IEnumerable<Row> GetStyledRows() {
        if (HasHeaderRow) {
            return Rows.Take(1)
                       .Select(it => it with { Style = Row.RowStyle.Header })
                       .Append(GetHeaderSeparatorRow())
                       .Concat(Rows.Skip(1).Select(it => it with { Style = Row.RowStyle.Normal }));
        }
        else {
            return Rows.Select(it => it with { Style = Row.RowStyle.Normal });
        }
    }

    public string Prettify(PrettificationSettings? settings = default) {
        var widths = GetWidths(settings);
        return GetStyledRows().Select(it => it.Render(widths, settings)).JoinLines();
    }

    #region IReadOnlyCollection Implementation

    public IEnumerator<Row> GetEnumerator() {
        return Rows.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)Rows).GetEnumerator();
    }

    public Row this[int index] => Rows[index];

    #endregion

    public override string ToString() {
        return Prettify();
    }
}