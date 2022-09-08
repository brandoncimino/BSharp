using System;

namespace FowlFever.BSharp.Memory;

public readonly ref partial struct RoMultiSpan<T> {
    public static implicit operator RoMultiSpan<T>(RoSpanTuple             _)     => new();
    public static implicit operator RoMultiSpan<T>(ReadOnlySpan<T>         span)  => new(span);
    public static implicit operator RoMultiSpan<T>(RoSpanTuple<T>          span)  => new(span.A);
    public static implicit operator RoMultiSpan<T>(RoSpanTuple<T, T>       spans) => new(spans.A, spans.B);
    public static implicit operator RoMultiSpan<T>(RoSpanTuple<T, T, T>    spans) => new(spans.A, spans.B, spans.C);
    public static implicit operator RoMultiSpan<T>(RoSpanTuple<T, T, T, T> spans) => new(spans.A, spans.B, spans.C, spans.D);
}