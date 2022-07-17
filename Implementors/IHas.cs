namespace FowlFever.Implementors;

/// <summary>
/// Indicates that the implementer contains a meaningful <typeparamref name="T"/> instance, stored in the <see cref="Value"/> parameter.
/// </summary>
/// <typeparam name="T">the <see cref="Type"/> of the underlying <see cref="IHas{T}.Value"/></typeparam>
public interface IHas<out T> {
    /// <summary>
    /// The actual, underlying <typeparamref name="T"/> instance.
    /// </summary>
    T Value { get; }
}