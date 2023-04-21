using System;
using System.Buffers;

namespace FowlFever.BSharp.Memory;

#region ReadOnlySpan

public delegate TOut               ReadOnlySpanFunc<TIn, in TArg, out TOut>(ReadOnlySpan<TIn> span, TArg arg);
public delegate TOut               ReadOnlySpanFunc<TIn, out TOut>(ReadOnlySpan<TIn>          span);
public delegate TOut               BiRoSpanFunc<A, B, in TArg, out TOut>(ReadOnlySpan<A>      a, ReadOnlySpan<B> b, TArg arg);
public delegate TOut               BiRoSpanFunc<A, B, out TOut>(ReadOnlySpan<A>               a, ReadOnlySpan<B> b);
public delegate ReadOnlySpan<TOut> ToRoSpanFunc<in TIn, TOut>(TIn                             input);
public delegate ReadOnlySpan<TOut> RoSpanTransformer<TIn, TOut>(ReadOnlySpan<TIn>             span);
public delegate ReadOnlySpan<TOut> RoSpanMixer<TIn, in TArg, TOut>(ReadOnlySpan<TIn>          span, TArg arg);

/// <summary>
/// An <see cref="Action{T}"/> that acts on a <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <remarks>
/// Equivalent to the built-in <see cref="ReadOnlySpanAction{T,TArg}"/>, but without the <c>TArg</c> "state" parameter.
/// </remarks>
/// <typeparam name="T">the <see cref="ReadOnlySpan{T}"/> type</typeparam>
public delegate void ReadOnlySpanAction<T>(ReadOnlySpan<T> span);

#endregion

#region Span

public delegate TOut       SpanFunc<TIn, in TArg, out TOut>(Span<TIn>    span, TArg arg);
public delegate TOut       SpanFunc<TIn, out TOut>(Span<TIn>             span);
public delegate TOut       BiSpanFunc<A, B, in TArg, out TOut>(Span<A>   a, Span<B> b, TArg arg);
public delegate TOut       BiSpanFunc<A, B, out TOut>(Span<A>            a, Span<B> b);
public delegate Span<TOut> ToSpanFunc<in TIn, TOut>(TIn                  input);
public delegate Span<TOut> SpanTransformer<TIn, in TArg, TOut>(Span<TIn> span, TArg arg);

/// <summary>
/// An <see cref="Action{T}"/> that acts on a <see cref="Span{T}"/>.
/// </summary>
/// <remarks>
/// Equivalent to the built-in <see cref="System.Buffers.SpanAction{T, TArg}"/>, but without the <c>TArg</c> "state" parameter.
/// </remarks>
/// <typeparam name="T">the <see cref="Span{T}"/> type</typeparam>
public delegate void SpanAction<T>(Span<T> span);

#endregion