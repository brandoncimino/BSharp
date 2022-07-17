using System;

using FowlFever.Implementors;

namespace FowlFever.BSharp;

/// TODO: Move into <see cref="FowlFever.Implementors"/>
public static class HasExtensions {
    [Obsolete($"Please use {nameof(OrDefault)} instead")]
    public static T? GetValueOrDefault<T>(this IHas<T>? self) => self.OrDefault();

    public static T? OrDefault<T>(this IHas<T>? self) => self == null ? default : self.Value;

    /// <param name="self">this <see cref="IHas{T}"/></param>
    /// <typeparam name="T">the type of the wrapped <see cref="IHas{T}.Value"/></typeparam>
    /// <returns>this <see cref="IHas{T}"/>.<see cref="IHas{T}.Value"/></returns>
    public static T Get<T>(this IHas<T> self) => self.Value;

    public static Supplied<T> Supplied<T>(this IHas<T> self) => new(self.Get);

    public static bool ValueEquals<T>(this IHas<T?>? self, IEquatable<T?>? other) {
        return other switch {
            T t         => self.OrDefault()?.Equals(t),
            IHas<T> has => self.OrDefault()?.Equals(has.OrDefault()),
            _           => other?.Equals(self.OrDefault())
        } == true;
    }

    public static bool             ValueEquals<T>(this  IHas<T?>? self, IHas<T?>? other) => self.OrDefault()?.Equals(other.OrDefault()) == true;
    public static ComparisonResult ComparedWith<T>(this IHas<T?>? self, T?        other) => self.OrDefault().ComparedWith(other);
    public static ComparisonResult ComparedWith<T>(this IHas<T?>? self, IHas<T?>? other) => Comparator<T>.Default.Compare(self.OrDefault(), other.OrDefault());

    public static ComparisonResult ComparedWith<T>(this IHas<T?>? self, IComparable<T?>? other) => other switch {
        T t         => self.ComparedWith(t),
        IHas<T> has => self.ComparedWith(has),
        _           => Comparator<T>.Default.Compare(self.OrDefault(), other)
    };
}