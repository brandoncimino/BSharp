using System;

namespace FowlFever.BSharp.Memory;

public static partial class Spanq {
    #region Skip'n'Take

    #region TakeLeftovers

    /// <summary>
    /// Splits a span into 2 separate <see cref="RoSpanTuple{TA, TB}.A"/> and <see cref="RoSpanTuple{TA,TB}.B"/> spans.
    /// </summary>
    /// <example>
    /// This method is most convenient when used with <see cref="RoSpanTuple{TA,TB}.Deconstruct">Deconstruct</see>ion:
    /// <code><![CDATA[
    /// var (a, b) = span.TakeLeftovers(5);
    /// ]]></code>
    /// </example>
    /// <param name="span">this span</param>
    /// <param name="toTake">the number of elements to put into the <see cref="RoSpanTuple{TA,TB}.A"/> span</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>a <see cref="RoSpanTuple{TA,TB}"/></returns>
    [Pure]
    public static RoSpanTuple<T, T> TakeLeftovers<T>(this ReadOnlySpan<T> span, int toTake) =>
        new() {
            A = span.Take(toTake),
            B = span.Skip(toTake),
        };

    /// <summary>
    /// Splits a span into 2 separate <see cref="SpanTuple{TA,TB}.A"/> and <see cref="SpanTuple{TA,TB}.B"/> spans.
    /// </summary>
    /// <example><inheritdoc cref="TakeLeftovers{T}(System.ReadOnlySpan{T},int)"/></example>
    /// <param name="span">this span</param>
    /// <param name="toTake">the number of elements to put into the <see cref="SpanTuple{TA,TB}.A"/> span</param>
    /// <typeparam name="T">the span element type</typeparam>
    /// <returns>a <see cref="SpanTuple{TA,TB}"/></returns>
    [Pure]
    public static SpanTuple<T, T> TakeLeftovers<T>(this Span<T> span, int toTake) => new() {
        A = span.Take(toTake),
        B = span.Skip(toTake),
    };

    #endregion

    #endregion
}