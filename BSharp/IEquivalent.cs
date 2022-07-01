using System;

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
    IComparable<IComparable<T?>?> { }