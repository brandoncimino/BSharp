using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using FowlFever.BSharp.Exceptions;

namespace FowlFever.BSharp.Collections;

/// <summary>
/// TODO: Move extensions from <see cref="CollectionUtils"/> into here
/// </summary>
public static class DictionaryExtensions {
    public static TValue? GetOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dic,
        TKey                           key,
        TValue?                        fallback = default
    ) {
        return dic.TryGetValue(key, out var val) ? val : fallback;
    }

    public static TValue? GetOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dic,
        TKey                           key,
        Func<TValue>                   fallbackSupplier
    ) {
        if (dic == null) {
            throw new ArgumentNullException(nameof(dic));
        }

        if (key == null) {
            throw new ArgumentNullException(nameof(key));
        }

        if (fallbackSupplier == null) {
            throw new ArgumentNullException(nameof(fallbackSupplier));
        }

        return dic.ContainsKey(key) ? dic[key] : fallbackSupplier.Invoke();
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
    public static TVal GetOrAddLazily<TKey, TVal, TIn, TOut>(this ConcurrentDictionary<TKey, Lazy<TVal>> concurrentDictionary, [NotNull] TIn key, Func<TIn, TOut> valueFactory)
        where TIn : TKey
        where TOut : TVal {
        Must.NotBeNull(key);
        return concurrentDictionary.GetOrAdd(key, k => new Lazy<TVal>(() => valueFactory((TIn)k!))).Value;
    }
}