using System.Collections.Immutable;

namespace FowlFever.Implementors;

/// <summary>
/// Delegates the implementation of <see cref="IImmutableList{T}"/> to <see cref="AsImmutableArray"/>.
/// </summary>
/// <typeparam name="T">the type of the entries in <see cref="AsImmutableArray"/></typeparam>
public interface IHasImmutableArray<T> : IHasImmutableList<T> {
    /// <summary>
    /// The actual <see cref="ImmutableArray{T}"/> that this object delegates to.
    /// </summary>
    ImmutableArray<T> AsImmutableArray { get; }
    IImmutableList<T> IHasImmutableList<T>.AsImmutableList => AsImmutableArray;
}