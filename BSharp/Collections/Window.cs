using System.Collections;
using System.Collections.Generic;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Optional;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Collections;

/// <summary>
/// A <see cref="Queue{T}"/> with a fixed <see cref="Capacity"/> that automatically <see cref="Dequeue"/>s when that capacity is exceeded.
/// </summary>
/// <inheritdoc cref="Queue{T}"/>
[Experimental("It seems like this should already exist")]
internal class Window<T> : IReadOnlyCollection<T> {
    private readonly Queue<T> _queue;

    /// <summary>
    /// The maximum number of items that this <see cref="Window{T}"/> can hold.
    ///
    /// When additional items are <see cref="Enqueue"/>d beyond the <see cref="Capacity"/>, a corresponding amount is automatically <see cref="Dequeue"/>d.
    /// </summary>
    [NonNegativeValue]
    public int Capacity { get; }

    /// <summary>
    /// The actual number of items in the <see cref="Window{T}"/>, which will always be less than or equal to the <see cref="Capacity"/>.
    /// </summary>
    [NonNegativeValue]
    public int Count => _queue.Count;

    public Window(int capacity) {
        Must.Have(capacity > 0);
        _queue   = new Queue<T>(capacity);
        Capacity = capacity;
    }

    /// <summary>
    /// Adds an item to the <see cref="Window{T}"/>.
    ///
    /// If the <see cref="Count"/> is already at <see cref="Capacity"/>, then an item is <see cref="Dequeue"/>d and returned to make space for the new <paramref name="item"/>.
    /// </summary>
    /// <param name="item">the new <typeparamref name="T"/> entry</param>
    /// <returns>the item that was <see cref="Dequeue"/>d to make space, if any</returns>
    public Optional<T> Enqueue(T item) {
        Optional<T> removed = default;
        if (_queue.Count == Capacity) {
            removed = _queue.Dequeue();
        }

        _queue.Enqueue(item);
        return removed;
    }

    /// <inheritdoc cref="Queue{T}.Dequeue"/>
    public T Dequeue() => _queue.Dequeue();

    /// <inheritdoc cref="Queue{T}.TryDequeue"/>
    public bool TryDequeue(out T result) => _queue.TryDequeue(out result);

    /// <inheritdoc cref="Queue{T}.Peek"/>
    public T Peek() => _queue.Peek();

    /// <inheritdoc cref="Queue{T}.TryPeek"/>
    public bool TryPeek(out T result) => _queue.TryPeek(out result);

    public IEnumerator<T> GetEnumerator() {
        return _queue.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)_queue).GetEnumerator();
    }
}