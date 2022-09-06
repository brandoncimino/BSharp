using System;
using System.Collections.Generic;
using System.Linq;

namespace FowlFever.BSharp;

public static partial class LambdaExtensions {
    #region InvokeAll

    /// <summary>
    /// Retrieves the <see cref="Delegate.GetInvocationList"/> from a <see cref="Delegate"/> as the actual derived type instead of the base <see cref="Delegate"/>.
    /// </summary>
    /// <param name="delgato">a <see cref="Delegate"/></param>
    /// <typeparam name="T">a <see cref="Delegate"/> type</typeparam>
    /// <returns>the <see cref="Delegate.GetInvocationList"/> as <typeparamref name="T"/> delegates</returns>
    public static IEnumerable<T> GetInvocations<T>(this T? delgato) where T : Delegate {
        return delgato?.GetInvocationList().Select(it => (T)it) ?? Enumerable.Empty<T>();
    }

    /// <summary>
    /// Retrieves the results of each of a <see cref="Delegate"/>'s <see cref="GetInvocations{T}"/>.
    /// </summary>
    /// <param name="func">this <see cref="Func{T,TResult}"/></param>
    /// <param name="arg">the argument to the <see cref="Func{T,TResult}"/></param>
    /// <typeparam name="T">the <paramref name="arg"/> type</typeparam>
    /// <typeparam name="TOut">the result type</typeparam>
    /// <returns>a collection of <typeparamref name="TOut"/> results</returns>
    public static IEnumerable<TOut> InvokeAll<T, TOut>(this Func<T, TOut> func, T arg) => func.GetInvocations().Select(it => it(arg));

    /// <summary>
    /// Retrieves the results of each <see cref="Func{T,TResult}"/>.
    /// </summary>
    /// <param name="functions">a collection of <see cref="Func{T,TResult}"/>s</param>
    /// <param name="arg"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public static IEnumerable<TOut> InvokeAll<T, TOut>(this IEnumerable<Func<T, TOut>> functions, T arg) => functions.Select(it => it(arg));

    #endregion
}