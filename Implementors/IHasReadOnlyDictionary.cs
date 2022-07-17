using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace FowlFever.Implementors;

/// <inheritdoc/>
public interface IHasReadOnlyDictionary<K, V> : IReadOnlyDictionary<K, V> {
    protected IReadOnlyDictionary<K, V>                             AsReadOnlyDictionary                                   { get; }
    IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()                                        => AsReadOnlyDictionary.GetEnumerator();
    IEnumerator IEnumerable.                                        GetEnumerator()                                        => ((IEnumerable)AsReadOnlyDictionary).GetEnumerator();
    int IReadOnlyCollection<KeyValuePair<K, V>>.                    Count                                                  => AsReadOnlyDictionary.Count;
    bool IReadOnlyDictionary<K, V>.                                 ContainsKey(K key)                                     => AsReadOnlyDictionary.ContainsKey(key);
    bool IReadOnlyDictionary<K, V>.                                 TryGetValue(K key, [MaybeNullWhen(false)] out V value) => AsReadOnlyDictionary.TryGetValue(key, out value);
    V IReadOnlyDictionary<K, V>.this[K                                            key] => AsReadOnlyDictionary[key];
    IEnumerable<K> IReadOnlyDictionary<K, V>.Keys   => AsReadOnlyDictionary.Keys;
    IEnumerable<V> IReadOnlyDictionary<K, V>.Values => AsReadOnlyDictionary.Values;
}