using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Collections;

/// <summary>
/// An multi-thread-safe <see cref="ISet{T}"/>.
/// </summary>
/// <remarks>
/// Backed by a <see cref="ConcurrentDictionary{TKey,TValue}"/>'s <see cref="ConcurrentDictionary{TKey,TValue}.Keys"/>.
/// </remarks>
/// <typeparam name="T">the type of the entries in the <see cref="ISet{T}"/></typeparam>
public class ConcurrentSet<T> : ISet<T> {
    private readonly ConcurrentDictionary<T, byte> _myDic = new ConcurrentDictionary<T, byte>();
    /// <summary>
    /// TODO: see if there's a more efficient way to retrieve the <see cref="ConcurrentDictionary{TKey,TValue}.Keys"/> as a <see cref="ISet{T}"/>
    /// </summary>
    private ISet<T> KeySet => _myDic.Keys.ToImmutableHashSet();
    private NotSupportedException NotSupported([CallerMemberName] string? unsupportedMethod = default) => new($"✋ {unsupportedMethod} isn't supported by {GetType().Name}!");

    public IEnumerator<T>   GetEnumerator()                          => _myDic.Keys.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()                          => _myDic.Keys.GetEnumerator();
    void ICollection<T>.    Add(T                             item)  => _myDic.TryAdd(item, default);
    public bool             IsProperSubsetOf(IEnumerable<T>   other) => KeySet.IsProperSubsetOf(other);
    public bool             IsProperSupersetOf(IEnumerable<T> other) => KeySet.IsProperSupersetOf(other);
    public bool             IsSubsetOf(IEnumerable<T>         other) => KeySet.IsSubsetOf(other);
    public bool             IsSupersetOf(IEnumerable<T>       other) => KeySet.IsSupersetOf(other);
    public bool             Overlaps(IEnumerable<T>           other) => _myDic.Keys.ContainsAny(other);
    public bool             SetEquals(IEnumerable<T>          other) => KeySet.SetEquals(other);
    public bool             Add(T                             item)  => _myDic.TryAdd(item, default);
    public void             Clear()                                  => _myDic.Clear();
    public bool             Contains(T item)                         => _myDic.ContainsKey(item);
    public bool             Remove(T   item)                         => _myDic.TryRemove(item, out _);
    public int              Count                                    => _myDic.Count;
    bool ICollection<T>.    IsReadOnly                               => false;

    #region Unsupported ISet<T> Methods

    void ISet<T>.       SymmetricExceptWith(IEnumerable<T> other)                 => throw NotSupported();
    void ISet<T>.       ExceptWith(IEnumerable<T>          other)                 => throw NotSupported();
    void ISet<T>.       IntersectWith(IEnumerable<T>       other)                 => throw NotSupported();
    void ICollection<T>.CopyTo(T[]                         array, int arrayIndex) => NotSupported();
    void ISet<T>.       UnionWith(IEnumerable<T>           other) => throw NotSupported();

    #endregion
}