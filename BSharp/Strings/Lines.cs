using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

using FowlFever.BSharp.Collections;
using FowlFever.Implementors;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Represents a collection of <see cref="OneLine"/> <see cref="string"/>s.
/// </summary>
public readonly record struct Lines : IImmutableList<OneLine>, IHas<string>, IEquatable<string?> {
    private readonly ImmutableArray<OneLine> _lines = ImmutableArray<OneLine>.Empty;
    /// <summary>
    /// The largest <see cref="GraphemeClusterExtensions.VisibleLength(string?)"/> of any of the individual <see cref="Lines"/>.
    /// </summary>
    public int Width => _lines.Max(it => it.Length);
    /// <summary>
    /// The number of <see cref="Lines"/>.
    /// </summary>
    /// <remarks>This is identical to <see cref="ICollection.Count"/>, but is more idiomatic when combined with <see cref="Width"/>.</remarks>
    public int Height => _lines.Length;
    /// <summary>
    /// Combines (<see cref="Width"/>, <see cref="Height"/>).
    /// </summary>
    public (int width, int height) Dimensions => (Width, Height);

    /// <inheritdoc cref="ICollection{T}.Count"/>
    public int Count => _lines.Length;

    internal Lines(IEnumerable<OneLine>? lines) => _lines = lines?.ToImmutableArray() ?? ImmutableArray<OneLine>.Empty;

    internal Lines(SpanLineEnumerator lineEnumerator) {
        var lineBuilder = ImmutableArray.CreateBuilder<OneLine>();
        foreach (var line in lineEnumerator) {
            lineBuilder.Add(new OneLine(line, OneLine.ShouldValidate.No));
        }

        _lines = lineBuilder.MoveToImmutableSafely();
    }

    internal Lines(OneLine line) => _lines = _lines.Add(line);
    public  ImmutableArray<OneLine>.Enumerator GetEnumerator()  => _lines.GetEnumerator();
    private IEnumerable<OneLine>               LineEnumerable() => _lines;
    IEnumerator<OneLine> IEnumerable<OneLine>. GetEnumerator()  => LineEnumerable().GetEnumerator();
    IEnumerator IEnumerable.                   GetEnumerator()  => LineEnumerable().GetEnumerator();
    string IHas<string>.                       Value            => ToString();

    private static readonly string[] LineBreakSplitters = {
        "\r\n", "\r", "\n"
    };

    private static IEnumerable<OneLine> _SplitStringLines(string? str) {
        if (string.IsNullOrEmpty(str)) {
            return Enumerable.Empty<OneLine>();
        }

        return str.Split(LineBreakSplitters, StringSplitOptions.None).Select(OneLine.CreateRisky);
    }

    #region Split (factory methods)

    public static Lines Split(string?                      source) => new(_SplitStringLines(source));
    public static Lines Split(IHas<string?>?               source) => Split(source.OrDefault());
    public static Lines Split(IEnumerable<string?>?        source) => new(source?.Select(_SplitStringLines).AggregateImmutableList());
    public static Lines Split(IEnumerable<IHas<string?>?>? source) => new(source?.Select(it => _SplitStringLines(it.OrDefault())).AggregateImmutableList());
    public static Lines Split(ReadOnlySpan<char>           source) => new(source.EnumerateLines());

    #endregion

    public override string ToString()            => string.Join("\n", this.AsEnumerable());
    public          bool   Equals(string? other) => Height == 1 && ToString().Equals(other);
    public override int    GetHashCode()         => ToString().GetHashCode();
    public          bool   Equals(Lines? other)  => Height == other?.Height && ToString() == other.ToString();

    #region Operators

    public static implicit operator string(Lines  lines) => lines.ToString();
    public static implicit operator Lines(OneLine line)  => new(line);

    #endregion

    /// <summary>
    /// Limits the <see cref="Lines"/> to a <see cref="Height"/> of <paramref name="maxLineCount"/>.
    /// </summary>
    /// <param name="maxLineCount">the maximum allows <see cref="Height"/></param>
    /// <param name="truncationMessage">optionally produces a message describing the number of lines that were truncated</param>
    /// <returns></returns>
    public Lines Truncate(int maxLineCount, Func<int, OneLine>? truncationMessage = default) {
        var (taken, leftovers) = this.TakeLeftovers(maxLineCount);

        if (truncationMessage == null) {
            return new Lines(taken);
        }

        var leftoverCount = leftovers.Count();
        if (leftoverCount > 0) {
            taken = taken.Take(maxLineCount - 1)
                         .Concat(
                             truncationMessage.WrapInEnumerable()
                                              .Select(it => it.Invoke(leftoverCount))
                         );
            truncationMessage(leftoverCount);
        }

        return new Lines(taken);
    }

    #region IImmutableList<OneLine> Implementation

    /// <summary>
    /// Equivalent to <see cref="Height"/>, which should be preferred because it is less ambiguous.
    /// </summary>
    int IReadOnlyCollection<OneLine>.Count => Height;
    public OneLine this[int                                      index] => _lines[index];
    public IImmutableList<OneLine> Add(OneLine                   value)                                                                                                => _lines.Add(value);
    public IImmutableList<OneLine> AddRange(IEnumerable<OneLine> items)                                                                                                => _lines.AddRange(items);
    public IImmutableList<OneLine> Clear()                                                                                                                             => _lines.Clear();
    public int                     IndexOf(OneLine                  item,  int                         index, int count, IEqualityComparer<OneLine>? equalityComparer) => _lines.IndexOf(item, index, count, equalityComparer);
    public IImmutableList<OneLine> Insert(int                       index, OneLine                     element)                                                        => _lines.Insert(index, element);
    public IImmutableList<OneLine> InsertRange(int                  index, IEnumerable<OneLine>        items)                                                          => _lines.InsertRange(index, items);
    public int                     LastIndexOf(OneLine              item,  int                         index, int count, IEqualityComparer<OneLine>? equalityComparer) => _lines.LastIndexOf(item, index, count, equalityComparer);
    public IImmutableList<OneLine> Remove(OneLine                   value, IEqualityComparer<OneLine>? equalityComparer) => _lines.Remove(value, equalityComparer);
    public IImmutableList<OneLine> RemoveAll(Predicate<OneLine>     match)                                                                                        => _lines.RemoveAll(match);
    public IImmutableList<OneLine> RemoveAt(int                     index)                                                                                        => _lines.RemoveAt(index);
    public IImmutableList<OneLine> RemoveRange(IEnumerable<OneLine> items,    IEqualityComparer<OneLine>? equalityComparer)                                       => _lines.RemoveRange(items, equalityComparer);
    public IImmutableList<OneLine> RemoveRange(int                  index,    int                         count)                                                  => _lines.RemoveRange(index, count);
    public IImmutableList<OneLine> Replace(OneLine                  oldValue, OneLine                     newValue, IEqualityComparer<OneLine>? equalityComparer) => _lines.Replace(oldValue, newValue, equalityComparer);
    public IImmutableList<OneLine> SetItem(int                      index,    OneLine                     value) => _lines.SetItem(index, value);

    #endregion
}

/// <summary>
/// Extension methods for working with <see cref="FowlFever.BSharp.Strings.Lines"/>.
/// </summary>
public static class LineExtensions {
    public static Lines Lines(this IEnumerable<OneLine>? lines) => lines switch {
        Lines ln => ln,
        _        => new Lines(lines),
    };

    public static Lines Lines(this IEnumerable<string?>? source) => Strings.Lines.Split(source);

    public static Lines Lines(this string? str) => Strings.Lines.Split(str);

    public static Lines Lines(this IEnumerable<Lines>? sources) {
        if (sources == null) {
            return default;
        }

        var lineBuilder = ImmutableList.CreateBuilder<OneLine>();

        foreach (var src in sources) {
            lineBuilder.Plus(src);
        }

        return lineBuilder.ToImmutable().Lines();
    }

    /// <inheritdoc cref="Strings.OneLine.Create(string?)"/>
    public static OneLine OneLine(this string str) => Strings.OneLine.Create(str);
}