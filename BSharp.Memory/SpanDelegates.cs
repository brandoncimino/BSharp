using System;
using System.Buffers;

namespace FowlFever.BSharp.Memory;

#region {Ro}SpanFunc

/// <summary>
/// A <see cref="Func{T1,T2,TResult}"/> that accepts a <see cref="ReadOnlySpan{T}"/> argument.
/// </summary>
/// <remarks>
/// Analogous to <see cref="ReadOnlySpanAction{T,TArg}"/>.
/// </remarks>
/// <seealso cref="SpanFunc{TIn,TArg,TOut}"/>
public delegate TOut ReadOnlySpanFunc<TIn, in TArg, out TOut>(ReadOnlySpan<TIn> span, TArg arg);

/// <summary>
/// A <see cref="Func{T1, TResult}"/> that accepts a <see cref="ReadOnlySpan{T}"/> argument.
/// </summary>
/// <seealso cref="SpanFunc{TIn,TOut}"/>
public delegate TOut ReadOnlySpanFunc<TIn, out TOut>(ReadOnlySpan<TIn> span);

/// <summary>
/// A <see cref="Func{T1, T2, TResult}"/> that accepts a <see cref="Span{T}"/> argument.
/// </summary>
/// <remarks>
/// Analogous to <see cref="SpanAction{T,TArg}"/>.
/// </remarks>
/// <seealso cref="ReadOnlySpanFunc{TIn,TArg,TOut}"/>
public delegate TOut SpanFunc<TIn, in TArg, out TOut>(Span<TIn> span, TArg arg);

/// <summary>
/// A <see cref="Func{T1, TResult}"/> that accepts a <see cref="Span{T}"/> argument.
/// </summary>
/// <seealso cref="ReadOnlySpanFunc{TIn, TOut}"/>
public delegate TOut SpanFunc<TIn, out TOut>(Span<TIn> span);

#endregion

#region {Ro}SpanTransformer

/// <summary>
/// A <see cref="Func{T1, T2, TResult}"/> that operates on and returns <see cref="ReadOnlySpan{T}"/>s.
/// </summary>
/// <seealso cref="SpanTransformer{TIn,TArg,TOut}"/>
public delegate ReadOnlySpan<TOut> RoSpanTransformer<TIn, in TArg, TOut>(ReadOnlySpan<TIn> span, TArg arg);

/// <summary>
/// A <see cref="Func{T, TResult}"/> that operates on and returns <see cref="ReadOnlySpan{T}"/>s.
/// </summary>
/// <seealso cref="SpanTransformer{TIn,TOut}"/>
public delegate ReadOnlySpan<TOut> RoSpanTransformer<TIn, TOut>(ReadOnlySpan<TIn> span);

/// <summary>
/// A <see cref="Func{T1, T2, TResult}"/> that operates on and returns <see cref="Span{T}"/>s.
/// </summary>
/// <seealso cref="RoSpanTransformer{T1, T2, TOut}"/>
public delegate Span<TOut> SpanTransformer<TIn, in TArg, TOut>(Span<TIn> span, TArg arg);

/// <summary>
/// A <see cref="Func{T, TResult}"/> that operates on and returns <see cref="Span{T}"/>s.
/// </summary>
/// <seealso cref="RoSpanTransformer{TIn,TOut}"/>
public delegate Span<TOut> SpanTransformer<TIn, TOut>(Span<TIn> span);

#endregion

/// <summary>
/// An <see cref="Action{T}"/> that acts on a <see cref="Span{T}"/>.
/// </summary>
/// <remarks>
/// Equivalent to the built-in <see cref="System.Buffers.SpanAction{T, TArg}"/>, but without the <c>TArg</c> "state" parameter.
/// </remarks>
/// <typeparam name="T">the <see cref="Span{T}"/> type</typeparam>
public delegate void SpanAction<T>(Span<T> span);