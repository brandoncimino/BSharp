using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using DiffPlex;
using DiffPlex.Chunkers;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Settings;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace FowlFever.Testing;

/// <summary>
/// Performs <b><i>G</i></b>eneric d<b><i>iff</i></b> functions backed by <see cref="DiffPlex"/>.
/// </summary>
[Experimental("Logic seems sound, but it needs prettier formatting of the output using Spectre")]
public static class Giff {
    /// <summary>
    /// Calculates the "difference" between <paramref name="a"/> and <paramref name="b"/> using <see cref="DiffPlex"/>.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>a new <see cref="GiffResult{T}"/></returns>
    public static GiffResult<T> Of<T>(IEnumerable<T> a, IEnumerable<T> b) => GiffBuilder<T>.Default.Giff(a, b);

    public record GiffBuilder<T>(Func<T, string>? ToStringFunction = default) {
        private readonly ConcurrentDictionary<T, Lazy<string>> _toStringCache = new();
        public           Func<T, string>                       ToStringFunction { get; init; } = ToStringFunction ?? (it => EqualityComparer<T>.Default.GetHashCode(it).ToString());
        private readonly ConcurrentDictionary<string, T>       _valueMap = new();
        public           string                                Token   { get; init; } = "🔮✂";
        public           IChunker                              Chunker => new CustomFunctionChunker(str => str.Split(new[] { Token }, StringSplitOptions.None));

        private static GiffBuilder<T>? _default;
        public static  GiffBuilder<T>  Default => _default ??= new GiffBuilder<T>();

        public string ConvertToString(T value) {
            var str = _toStringCache.GetOrAddLazily(value, ToStringFunction).MustNotBeNull();
            _valueMap.TryAdd(str, value);
            return str;
        }

        public T? ConvertFromString(string? serialized) {
            if (serialized == null) {
                return default;
            }

            return _valueMap.TryGetValue(serialized.MustNotBeNull(), out var value)
                       ? value
                       : throw new KeyNotFoundException($"No value was found that corresponds to the string [{serialized}]!");
        }

        public GiffResult<T> CreateGiffResult(SideBySideDiffModel diffModel) => new(CreateGiffPane(diffModel.OldText), CreateGiffPane(diffModel.NewText));
        public GiffPane<T>   CreateGiffPane(DiffPaneModel         diffPane)  => new(diffPane.Lines.Select(CreateGiffPiece).ToImmutableList());
        public GiffPiece<T>  CreateGiffPiece(DiffPiece            diffPiece) => new(ConvertFromString(diffPiece.Text), diffPiece.Type, diffPiece.Position);

        public string GetDiffableString(IEnumerable<T> values) => values.Select(ConvertToString).JoinString(Token);

        private SideBySideDiffModel GetStringDiff(IEnumerable<T> old, IEnumerable<T> neu) {
            var oldText = GetDiffableString(old);
            var newText = GetDiffableString(neu);
            return SideBySideDiffBuilder.Diff(new Differ(), oldText, newText, ignoreWhiteSpace: false, ignoreCase: false, lineChunker: Chunker, wordChunker: Chunker);
        }

        public GiffResult<T> Giff(IEnumerable<T> old, IEnumerable<T> neu) {
            return CreateGiffResult(GetStringDiff(old, neu));
        }
    }

    /// <summary>
    /// Contains the results of a <see cref="Giff.Of{T}"/> function. The results are "lined up" so that they can be displayed side-by-side.
    /// </summary>
    /// <param name="OldSide">a <see cref="GiffPane{T}"/> describing the "old" collection</param>
    /// <param name="NewSide">a <see cref="GiffPane{T}"/> describing the "new" collection</param>
    /// <typeparam name="T">the type of the objects that were being compared</typeparam>
    /// <remarks>This is the generic equivalent of <see cref="SideBySideDiffModel"/>.</remarks>
    public record GiffResult<T>(GiffPane<T> OldSide, GiffPane<T> NewSide) : IPrettifiable {
        public Lines GetLines(PrettificationSettings? settings = default) {
            var oldLines = OldSide.GetLines(settings);
            var newLines = NewSide.GetLines(settings);
            return oldLines
                   .Zip(newLines, (old, neu) => $"{old} {neu}")
                   .Lines();
        }

        public IEnumerable<RenderableGiffPair<T>> GetResultPairs() {
            return OldSide.Pieces.Zip(NewSide.Pieces, (old, neu) => new RenderableGiffPair<T>(old, neu));
        }

        public override string ToString() {
            return GetLines();
        }

        public string Prettify(PrettificationSettings? settings = default) {
            return GetLines(settings);
        }

        public IRenderable GetRenderable() => new GiffRenderer<T>(this).GetRenderable();
    }

    public record RenderableGiffPair<T>(GiffPiece<T> Old, GiffPiece<T> New, bool IncludePositions = false) {
        private Style       RelevantStyle                     => Old.ChangeType == ChangeType.Imaginary ? New.Style : Old.Style;
        private string      RelevantIcon                      => Old.ChangeType == ChangeType.Imaginary ? New.Icon : Old.Icon;
        private IRenderable GetCell(string           content) => new Text(content, RelevantStyle);
        private IRenderable GetValue(GiffPiece<T>    piece)   => GetCell(piece.ChangeType == ChangeType.Imaginary ? "" : piece.Value.OrNullPlaceholder().EscapeMarkup());
        private IRenderable GetPosition(GiffPiece<T> piece)   => GetCell(piece.Position.ToString().EscapeMarkup());
        private IRenderable GetIcon                           => GetCell(RelevantIcon.EscapeMarkup());

        public IEnumerable<IRenderable> GetRenderable() {
            var cells = new[] {
                GetValue(Old),
                GetIcon,
                GetValue(New),
            };

            return IncludePositions ? cells.Prepend(GetPosition(Old)).Append(GetPosition(New)) : cells;
        }
    }

    public record GiffRenderer<T>(GiffResult<T> Result) : IRenderable {
        public LayoutDirection         Direction { get; init; } = LayoutDirection.Vertical;
        public RenderableGiffPair<T>[] Pairs     => Result.GetResultPairs().ToArray();
        public int ColCount => Direction switch {
            LayoutDirection.Horizontal => Pairs.Length,
            LayoutDirection.Vertical   => 5,
            _                          => throw BEnum.UnhandledSwitch(Direction),
        };

        public IRenderable GetRenderable() {
            return CreateGrid();
        }

        // private IRenderable[,] CollectResults() {
        //     var x = Pairs.Select(it => it.GetRenderable())
        //                  .ToArray();
        // }

        private IRenderable CreateGrid() {
            var pairs = Result.GetResultPairs().ToArray();
            var grid  = new Table() { Border = TableBorder.Rounded };
            // ColCount.Repeat(() => grid.AddColumn(new TableColumn("")));

            bool colsAdded = false;

            foreach (var p in Pairs) {
                var getRow = p.GetRenderable().ToArray();
                if (colsAdded == false) {
                    colsAdded = true;
                    for (int i = 0; i < getRow.Length; i++) {
                        grid.AddColumn(new TableColumn(""));
                    }
                }

                grid.AddRow(p.GetRenderable());
            }

            return grid;
        }

        public Measurement          Measure(RenderContext context, int maxWidth) => GetRenderable().Measure(context, maxWidth);
        public IEnumerable<Segment> Render(RenderContext  context, int maxWidth) => GetRenderable().Render(context, maxWidth);
    }

    /// <summary>
    /// Contains the results of <b>one</b> of the "sides" of a <see cref="Giff.Of{T}"/> function.
    /// </summary>
    /// <param name="Pieces">individual item comparison results</param>
    /// <typeparam name="T">the type of the objects that were being compared</typeparam>
    /// <remarks>This is the generic equivalent of <see cref="DiffPaneModel"/>.</remarks>
    public record GiffPane<T>(ImmutableList<GiffPiece<T>> Pieces) : IPrettifiable {
        public bool HasDifferences => Pieces.Any(it => it.ChangeType != ChangeType.Unchanged);

        public Lines GetLines(PrettificationSettings? settings = default) => Pieces.Select(it => it.GetLines(settings)).Lines();

        public string Prettify(PrettificationSettings? settings = default) {
            return GetLines(settings);
        }
    }

    /// <summary>
    /// Contains a <typeparamref name="T"/> <see cref="Value"/> and how it has been <see cref="ChangeType"/>d relative to another <typeparamref name="T"/> value.
    /// </summary>
    /// <param name="Value">the actual <typeparamref name="T"/> value</param>
    /// <param name="ChangeType">describes if and how the <see cref="Value"/> has changed</param>
    /// <param name="Position">???</param>
    /// <typeparam name="T">the type of the <see cref="Value"/></typeparam>
    /// <remarks>This is the generic equivalent of <see cref="DiffPiece"/>.</remarks>
    public record GiffPiece<T>(T? Value, ChangeType ChangeType, int? Position, GiffPrettifier<T>? Prettifier = default) : IPrettifiable {
        public GiffPrettifier<T> Prettifier { get; } = Prettifier ?? new GiffPrettifier<T>();
        public string            Icon       => Prettifier.GetChangeIcon(ChangeType);
        public Style             Style      => Prettifier.GetChangeStyle(ChangeType);

        public Lines GetLines(PrettificationSettings? settings = default) => $"[{Position}] {Icon} {Value}".Lines();

        public string Prettify(PrettificationSettings? settings = default) {
            return GetLines();
        }

        public IEnumerable<IRenderable> GetRenderable(LayoutDirection direction) {
            return new IRenderable[] {
                new Text(Position.ToString().EscapeMarkup(),       Style),
                new Text(Icon.EscapeMarkup(),                      Style),
                new Text(Value.OrNullPlaceholder().EscapeMarkup(), Style)
            };
            // return direction switch {
            // LayoutDirection.Horizontal => new Rows(cells),
            // LayoutDirection.Vertical   => new Columns(cells),
            // _                          => throw BEnum.UnhandledSwitch(direction)
            // };
        }
    }

    public record GiffPrettifier<T> :
        Settings,
        IPrettifier<GiffPiece<T>>,
        IPrettifier<GiffPane<T>>,
        IPrettifier<GiffResult<T>> {
        Type IPrimaryKeyed<Type>.          PrimaryKey       => PrettifierType;
        public Type                        PrettifierType   => typeof(T);
        public IPrettifier<GiffPiece<T>>?  PiecePrettifier  { get; init; }
        public IPrettifier<GiffPane<T>>?   PanePrettifier   { get; init; }
        public IPrettifier<GiffResult<T>>? ResultPrettifier { get; init; }

        public string UnchangedIcon { get; init; } = "💤";
        public string InsertedIcon  { get; init; } = "🆕";
        public string DeletedIcon   { get; init; } = "🚮";
        public string ModifiedIcon  { get; init; } = "〰";
        public string ImaginaryIcon { get; init; } = "💭";

        public Style UnchangedStyle { get; init; } = Style.Plain;
        public Style InsertedStyle  { get; init; } = new(Color.Green);
        public Style DeletedStyle   { get; init; } = new(Color.Red, decoration: Decoration.Strikethrough | Decoration.Italic);
        public Style ModifiedStyle  { get; init; } = new(Color.Blue, decoration: Decoration.Italic);
        public Style ImaginaryStyle { get; init; } = new(Color.LightSlateGrey);

        public string GetChangeIcon(ChangeType changeType) => changeType switch {
            ChangeType.Unchanged => UnchangedIcon,
            ChangeType.Inserted  => InsertedIcon,
            ChangeType.Deleted   => DeletedIcon,
            ChangeType.Modified  => ModifiedIcon,
            ChangeType.Imaginary => ImaginaryIcon,
            _                    => throw BEnum.UnhandledSwitch(changeType),
        };

        public Style GetChangeStyle(ChangeType changeType) => changeType switch {
            ChangeType.Unchanged => UnchangedStyle,
            ChangeType.Inserted  => InsertedStyle,
            ChangeType.Deleted   => DeletedStyle,
            ChangeType.Modified  => ModifiedStyle,
            ChangeType.Imaginary => ImaginaryStyle,
            _                    => throw BEnum.UnhandledSwitch(changeType)
        };

        public string Prettify(GiffPiece<T>?  cinderella, PrettificationSettings? settings = default) => PiecePrettifier?.Prettify(cinderella, settings)  ?? cinderella?.Prettify(settings) ?? settings.Resolve().NullPlaceholder;
        public string Prettify(GiffPane<T>?   cinderella, PrettificationSettings? settings = default) => PanePrettifier?.Prettify(cinderella, settings)   ?? cinderella?.Prettify(settings) ?? settings.Resolve().NullPlaceholder;
        public string Prettify(GiffResult<T>? cinderella, PrettificationSettings? settings = default) => ResultPrettifier?.Prettify(cinderella, settings) ?? cinderella?.Prettify(settings) ?? settings.Resolve().NullPlaceholder;

        public bool CanPrettify(Type type) => type.Extends(typeof(GiffPiece<T>), typeof(GiffPane<T>), typeof(GiffResult<T>));

        string IPrettifier.Prettify(object? cinderella, PrettificationSettings? settings) => cinderella switch {
            GiffPiece<T> piece   => Prettify(piece),
            GiffPane<T> pane     => Prettify(pane),
            GiffResult<T> result => Prettify(result),
            _                    => throw Reject.UnhandledSwitchType(cinderella),
        };
    }
}