using System;

using FowlFever.BSharp.Attributes;

using JetBrains.Annotations;

namespace FowlFever.Testing;

/// <summary>
/// Utilities for measuring memory allocations.
/// </summary>
[Experimental("This is limited by my abilities as of roughly April 1, 2023")]
public static class Memprint {
    /// <summary>
    /// Gets the change in <see cref="GC.GetAllocatedBytesForCurrentThread"/> before and after invoking an <see cref="Action{T}"/>.
    /// </summary>
    /// <param name="arg">the <see cref="Action{T}"/> input argument</param>
    /// <param name="code">the <see cref="Action{T}"/> to be invoked</param>
    /// <typeparam name="ARG">the type of <paramref name="arg"/></typeparam>
    /// <returns>the number of bytes allocated when the <paramref name="code"/> was invoked</returns>
    public static long Record<ARG>(ARG arg, [RequireStaticDelegate] Action<ARG> code) {
        var before = GC.GetAllocatedBytesForCurrentThread();
        code.Invoke(arg);
        var after = GC.GetAllocatedBytesForCurrentThread();
        return after - before;
    }

    /// <summary>
    /// Gets the change in <see cref="GC.GetAllocatedBytesForCurrentThread"/> before and after invoking a <see cref="Func{T,TResult}"/>.
    /// </summary>
    /// <param name="arg">the <see cref="Func{T,TResult}"/> input argument</param>
    /// <param name="code">the <see cref="Func{T,TResult}"/> to be invoked</param>
    /// <param name="result">will hold the <see cref="Func{T,TResult}"/> output</param>
    /// <typeparam name="IN">the <see cref="Func{T, TResult}"/> input</typeparam>
    /// <typeparam name="OUT">the <see cref="Func{T, TResult}"/> output</typeparam>
    /// <returns>the number of bytes allocated when the <paramref name="code"/> was invoked</returns>
    public static long Record<IN, OUT>(ref IN arg, [RequireStaticDelegate] Func<IN, OUT> code, out OUT result) {
        var before = GC.GetAllocatedBytesForCurrentThread();
        result = code.Invoke(arg);
        var after = GC.GetAllocatedBytesForCurrentThread();
        return after - before;
    }
}