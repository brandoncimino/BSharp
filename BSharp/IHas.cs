using System;

namespace FowlFever.BSharp;

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

public static class HasExtensions {
    public static T? GetValueOrDefault<T>(this IHas<T>? self) {
        return self == null ? default : self.Value;
    }

    /// <param name="self">this <see cref="IHas{T}"/></param>
    /// <typeparam name="T">the type of the wrapped <see cref="IHas{T}.Value"/></typeparam>
    /// <returns>this <see cref="IHas{T}"/>.<see cref="IHas{T}.Value"/></returns>
    public static T Get<T>(this IHas<T> self) => self.Value;

    public static Supplied<T> Supplied<T>(this IHas<T> self) => new(self.Get);
}