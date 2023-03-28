using System;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Memory;

/// <summary>
/// A super-lightweight enumerator for 1 or 0 objects.
/// </summary>
/// <typeparam name="T">the object type</typeparam>
public ref struct Onerator<T> {
    /// <summary>
    /// We want to make sure the <c>default</c> <see cref="Onerator{T}"/>s is considered empty, so we need to base it on something that <c>default</c>s to <c>false</c>.
    /// <p/>
    /// This makes the grammar really confusing, though, so you should actually reference <see cref="_done"/> instead. 
    /// </summary>
    private bool _notDone;

    /// <summary>
    /// The inverse of <see cref="_notDone"/>, which should be preferred because the grammar of it is much less confusing than <see cref="_notDone"/>.
    /// </summary>
    private bool _done {
        get => !_notDone;
        set => _notDone = !value;
    }

    /// <remarks>
    /// We don't need to worry about un-setting <see cref="Current"/> because this is a <see cref="Type.IsByRefLike"/> struct, which has a naturally limited lifespan.
    /// </remarks>
    [UsedImplicitly]
    public T Current { get; }

    public Onerator(T thing) {
        Current  = thing;
        _notDone = true;
    }

    public bool MoveNext() {
        if (_done) {
            return false;
        }

        _done = true;
        return true;
    }

    [UsedImplicitly] public Onerator<T> GetEnumerator() => this;
}