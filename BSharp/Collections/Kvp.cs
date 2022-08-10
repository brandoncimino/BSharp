using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

/// <summary>
/// Utilities and extensions for working with <see cref="KeyValuePair{TKey,TValue}"/>s.
/// </summary>
public static class Kvp {
    #region Construction

    /// <inheritdoc cref="KeyValuePair.Create{TKey,TValue}"/>
    /// <remarks>
    /// Shorthand for <see cref="KeyValuePair.Create{TKey,TValue}"/>.
    /// </remarks>
    [Pure]
    public static KeyValuePair<K, V> Of<K, V>(K key, V val)
        where K : notnull => KeyValuePair.Create(key, val);

    /// <inheritdoc cref="KeyValuePair.Create{TKey,TValue}"/>
    /// <remarks>
    /// Generates the <see cref="KeyValuePair{TKey,TValue}.Key"/> and <see cref="KeyValuePair{TKey,TValue}.Value"/> via <see cref="Func{T,TResult}"/>tions.
    /// </remarks>
    [Pure]
    public static KeyValuePair<K, V> Of<T, K, V>(
        T                          input,
        [InstantHandle] Func<T, K> keySelector,
        [InstantHandle] Func<T, V> valSelector
    )
        where K : notnull => Of(keySelector(input), valSelector(input));

    /// <inheritdoc cref="Of{T,K,V}(T,Func{T,K},Func{T,V})"/>
    /// <seealso cref="Of{T,K,V}(T,Func{T,K},Func{T,V})"/>
    public static KeyValuePair<K, V> ToKeyValuePair<T, K, V>(this T input, Func<T, K> keySelector, Func<T, V> valSelector)
        where K : notnull => Of(input, keySelector, valSelector);

    /// <summary>
    /// Creates a <see cref="KeyValuePair{TKey,TValue}"/> where the <see cref="KeyValuePair{TKey,TValue}.Value"/> is the <paramref name="input"/> and the <see cref="KeyValuePair{TKey,TValue}.Key"/>
    /// is the result of the <paramref name="keySelector"/>.
    /// </summary>
    /// <param name="input">the original <typeparamref name="T"/>, which will become the <see cref="KeyValuePair{TKey,TValue}.Value"/></param>
    /// <param name="keySelector">takes <typeparamref name="T"/> ➡ returns <see cref="KeyValuePair{TKey,TValue}.Key"/></param>
    /// <typeparam name="T">the <paramref name="input"/> / <see cref="KeyValuePair{TKey,TValue}.Value"/> type</typeparam>
    /// <typeparam name="K">the <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <returns>a new <see cref="KeyValuePair{TKey,TValue}"/></returns>
    [Pure]
    public static KeyValuePair<K, T> Of<T, K>(T input, Func<T, K> keySelector)
        where K : notnull => Of(keySelector(input), input);

    /// <inheritdoc cref="Of{T,K}(T,Func{T,K})"/>
    /// <seealso cref="Of{T,K}(T,Func{T,K})"/>
    /// <remarks>Made this one a bit more verbose to prevent it from cluttering up auto-complete suggestions too much, since it's an extension on any type.</remarks>
    [Pure]
    public static KeyValuePair<K, T> ToKeyValuePair<T, K>(this T input, Func<T, K> keySelector)
        where K : notnull => Of(input, keySelector);

    /// <summary>
    /// Converts from <see cref="ValueTuple{T1,T2}"/> ➡ <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="tuple">the original <see cref="ValueTuple{TKey,TValue}"/></param>
    /// <typeparam name="K">the <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <typeparam name="V">the <see cref="KeyValuePair{TKey,TValue}.Value"/> type</typeparam>
    /// <returns>a new <see cref="KeyValuePair{TKey,TValue}"/></returns>
    public static KeyValuePair<K, V> ToKeyValuePair<K, V>(this (K key, V value) tuple)
        where K : notnull => KeyValuePair.Create(tuple.key, tuple.value);

    public static KeyValuePair<K, T> ToKeyValuePair<T, K>(this T primaryKeyed)
        where T : IPrimaryKeyed<K> {
        return KeyValuePair.Create(primaryKeyed.PrimaryKey, primaryKeyed);
    }

    #endregion

    #region Invoking Functions

    /// <summary>
    /// Invokes a <see cref="Delegate"/>, using the <paramref name="keyValuePair"/>'s <see cref="KeyValuePair{TKey,TValue}.Key"/> and <see cref="KeyValuePair{TKey,TValue}.Value"/> as arguments.
    /// </summary>
    /// <param name="delgato">the <see cref="Delegate"/> being invoked</param>
    /// <param name="keyValuePair">the <see cref="KeyValuePair{TKey,TValue}.Key"/> and <see cref="KeyValuePair{TKey,TValue}.Value"/> arguments to the <paramref name="delgato"/></param>
    /// <typeparam name="K">the <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <typeparam name="V">the <see cref="KeyValuePair{TKey,TValue}.Value"/> type</typeparam>
    public static void Invoke<K, V>(this Action<K, V> delgato, KeyValuePair<K, V> keyValuePair)
        where K : notnull => delgato(keyValuePair.Key, keyValuePair.Value);

    /// <inheritdoc cref="Invoke{K,V}(System.Action{K,V},System.Collections.Generic.KeyValuePair{K,V})"/>
    /// <typeparam name="OUT">the return type</typeparam>
    /// <returns>the <typeparamref name="OUT"/> result</returns>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment", Justification = "Inherited")]
    public static OUT Invoke<K, V, OUT>(this Func<K, V, OUT> delgato, KeyValuePair<K, V> keyValuePair)
        where K : notnull => delgato(keyValuePair.Key, keyValuePair.Value);

    /// <summary>
    /// Invokes a <see cref="Delegate"/> that operates on <see cref="KeyValuePair{TKey,TValue}"/>s using separate <paramref name="key"/> and <paramref name="val"/> arguments.
    /// </summary>
    /// <param name="delgato">a <see cref="Delegate"/> that operates on <see cref="KeyValuePair{TKey,TValue}"/>s</param>
    /// <param name="key">the <see cref="KeyValuePair{TKey,TValue}.Key"/></param>
    /// <param name="val">the <see cref="KeyValuePair{TKey,TValue}.Value"/></param>
    /// <typeparam name="K">the <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <typeparam name="V">the <see cref="KeyValuePair{TKey,TValue}.Value"/> type</typeparam>
    public static void Invoke<K, V>(this Action<KeyValuePair<K, V>> delgato, K key, V val)
        where K : notnull
        => delgato(Of(key, val));

    /// <inheritdoc cref="Invoke{K,V}(System.Action{System.Collections.Generic.KeyValuePair{K,V}},K,V)"/>
    /// <typeparam name="OUT">the return type</typeparam>
    /// <returns>the <typeparamref name="OUT"/> result</returns>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment", Justification = "Inherited")]
    public static OUT Invoke<K, V, OUT>(this Func<KeyValuePair<K, V>, OUT> func, K key, V val)
        where K : notnull => func(Of(key, val));

    #endregion

    /// <summary>
    /// Converts from <see cref="KeyValuePair{TKey,TValue}"/> ➡ <see cref="ValueTuple{T1,T2}"/>.
    /// </summary>
    /// <param name="keyValuePair">the original <see cref="KeyValuePair{TKey,TValue}"/></param>
    /// <typeparam name="K">the <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <typeparam name="V">the <see cref="KeyValuePair{TKey,TValue}.Value"/> type</typeparam>
    /// <returns>(<see cref="KeyValuePair{TKey,TValue}.Key"/>, <see cref="KeyValuePair{TKey,TValue}.Value"/>)</returns>
    public static (K, V) ToTuple<K, V>(this KeyValuePair<K, V> keyValuePair)
        where K : notnull {
        return (keyValuePair.Key, keyValuePair.Value);
    }

    #region Transformations

    [Pure]
    public static KeyValuePair<K, NEW> TransformValue<K, OLD, NEW>(this KeyValuePair<K, OLD> keyValuePair, Func<K, OLD, NEW> valueSelector)
        where K : notnull {
        return KeyValuePair.Create(keyValuePair.Key, valueSelector.Invoke(keyValuePair));
    }

    [Pure]
    public static KeyValuePair<NEW, V> TransformKey<OLD, V, NEW>(this KeyValuePair<OLD, V> keyValuePair, Func<OLD, V, NEW> keySelector)
        where OLD : notnull
        where NEW : notnull {
        return KeyValuePair.Create(
            keySelector(keyValuePair.Key, keyValuePair.Value),
            keyValuePair.Value
        );
    }

    #endregion

    #region Non-generic DictionaryEntry

    /// <summary>
    /// Converts a <see cref="DictionaryEntry"/> to a <see cref="KeyValuePair{TKey,TValue}"/> by casting its <see cref="KeyValuePair{TKey,TValue}.Key"/> and <see cref="KeyValuePair{TKey,TValue}.Value"/>.
    /// </summary>
    /// <remarks>
    /// This performs an explicit hard-cast - i.e. <c><![CDATA[(K)key]]></c> - because using the <c>as</c> or <c>instanceof</c> operators or the <see cref="Enumerable.Cast{TResult}"/> function
    /// wouldn't take user-defined <c>implicit</c> or <c>explicit</c> conversion <c>operator</c>s into account.
    /// </remarks>
    /// <param name="entry">this <see cref="DictionaryEntry"/></param>
    /// <typeparam name="K">the <see cref="KeyValuePair{TKey,TValue}"/>'s <see cref="KeyValuePair{TKey,TValue}.Key"/> type</typeparam>
    /// <typeparam name="V">the <see cref="KeyValuePair{TKey,TValue}"/>'s <see cref="KeyValuePair{TKey,TValue}.Value"/> type</typeparam>
    /// <returns>an equivalent <see cref="KeyValuePair{TKey,TValue}"/></returns>
    /// <exception cref="InvalidCastException">if the <see cref="DictionaryEntry"/>'s <see cref="DictionaryEntry.Key"/> or <see cref="DictionaryEntry.Value"/> couldn't be cast to the appropriate type</exception>
    [Pure]
    public static KeyValuePair<K, V?> ToKeyValuePair<K, V>(this DictionaryEntry entry)
        where K : notnull {
        [return: NotNullIfNotNull("o")]
        static T? TryCastObj<T>(object? o) {
            return o switch {
                T t => t,
                _   => (T?)o,
            };
        }

        try {
            var key = TryCastObj<K>(entry.Key);
            var val = TryCastObj<V>(entry.Value);
            return Of(key, val);
        }
        catch (Exception e) {
            var fromType = (entry.Key.GetType().Name, entry.Value?.GetType().Name ?? "⛔");
            throw new InvalidCastException($"Unable to cast the {nameof(DictionaryEntry)} {entry} from {fromType} ➡ <{typeof(K).Name}, {typeof(V).Name}>!");
        }
    }

    #endregion
}