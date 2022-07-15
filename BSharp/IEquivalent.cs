using System;
using System.Collections.Generic;

namespace FowlFever.BSharp;

/// <summary>
/// Politely reminds you to implement a bunch of <see cref="IEquatable{T}"/> and <see cref="IComparable{T}"/> interfaces.
/// </summary>
/// <typeparam name="T">the true self</typeparam>
public interface IEquivalent<T> :
    IEquatable<T?>,
    IEquatable<IHas<T?>?>,
    IEquatable<IEquatable<T?>?>,
    IComparable<T?>,
    IComparable<IHas<T?>?>,
    IComparable<IComparable<T?>?> {
    protected IEqualityComparer<T?> CanonEquality => EqualityComparer<T?>.Default;
    protected IComparer<T?>         CanonComparer => Comparer<T?>.Default;

    T                          Equivalent              { get; }
    bool IEquatable<T?>.       Equals(T?        other) => CanonEquality.Equals(Equivalent, other);
    bool IEquatable<IHas<T?>?>.Equals(IHas<T?>? other) => CanonEquality.Equals(Equivalent, other.OrDefault());

    bool IEquatable<IEquatable<T?>?>.Equals(IEquatable<T?>? other) => other switch {
        T t         => Equivalent?.Equals(t),
        IHas<T> has => Equivalent?.Equals(has.OrDefault()),
        _           => other?.Equals(Equivalent)
    } == true;

    int IComparable<T?>.              CompareTo(T?               other) => CanonComparer.Compare(Equivalent, other);
    int IComparable<IHas<T?>?>.       CompareTo(IHas<T?>?        other) => CanonComparer.Compare(Equivalent, other.OrDefault());
    int IComparable<IComparable<T?>?>.CompareTo(IComparable<T?>? other) => other == null ? 1 : other.CompareTo(Equivalent) * -1;
}