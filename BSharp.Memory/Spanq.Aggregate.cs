using System;
using System.Buffers;
using System.Collections.Generic;

using FowlFever.BSharp.Memory.Enumerators;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region To{X}

    [MustUseReturnValue]
    public static RoMultiSpan<T> ToMultiSpan<T>(this SpanSpliterator<T> spliterator) where T : IEquatable<T> {
        var multiSpan = default(RoMultiSpan<T>);

        foreach (var span in spliterator) {
            multiSpan = multiSpan.Add(span);
        }

        return multiSpan;
    }

    [MustUseReturnValue]
    public static List<TOut> ToList<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut> spanConverter) where TSpan : IEquatable<TSpan> {
        var ls    = new List<TOut>();
        var aggro = spliterator.ForEach((ls, spanConverter), static (span, args) => args.ls.Add(args.spanConverter(span)));
        return aggro.ls;
    }

    [Pure]
    public static OUT[] ToArray<T, OUT>(this RoMultiSpan<T> spans, [RequireStaticDelegate] ReadOnlySpanFunc<T, OUT> spanConverter) {
        var ar = new OUT[spans.SpanCount];

        for (int i = 0; i < spans.SpanCount; i++) {
            ar[i] = spanConverter(spans[i]);
        }

        return ar;
    }

    #region ToString{X}

    [MustUseReturnValue] public static List<string> ToStringList(this SpanSpliterator<char> spliterator) => spliterator.ToList(static it => it.ToString());

    /// <summary>
    /// Converts each of the <see cref="RoMultiSpan{T}.EnumerateSpans"/> into a <see cref="string"/> and collects them into an array.
    /// </summary>
    /// <param name="spans">a collection of <see cref="ReadOnlySpan{T}"/>s of <see cref="char"/>s</param>
    /// <returns>an <see cref="Array"/> of <see cref="string"/>s</returns>
    [Pure]
    public static string[] ToStringArray(this RoMultiSpan<char> spans) => spans.ToArray(static it => it.ToString());

    /// <summary>
    /// Joins all of the spans in this <see cref="RoMultiSpan{T}"/> into a single <see cref="string"/>. 
    /// </summary>
    /// <remarks>
    /// The <paramref name="prefix"/> and <paramref name="joiner"/> will be included in the output string even if the input <paramref name="spans"/> <see cref="RoMultiSpan{T}.HasSpans"/> is <c>false</c>.
    /// </remarks>
    /// <param name="spans">a group of <see cref="ReadOnlySpan{T}"/>s</param>
    /// <param name="joiner">interposed betwixt each of the <paramref name="spans"/></param>
    /// <param name="prefix">appears once, at the beginning of the result</param>
    /// <param name="suffix">appears once, at the end of the result</param>
    /// <returns>a new <see cref="string"/></returns>
    [Pure]
    public static string JoinString(
        this RoMultiSpan<char> spans,
        ReadOnlySpan<char>     joiner = default,
        ReadOnlySpan<char>     prefix = default,
        ReadOnlySpan<char>     suffix = default
    ) {
        var joinerCount  = spans.SpanCount > 0 ? spans.SpanCount - 1 : 0;
        var joinerLength = joinerCount * joiner.Length;
        var bonusLength  = joinerLength + prefix.Length + suffix.Length;

        if (bonusLength == 0) {
            return spans.Flatten(static flat => flat.ToString());
        }

        var        totalLength = spans.ElementCount + bonusLength;
        Span<char> span        = stackalloc char[totalLength];

        span.Start(prefix, out var pos);

        foreach (var sp in spans) {
            span.WriteJoin(sp, joiner, ref pos);
        }

        return span.Write(suffix, ref pos)
                   .Finish(in pos);
    }

    #endregion

    /// <summary>
    /// Converts each of the <see cref="RoMultiSpan{T}.EnumerateSpans"/> into a jagged array, with each element being one of the <see cref="ReadOnlySpan{T}"/>s.
    /// </summary>
    /// <param name="spans">a collection of multiple <see cref="ReadOnlySpan{T}"/>s</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>a jagged array containing the same elements of the <see cref="RoMultiSpan{T}"/></returns>
    [Pure]
    public static T[][] ToJaggedArray<T>(this RoMultiSpan<T> spans) => spans.ToArray(static span => span.ToArray());

    #endregion

    #region Aggregate

    #region Chained Aggregator `Func`

    [MustUseReturnValue]
    public static TOut Aggregate<TSpan, TOut>(this SpanSpliterator<TSpan> spliterator, TOut seed, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut, TOut> aggregator) where TSpan : IEquatable<TSpan> {
        foreach (var span in spliterator) {
            seed = aggregator(span, seed);
        }

        return seed;
    }

    [Pure]
    public static TOut Aggregate<TSpan, TOut>(this RoMultiSpan<TSpan> spans, TOut seed, [RequireStaticDelegate] ReadOnlySpanFunc<TSpan, TOut, TOut> aggregator) {
        foreach (var span in spans) {
            seed = aggregator(span, seed);
        }

        return seed;
    }

    #endregion

    #region Unchained Aggregator `Action`

    /// <summary>
    /// Repeatedly performs a <see cref="ReadOnlySpanAction{T,T}"/> against each of these spans and the <paramref name="arg"/>,
    /// then returns the final value of <paramref name="arg"/> <i>(which may or may not have been modified by the <see cref="ReadOnlySpanAction{T,TArg}"/>)</i>
    /// </summary>
    /// <param name="spliterator">these spans</param>
    /// <param name="arg">the initial <typeparamref name="TArg"/> value</param>
    /// <param name="aggregator">an action invoked against each span + <i>(the current)</i> <paramref name="arg"/></param>
    /// <typeparam name="TSpan">the span element type</typeparam>
    /// <typeparam name="TArg">the <paramref name="arg"/> <i>(and result)</i> type</typeparam>
    /// <returns>the value of the <paramref name="arg"/> after all is said and done <i>(which may or may not have changed)</i></returns>
    [MustUseReturnValue]
    public static TArg ForEach<TSpan, TArg>(this SpanSpliterator<TSpan> spliterator, TArg arg, [RequireStaticDelegate] ReadOnlySpanAction<TSpan, TArg> aggregator) where TSpan : IEquatable<TSpan> {
        foreach (var span in spliterator) {
            aggregator(span, arg);
        }

        return arg;
    }

    /// <inheritdoc cref="ForEach{TSpan,TArg}(SpanSpliterator{TSpan},TArg,System.Buffers.ReadOnlySpanAction{TSpan,TArg})"/>
    [Pure]
    public static TArg ForEach<TSpan, TArg>(this RoMultiSpan<TSpan> spans, TArg arg, [RequireStaticDelegate] ReadOnlySpanAction<TSpan, TArg> aggregator) {
        foreach (var span in spans) {
            aggregator(span, arg);
        }

        return arg;
    }

    #endregion

    #region Flatten

    /// <summary>
    /// Concatenates each of these <paramref name="spans"/> together into a single <see cref="Span{T}"/>, then invokes <paramref name="finisher"/> against it.
    /// </summary>
    /// <param name="spans">these spans</param>
    /// <param name="finisher">converts a <see cref="Span{T}"/> of <typeparamref name="TSpan"/>s into <typeparamref name="TOut"/></param>
    /// <typeparam name="TSpan">the span element type <i>(📎 Must be an "<c>unmanaged</c>" type in order to use the <c>stackalloc</c> keyword)</i></typeparam>
    /// <typeparam name="TOut">the output type</typeparam>
    /// <returns>the <typeparamref name="TOut"/> result of <paramref name="finisher"/></returns>
    public static TOut Flatten<TSpan, TOut>(this RoMultiSpan<TSpan> spans, [RequireStaticDelegate] SpanFunc<TSpan, TOut> finisher) where TSpan : unmanaged {
        if (spans.HasElements == false) {
            return finisher(default);
        }

        Span<TSpan> buffer = stackalloc TSpan[spans.ElementCount];
        var         pos    = 0;

        foreach (var span in spans) {
            buffer.Write(span, ref pos);
        }

        return buffer.Finish(in pos, finisher);
    }

    /// <summary>
    /// <inheritdoc cref="Flatten{TSpan,TOut}"/>
    /// </summary>
    /// <param name="spans">these spans</param>
    /// <param name="arg">additional stuff passed to the <paramref name="finisher"/></param>
    /// <param name="finisher">converts a <see cref="Span{T}"/> of <typeparamref name="TSpan"/>s + <paramref name="arg"/> into <typeparamref name="TOut"/></param>
    /// <typeparam name="TSpan">the span element type</typeparam>
    /// <typeparam name="TArg">the <paramref name="arg"/> type</typeparam>
    /// <typeparam name="TOut">the output type</typeparam>
    /// <returns><inheritdoc cref="Flatten{TSpan,TOut}"/></returns>
    public static TOut Flatten<TSpan, TArg, TOut>(this RoMultiSpan<TSpan> spans, TArg arg, [RequireStaticDelegate] SpanFunc<TSpan, TArg, TOut> finisher) where TSpan : unmanaged {
        if (spans.HasElements == false) {
            return finisher(default, arg);
        }

        Span<TSpan> buffer = stackalloc TSpan[spans.ElementCount];
        var         pos    = 0;
        foreach (var span in spans) {
            buffer.Write(span, ref pos);
        }

        return finisher(buffer, arg);
    }

    #endregion

    #endregion
}