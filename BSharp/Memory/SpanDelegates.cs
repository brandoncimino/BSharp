using System;

namespace FowlFever.BSharp.Memory;

public delegate TOut ReadOnlySpanFunc<TIn, in TArg, out TOut>(ReadOnlySpan<TIn> span, TArg arg);
public delegate TOut ReadOnlySpanFunc<TIn, out TOut>(ReadOnlySpan<TIn>          span);
public delegate TOut SpanFunc<TIn, in TArg, out TOut>(Span<TIn>                 span, TArg arg);
public delegate TOut SpanFunc<TIn, out TOut>(Span<TIn>                          span);