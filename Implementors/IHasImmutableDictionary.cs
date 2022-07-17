using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace FowlFever.Implementors;

/// <summary>
/// Delegates the implementation of <see cref="IImmutableDictionary{TKey,TValue}"/> to the <see cref="AsImmutableDictionary"/> property.
/// </summary>
/// <typeparam name="K">the type of the <see cref="IHasImmutableDictionary{K,V}.Keys"/></typeparam>
/// <typeparam name="V">the type of the <see cref="IHasImmutableDictionary{K,V}.Values"/></typeparam>
public interface IHasImmutableDictionary<K, V> : IImmutableDictionary<K, V> {
    protected IImmutableDictionary<K, V> AsImmutableDictionary { get; }

    #region Implementation

    IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()                                        => AsImmutableDictionary.GetEnumerator();
    IEnumerator IEnumerable.                                        GetEnumerator()                                        => ((IEnumerable)AsImmutableDictionary).GetEnumerator();
    int IReadOnlyCollection<KeyValuePair<K, V>>.                    Count                                                  => AsImmutableDictionary.Count;
    bool IReadOnlyDictionary<K, V>.                                 ContainsKey(K key)                                     => AsImmutableDictionary.ContainsKey(key);
    bool IReadOnlyDictionary<K, V>.                                 TryGetValue(K key, [MaybeNullWhen(false)] out V value) => AsImmutableDictionary.TryGetValue(key, out value);
    V IReadOnlyDictionary<K, V>.this[K                                            key] => AsImmutableDictionary[key];
    IEnumerable<K> IReadOnlyDictionary<K, V>.             Keys                                                   => AsImmutableDictionary.Keys;
    IEnumerable<V> IReadOnlyDictionary<K, V>.             Values                                                 => AsImmutableDictionary.Values;
    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Add(K                                    key, V value) => AsImmutableDictionary.Add(key, value);
    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.AddRange(IEnumerable<KeyValuePair<K, V>> pairs)        => AsImmutableDictionary.AddRange(pairs);
    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Clear()                                                => AsImmutableDictionary.Clear();
    bool IImmutableDictionary<K, V>.                      Contains(KeyValuePair<K, V>              pair)         => AsImmutableDictionary.Contains(pair);
    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.Remove(K                                 key)          => AsImmutableDictionary.Remove(key);
    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.RemoveRange(IEnumerable<K>               keys)         => AsImmutableDictionary.RemoveRange(keys);
    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItem(K                                key, V value) => AsImmutableDictionary.SetItem(key, value);
    IImmutableDictionary<K, V> IImmutableDictionary<K, V>.SetItems(IEnumerable<KeyValuePair<K, V>> items)                     => AsImmutableDictionary.SetItems(items);
    bool IImmutableDictionary<K, V>.                      TryGetKey(K                              equalKey, out K actualKey) => AsImmutableDictionary.TryGetKey(equalKey, out actualKey);

    #endregion
}