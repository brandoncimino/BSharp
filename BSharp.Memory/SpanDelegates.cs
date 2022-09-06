using System;
using System.Buffers;

namespace FowlFever.BSharp.Memory;

#region ReadOnlySpan

public delegate TOut ReadOnlySpanFunc<TIn, in TArg, out TOut>(ReadOnlySpan<TIn> span, TArg arg);
public delegate TOut ReadOnlySpanFunc<TIn, out TOut>(ReadOnlySpan<TIn>          span);

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

public delegate TOut SpanFunc<TIn, in TArg, out TOut>(Span<TIn> span, TArg arg);
public delegate TOut SpanFunc<TIn, out TOut>(Span<TIn>          span);

/// <summary>
/// An <see cref="Action{T}"/> that acts on a <see cref="Span{T}"/>.
/// </summary>
/// <remarks>
/// Equivalent to the built-in <see cref="System.Buffers.SpanAction{T, TArg}"/>, but without the <c>TArg</c> "state" parameter.
/// </remarks>
/// <typeparam name="T">the <see cref="Span{T}"/> type</typeparam>
public delegate void SpanAction<T>(Span<T> span);

#endregion