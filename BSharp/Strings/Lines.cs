using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Strings;

/// <summary>
/// Represents a collection of <see cref="OneLine"/> <see cref="string"/>s.
/// </summary>
public readonly record struct Lines : IEnumerable<OneLine> {
    // public override  string                    Value => this.JoinLines();
    private readonly Supplied<IEnumerable<OneLine>> _lineSupplier;

    public Lines() => _lineSupplier = Supplied<IEnumerable<OneLine>>.Empty;

    public Lines(Supplied<IEnumerable<OneLine>> lineSupplier) => _lineSupplier = lineSupplier;

    public IEnumerator<OneLine> GetEnumerator() => _lineSupplier.Value.OrEmpty().GetEnumerator();
    IEnumerator IEnumerable.    GetEnumerator() => GetEnumerator();

    public static IEnumerable<OneLine> EachLine(string? multilineContent) => EachLine(multilineContent, default);

    private static IEnumerable<OneLine> EachLine(string? multilineContent, StringSplitOptions options) {
        static (bool shouldReturn, OneLine line) FinishLine(StringBuilder sb, StringSplitOptions options) {
            var str = sb.ToString();
            sb.Clear();

            if (options == StringSplitOptions.RemoveEmptyEntries && str.Length == 0) {
                return (false, new OneLine(str));
            }

            return (true, new OneLine(str));
        }

        if (multilineContent == null) {
            yield break;
        }

        var line = new StringBuilder();
        foreach (var c in multilineContent) {
            if (c.IsLineBreak()) {
                var (shouldReturn, str) = FinishLine(line, options);

                if (shouldReturn) {
                    yield return str;
                }

                continue;
            }

            line.Append(c);
        }
    }

    public static implicit operator string(Lines lines) => lines.JoinLines();

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

    // public Lines EachLine(Func<string, string> transformer) => this.Select(it => transformer(it)).Lines();
}

/// <summary>
/// Extension methods for working with <see cref="FowlFever.BSharp.Strings.Lines"/>.
/// </summary>
public static class LineExtensions {
    public static Lines Lines(this IEnumerable<OneLine>? lines) => lines switch {
        Lines ln => ln,
        _        => new Lines(new Supplied<IEnumerable<OneLine>>(lines.OrEmpty)),
    };

    public static Lines Lines(this IEnumerable<string>? source) => source.OrEmpty().Select(it => it.Lines().AsEnumerable()).Flatten().Lines();
    public static Lines Lines(this string?              str)    => new(Strings.Lines.EachLine(str).Shim());
}