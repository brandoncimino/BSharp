using System;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    public static implicit operator RoMultiSpan<T>(RoSpanTuple             _)     => new();
    public static implicit operator RoMultiSpan<T>(ReadOnlySpan<T>         span)  => new(span);
    public static implicit operator RoMultiSpan<T>(RoSpanTuple<T>          span)  => new(span.A);
    public static implicit operator RoMultiSpan<T>(RoSpanTuple<T, T>       spans) => new(spans.A, spans.B);
    public static implicit operator RoMultiSpan<T>(RoSpanTuple<T, T, T>    spans) => new(spans.A, spans.B, spans.C);
    public static implicit operator RoMultiSpan<T>(RoSpanTuple<T, T, T, T> spans) => new(spans.A, spans.B, spans.C, spans.D);

    /// <summary>
    /// Combines all of the contents of this <see cref="RoMultiSpan{T}"/> into a <b>single</b> new <see cref="Array"/>.
    /// </summary>
    /// <returns>a new <see cref="Array"/></returns>
    public T[] FlattenToArray() {
        if (HasElements == false) {
            return Array.Empty<T>();
        }

        var ar = new T[ElementCount];

        int totalCount = 0;
        for (int spanCount = 0; spanCount < SpanCount; spanCount++) {
            for (int eCount = 0; eCount < this[spanCount].Length; eCount++) {
                ar[totalCount] =  this[spanCount][eCount];
                totalCount     += 1;
            }
        }

        return ar;
    }

    /// <summary>
    /// Combines all of the contents of this <see cref="RoMultiSpan{T}"/> into a <b>single</b>, <i>pre-allocated</i> <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="destination">a <i>pre-allocated</i> <see cref="Span{T}"/></param>
    /// <param name="cursor">the position in the <paramref name="destination"/> were we'd like to write these elements</param>
    /// <returns>the updated <paramref name="destination"/> <see cref="Span{T}"/></returns>
    public Span<T> FlattenInto(Span<T> destination, ref int cursor) {
        destination.RequireSpace(cursor, ElementCount);

        foreach (var span in this) {
            destination.Write(span, ref cursor);
        }

        return destination;
    }
}