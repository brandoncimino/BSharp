using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Implementors;

/// <inheritdoc cref="IDictionary{K,V}"/>
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public interface IHasDictionary<K, V> : IDictionary<K, V>, IReadOnlyDictionary<K, V> {
    protected IDictionary<K, V>                                     AsDictionary    { get; }
    IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() => AsDictionary.GetEnumerator();

    IEnumerator IEnumerable.             GetEnumerator()                                    => ((IEnumerable)AsDictionary).GetEnumerator();
    void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item)                       => AsDictionary.Add(item);
    void ICollection<KeyValuePair<K, V>>.Clear()                                            => AsDictionary.Clear();
    bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item)                  => AsDictionary.Contains(item);
    void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) => AsDictionary.CopyTo(array, arrayIndex);
    bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V>   item) => AsDictionary.Remove(item);
    int ICollection<KeyValuePair<K, V>>. Count                             => AsDictionary.Count;
    bool ICollection<KeyValuePair<K, V>>.IsReadOnly                        => AsDictionary.IsReadOnly;
    void IDictionary<K, V>.              Add(K         key, V value)       => AsDictionary.Add(key, value);
    bool IDictionary<K, V>.              ContainsKey(K key)              => AsDictionary.ContainsKey(key);
    bool IReadOnlyDictionary<K, V>.      TryGetValue(K key, out V value) => AsDictionary.TryGetValue(key, out value);

    V IReadOnlyDictionary<K, V>.this[K key] => AsDictionary[key];
    IEnumerable<K> IReadOnlyDictionary<K, V>.Keys                            => AsDictionary.Keys;
    IEnumerable<V> IReadOnlyDictionary<K, V>.Values                          => AsDictionary.Values;
    bool IDictionary<K, V>.                  Remove(K      key)              => AsDictionary.Remove(key);
    bool IReadOnlyDictionary<K, V>.          ContainsKey(K key)              => AsDictionary.ContainsKey(key);
    bool IDictionary<K, V>.                  TryGetValue(K key, out V value) => AsDictionary.TryGetValue(key, out value);

    V IDictionary<K, V>.this[K key] {
        get => AsDictionary[key];
        set => AsDictionary[key] = value;
    }
    ICollection<K> IDictionary<K, V>.           Keys   => AsDictionary.Keys;
    ICollection<V> IDictionary<K, V>.           Values => AsDictionary.Values;
    int IReadOnlyCollection<KeyValuePair<K, V>>.Count  => AsDictionary.Count;
}