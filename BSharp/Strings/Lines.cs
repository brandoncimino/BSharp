using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Represents a collection of <see cref="OneLine"/> <see cref="string"/>s.
/// </summary>
public readonly record struct Lines : IImmutableList<OneLine> {
    private readonly IImmutableList<OneLine> _lines;

    public Lines() => _lines = ImmutableList<OneLine>.Empty;
    public Lines(IEnumerable<OneLine> lines) => _lines = lines.AsImmutableList();
    public Lines(string?              str) : this(EachLine(str)) { }
    private IEnumerable<OneLine> LineEnumerable() => _lines;
    public  IEnumerable<OneLine> AsEnumerable()   => LineEnumerable();
    public  IEnumerator<OneLine> GetEnumerator()  => LineEnumerable().GetEnumerator();
    IEnumerator IEnumerable.     GetEnumerator()  => GetEnumerator();

    public override string ToString() {
        return string.Join("\n", AsEnumerable());
    }

    private static IEnumerable<OneLine> EachLine(string? multilineContent) => EachLine(multilineContent, default);

    private static IEnumerable<OneLine> EachLine(string? multilineContent, StringSplitOptions options) {
        return multilineContent?.Split(StringUtils.LineBreakSplitters, options)
                               .Select(it => new OneLine(it)) ?? Enumerable.Empty<OneLine>();
    }

    public static implicit operator string(Lines lines) => lines.ToString();

    /// <summary>
    /// Performs a <see cref="Func{T,TResult}"/> against each <see cref="OneLine"/>.
    /// </summary>
    /// <param name="transformer">a <see cref="Func{T,TResult}"/> that transforms each <see cref="OneLine"/> into one or more <see cref="string"/>s</param>
    /// <returns></returns>
    /// <remarks>
    /// <see cref="EnumerableShim{T}"/> is used to coalesce nearly any <see cref="string"/>-like result into an <see cref="IEnumerable{T}"/>.
    /// <c>null</c> <see cref="string"/>s are then discarded.
    /// </remarks>
    public Lines SelectLines(Func<OneLine, EnumerableShim<string?>> transformer) => this.Select(transformer)
                                                                                        .SelectMany(it => it)
                                                                                        .NonNull()
                                                                                        .Lines();

    private (int x, int y) GetDimensions() {
        (int x, int y) dim = default;
        foreach (var line in this) {
            dim.x =  dim.x.Max(line.VisibleLength());
            dim.y += 1;
        }

        return dim;
    }

    public int Count => _lines.Count;
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
}

/// <summary>
/// Extension methods for working with <see cref="FowlFever.BSharp.Strings.Lines"/>.
/// </summary>
public static class LineExtensions {
    public static Lines Lines(this IEnumerable<OneLine>? lines) => lines switch {
        Lines ln => ln,
        _        => new Lines(lines.OrEmpty()),
    };

    public static Lines Lines(this IEnumerable<string>? source) {
        return source.OrEmpty()
                     .Select(it => it.Lines())
                     .Lines();
    }

    public static Lines Lines(this string? str) => new(str);

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
}