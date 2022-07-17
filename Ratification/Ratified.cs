using FowlFever.Implementors;

namespace Ratified;

/// <summary>
/// A wrapper around a <see cref="Value"/> stored as a <see cref="RatifiedProp{T}"/>.
/// </summary>
/// <typeparam name="T">the type of the <see cref="Value"/></typeparam>
public abstract record Ratified<T> : IHas<T>
    where T : notnull {
    /// <summary>
    /// The underlying <see cref="RatifiedProp{T}"/>. 
    /// </summary>
    private RatifiedProp<T> RatifiedValue { get; }

    /// <inheritdoc/>
    /// <remarks>
    /// Delegates to the internal <see cref="RatifiedValue"/>.
    /// </remarks>
    public T Value {
        get => RatifiedValue.Value;
        init => RatifiedValue.Value = value;
    }

    protected Ratified(RatifiedProp<T> ratifiedProp) => RatifiedValue = ratifiedProp;
    protected Ratified(IRatifier<T>    ratifier, T initialValue, MustRatify ratifyInitialValue) : this(ratifier.CreateRatifiedProp(initialValue, ratifyInitialValue)) { }
}