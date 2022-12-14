using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Collections;

/// <summary>
/// TODO: Move extensions from <see cref="CollectionUtils"/> into here
/// </summary>
public static class DictionaryExtensions {
    /// <summary>
    /// "Safely" retrieves an entry that might not be there.
    /// </summary>
    /// <param name="dic">this <see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="key">the desired <see cref="KeyValuePair{TKey,TValue}.key"/></param>
    /// <typeparam name="TKey">the <see cref="KeyValuePair{TKey,TValue}.key"/> type</typeparam>
    /// <typeparam name="TValue">the <see cref="KeyValuePair{TKey,TValue}.value"/> type</typeparam>
    /// <returns><see cref="IDictionary{TKey,TValue}.this[TKey]"/>, if present; otherwise, the <c>default</c>(<typeparamref name="TValue"/>)</returns>
    public static TValue? GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dic,
        TKey                           key
    ) {
        return dic.TryGetValue(key, out var val) ? val : default;
    }

    /// <summary>
    /// <inheritdoc cref="GetValueOrDefault{TKey,TValue}"/>
    /// </summary>
    /// <param name="dic">this <see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="key">the desired <see cref="KeyValuePair{TKey,TValue}.key"/></param>
    /// <param name="fallback">the <typeparamref name="TValue"/> returned if the <paramref name="key"/> isn't found</param>
    /// <typeparam name="TKey">the <see cref="KeyValuePair{TKey,TValue}.key"/> type</typeparam>
    /// <typeparam name="TValue">the <see cref="KeyValuePair{TKey,TValue}.value"/> type</typeparam>
    /// <typeparam name="TFallback">the <paramref name="fallback"/> type</typeparam>
    /// <returns><see cref="IDictionary{TKey,TValue}.this[TKey]"/>, if present; otherwise, <paramref name="fallback"/></returns>
    /// <remarks>
    /// Compared to the built-in <see cref="CollectionExtensions"/> method, <see cref="CollectionExtensions.GetValueOrDefault{TKey,TValue}(System.Collections.Generic.IReadOnlyDictionary{TKey,TValue},TKey)"/>.
    /// <ul>
    /// <li>This method works with <see cref="IDictionary{TKey,TValue}"/> instead of <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
    /// While concrete types like <see cref="Dictionary{TKey,TValue}"/> generally implement both <see cref="IDictionary{TKey,TValue}"/> and <see cref="IReadOnlyDictionary{TKey,TValue}"/>,
    /// code often passes around the most generic interface, which is <see cref="IDictionary{TKey,TValue}"/>.
    /// </li>
    /// <li>This method introduces an extra type parameter, <typeparamref name="TFallback"/>, which allows the nullability of the return type to be determined by the given <paramref name="fallback"/>.</li>
    /// </ul>
    /// </remarks>
    public static TFallback GetValueOrDefault<TKey, TValue, TFallback>(
        this IDictionary<TKey, TValue> dic,
        TKey                           key,
        TFallback                      fallback
    )
        where TValue : TFallback {
        return dic.TryGetValue(key, out var val) ? val : fallback;
    }

    /// <summary>
    /// Invokes <see cref="IDictionary{TKey,TValue}.this[TKey]"/> if the <paramref name="key"/> is present; otherwise, invokes <paramref name="fallbackSupplier"/>.
    /// </summary>
    /// <param name="dic">this <see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="key">the desired <see cref="KeyValuePair{TKey,TValue}.key"/></param>
    /// <param name="fallbackSupplier">a <see cref="Func{TResult}"/> that generates the fallback value if the <paramref name="key"/> isn't found</param>
    /// <typeparam name="TKey">the <see cref="KeyValuePair{TKey,TValue}.key"/> type</typeparam>
    /// <typeparam name="TValue">the <see cref="KeyValuePair{TKey,TValue}.value"/> type</typeparam>
    /// <returns><see cref="IDictionary{TKey,TValue}.this[TKey]"/>, if present; otherwise, the result of <paramref name="fallbackSupplier"/></returns>
    public static TValue GetValueOrSupply<TKey, TValue>(
        this IDictionary<TKey, TValue> dic,
        TKey                           key,
        Func<TValue>                   fallbackSupplier
    ) {
        return dic.TryGetValue(key, out var val) ? val : fallbackSupplier();
    }

    /// <summary>
    /// Invokes <see cref="IDictionary{TKey,TValue}.this[TKey]"/> if the <paramref name="key"/> is present; otherwise, invokes <see cref="fallbackSupplier"/> with the given <paramref name="key"/>.
    /// </summary>
    /// <param name="dic">this <see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="key">the desired <see cref="KeyValuePair{TKey,TValue}.key"/></param>
    /// <param name="fallbackSupplier">invoked with <paramref name="key"/> if it isn't found in the dictionary</param>
    /// <typeparam name="TKey">the <see cref="KeyValuePair{TKey,TValue}.key"/> type</typeparam>
    /// <typeparam name="TValue">the <see cref="KeyValuePair{TKey,TValue}.value"/> type</typeparam>
    /// <returns><see cref="IDictionary{TKey,TValue}.this[TKey]"/>, if present; otherwise, the result of <paramref name="fallbackSupplier"/>(<paramref name="key"/>)</returns>
    public static TValue GetValueOrSupply<TKey, TValue>(
        this IDictionary<TKey, TValue> dic,
        TKey                           key,
        Func<TKey, TValue>             fallbackSupplier
    ) {
        return dic.TryGetValue(key, out var val) ? val : fallbackSupplier(key);
    }

    /// <summary>
    /// Simplifies the implementation of <see cref="ConcurrentDictionary{TKey,TValue}"/>-based caching by "hiding" the <see cref="Lazy{T}"/>-ness of the <see cref="ConcurrentDictionary{TKey,TValue}.Values"/>.
    /// </summary>
    /// <remarks>
    /// The <typeparamref name="TIn"/> and <typeparamref name="TOut"/> type parameters are used to overcome the lack of co- / contra-variance in <see cref="Lazy{T}"/>.
    /// </remarks>
    /// <param name="concurrentDictionary">the actual holder for all of the cached values</param>
    /// <param name="key">the <see cref="KeyValuePair{TKey,TValue}.Key"/> of the entry</param>
    /// <param name="valueFactory">the <see cref="Func{TResult}"/> that will generate a new <see cref="KeyValuePair{TKey,TValue}.Value"/> if <paramref name="key"/> isn't found</param>
    /// <typeparam name="TKey">the actual <see cref="ConcurrentDictionary{TKey,TValue}"/> key type</typeparam>
    /// <typeparam name="TVal">the actual <see cref="ConcurrentDictionary{TKey,TValue}"/> value type</typeparam>
    /// <typeparam name="TIn">the type of the desired <paramref name="key"/></typeparam>
    /// <typeparam name="TOut">the type of the <paramref name="valueFactory"/> output</typeparam>
    /// <returns>the existing <typeparamref name="TVal"/> or the result of <paramref name="valueFactory"/></returns>
    public static TVal GetOrAddLazily<TKey, TVal, TIn, TOut>(this ConcurrentDictionary<TKey, Lazy<TVal>> concurrentDictionary, TIn key, Func<TIn, TOut> valueFactory)
        where TIn : TKey
        where TOut : TVal
        where TKey : notnull {
        Must.NotBeNull(key);
        return concurrentDictionary.GetOrAdd(key, k => new Lazy<TVal>(() => valueFactory((TIn)k!))).Value;
    }
}