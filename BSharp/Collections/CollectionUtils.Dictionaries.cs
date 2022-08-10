using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

public static partial class CollectionUtils {
    #region Dictionaries After Dark

    //TODO: move to a `DictionaryExtensions` class

    /// <summary>
    /// Shorthand to go from an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>s back to an <see cref="IDictionary{TKey,TValue}"/> via <see cref="Enumerable.ToDictionary{TSource,TKey,TValue}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey},System.Func{TSource,TValue})"/>
    /// </summary>
    /// <param name="source">a collection of <see cref="KeyValuePair{TKey,TValue}"/>s</param>
    /// <typeparam name="TKey">the type of <see cref="IDictionary{TKey,TValue}.Keys"/></typeparam>
    /// <typeparam name="TVal">the type of <see cref="IDictionary{TKey,TValue}.Values"/></typeparam>
    /// <returns>a new <see cref="IDictionary{TKey,TValue}"/></returns>
    public static IDictionary<TKey, TVal> ToDictionary<TKey, TVal>([InstantHandle] this IEnumerable<KeyValuePair<TKey, TVal>> source)
        where TKey : notnull {
        return new Dictionary<TKey, TVal>(source);
    }

    /// <remarks>
    /// Similar to <see cref="Enumerable.ToDictionary{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/>, but produces
    /// a sequence of <see cref="KeyValuePair{T,T}"/>s so you can branch of into things like:
    /// <ul>
    /// <li><see cref="ImmutableDictionary.ToImmutableDictionary{TKey,TValue}(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{TKey,TValue}})"/></li>
    /// <li><see cref="ToConcurrentDictionary{TKey,TVal}"/></li>
    /// </ul>
    /// </remarks>
    /// <param name="source">this <see cref="IEnumerable{T}"/></param>
    /// <param name="keySelector">generates the <see cref="KeyValuePair{TKey,TValue}.Key"/> for a <typeparamref name="T"/></param>
    /// <param name="valSelector">generates the <see cref="KeyValuePair{TKey,TValue}.Value"/> for a <typeparamref name="T"/></param>
    /// <typeparam name="T">the type of entries in the <paramref name="source"/></typeparam>
    /// <typeparam name="TKey">the <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <typeparam name="TVal">the <see cref="KeyValuePair{TKey,TValue}.Value"/> type</typeparam>
    /// <returns>a sequence of <see cref="KeyValuePair{TKey,TValue}"/>s</returns>
    public static IEnumerable<KeyValuePair<TKey, TVal>> ToKeyValuePairs<T, TKey, TVal>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TVal> valSelector) {
        return source.Select(it => KeyValuePair.Create(keySelector(it), valSelector(it)));
    }

    /// <summary>
    /// Selects a <see cref="KeyValuePair{TKey,TValue}.Key"/> from each <typeparamref name="T"/> element, combining them into <see cref="KeyValuePair{TKey,TValue}"/>s.
    /// </summary>
    /// <remarks>
    /// Similar to <see cref="Enumerable.ToDictionary{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/>, but produces
    /// a sequence of <see cref="KeyValuePair{T,T}"/>s so you can branch of into things like:
    /// <ul>
    /// <li><see cref="ImmutableDictionary.ToImmutableDictionary{TKey,TValue}(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{TKey,TValue}})"/></li>
    /// <li><see cref="ToConcurrentDictionary{TKey,TVal}"/></li>
    /// </ul>
    /// </remarks>
    /// <param name="source">this <see cref="IEnumerable{T}"/></param>
    /// <param name="keySelector">generates the <see cref="KeyValuePair{TKey,TValue}.Key"/>s</param>
    /// <typeparam name="T">the type of the entries in the <paramref name="source"/></typeparam>
    /// <typeparam name="TKey">the <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <returns>a sequence of <see cref="KeyValuePair{TKey,TValue}"/>s</returns>
    public static IEnumerable<KeyValuePair<TKey, T>> ToKeyValuePairs<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        where TKey : notnull {
        return source.Select(it => KeyValuePair.Create(keySelector(it), it));
    }

    /// <inheritdoc cref="M:System.Collections.Concurrent.ConcurrentDictionary`2.#ctor(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{`0,`1}})"/>
    public static ConcurrentDictionary<TKey, TVal> ToConcurrentDictionary<TKey, TVal>([InstantHandle] this IEnumerable<KeyValuePair<TKey, TVal>> source)
        where TKey : notnull {
        return new ConcurrentDictionary<TKey, TVal>(source);
    }

    /// <summary>
    /// Filters out <see cref="KeyValuePair{TKey,TValue}"/>s with <c>null</c> <see cref="KeyValuePair{TKey,TValue}.Value"/>s.
    /// </summary>
    /// <param name="source">a sequence of <see cref="KeyValuePair{TKey,TValue}"/>s</param>
    /// <typeparam name="TKey">the <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <typeparam name="TVal">the <see cref="KeyValuePair{TKey,TValue}.Value"/> type</typeparam>
    /// <returns>the <see cref="KeyValuePair{TKey,TValue}"/>s with non-<c>null</c> <see cref="KeyValuePair{TKey,TValue}.Value"/>s</returns>
    public static IEnumerable<KeyValuePair<TKey, TVal>> NonNull<TKey, TVal>(this IEnumerable<KeyValuePair<TKey, TVal>> source)
        where TKey : notnull {
        return source.Where(it => it.Value != null);
    }

    /// <summary>
    /// A variation of the built-int <see cref="Enumerable.ToDictionary{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/>
    /// that "deconstructs" the <see cref="KeyValuePair{TKey,TValue}"/> to a separate <see cref="KeyValuePair{TKey,TValue}.Key"/> and <see cref="KeyValuePair{TKey,TValue}.Value"/>.
    /// </summary>
    /// <param name="source">the original <see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="selector">the <see cref="Func{TResult}"/> that transforms each <see cref="KeyValuePair{TKey,TValue}"/></param>
    /// <typeparam name="TKey">the <see cref="Type"/> of <paramref name="source"/>'s <see cref="IDictionary{TKey,TValue}.Keys"/></typeparam>
    /// <typeparam name="TOld">the <see cref="Type"/> of <paramref name="source"/>'s <see cref="IDictionary{TKey,TValue}.Values"/></typeparam>
    /// <typeparam name="TNew">the <see cref="Type"/> of the <b>new</b> <see cref="IDictionary.Values"/></typeparam>
    /// <returns>a new <see cref="Dictionary{TKey,TValue}"/></returns>
    public static Dictionary<TKey, TNew> ToDictionary<TKey, TOld, TNew>(this IEnumerable<KeyValuePair<TKey, TOld>> source, Func<TKey, TOld, TNew> selector)
        where TKey : notnull {
        return source.ToDictionary(
            kvp => kvp.Key,
            kvp => selector(kvp.Key, kvp.Value)
        );
    }

    /// <summary>
    /// Selects the <see cref="KeyValuePair{TKey,TValue}.Key"/>s from a "pseudo-<see cref="IDictionary{TKey,TValue}"/>".
    /// </summary>
    /// <param name="source">a sequence of <see cref="KeyValuePair{TKey,TValue}"/>s <i>(such as an <see cref="IDictionary{TKey,TValue}"/>)</i></param>
    /// <typeparam name="K">the <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <typeparam name="V">the <see cref="KeyValuePair{TKey,TValue}.Value"/> type</typeparam>
    /// <returns>each <see cref="KeyValuePair{TKey,TValue}.Key"/></returns>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<K> Keys<K, V>([NoEnumeration] this IEnumerable<KeyValuePair<K, V>> source) {
        return source switch {
            IDictionary<K, V> dic           => dic.Keys,
            IImmutableDictionary<K, V> iDic => iDic.Keys,
            _                               => source.Select(kvp => kvp.Key),
        };
    }

    /// <summary>
    /// Selects the <see cref="KeyValuePair{TKey,TValue}.Value"/>s from a "pseudo-<see cref="IDictionary{TKey,TValue}"/>".
    /// </summary>
    /// <param name="source">a sequence of <see cref="KeyValuePair{TKey,TValue}"/>s <i>(such as an <see cref="IDictionary{TKey,TValue}"/>)</i></param>
    /// <typeparam name="K">the <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <typeparam name="V">the <see cref="KeyValuePair{TKey,TValue}.Value"/> type</typeparam>
    /// <returns>each <see cref="KeyValuePair{TKey,TValue}.Value"/></returns>
    [Pure]
    [LinqTunnel]
    public static IEnumerable<V> Values<K, V>([NoEnumeration] this IEnumerable<KeyValuePair<K, V>> source) {
        return source switch {
            IDictionary<K, V> dic           => dic.Values,
            IImmutableDictionary<K, V> iDic => iDic.Values,
            _                               => source.Select(kvp => kvp.Value),
        };
    }

    #region Non-Generic IDictionary

    /// <summary>
    /// Converts each <see cref="DictionaryEntry"/> in a non-generic <see cref="IDictionary"/> into a <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="dictionary">this non-generic <see cref="IDictionary"/></param>
    /// <typeparam name="K">the <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <typeparam name="V">the <see cref="KeyValuePair{TKey,TValue}.Value"/> type</typeparam>
    /// <returns>a sequence of <see cref="KeyValuePair{TKey,TValue}"/>s</returns>
    public static IEnumerable<KeyValuePair<K, V?>> ToKeyValuePairs<K, V>(this IDictionary dictionary)
        where K : notnull {
        foreach (DictionaryEntry o in dictionary) {
            yield return o.ToKeyValuePair<K, V>();
        }
    }

    /// <inheritdoc cref="ToKeyValuePairs{K,V}(System.Collections.IDictionary)"/>
    /// <param name="keySelector">generates the <see cref="KeyValuePair{TKey,TValue}.Key"/>s</param>
    /// <param name="valSelector">generates the <see cref="KeyValuePair{TKey,TValue}.Value"/>s</param>
    // ReSharper disable once InvalidXmlDocComment
    public static IEnumerable<KeyValuePair<K, V>> ToKeyValuePairs<K, V>(this IDictionary dictionary, Func<object, K> keySelector, Func<object?, V> valSelector)
        where K : notnull {
        foreach (DictionaryEntry o in dictionary) {
            yield return Kvp.Of(keySelector(o.Key), valSelector(o.Value));
        }
    }

    #endregion

    #endregion
}