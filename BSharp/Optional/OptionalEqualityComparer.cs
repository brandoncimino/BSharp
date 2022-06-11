using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Optional;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// 📝 NOTE: As can be seen in <a href="https://referencesource.microsoft.com/#mscorlib/system/collections/generic/equalitycomparer.cs,140">the actual .NET source code</a>,
/// <see cref="IEqualityComparer{T}.GetHashCode(T)"/> is absolutely null safe. Plus, it definitely was yesterday. I don't know whats gotten into Rider.
/// </remarks>
public class OptionalEqualityComparer<T> : EqualityComparer<T?>, IEqualityComparer<IOptional<T?>?> {
    private IEqualityComparer<T?> CanonComparer { get; }

    public OptionalEqualityComparer(IEqualityComparer<T?>? canonComparer = default) {
        CanonComparer = canonComparer ?? Default;
    }

    public int GetHashCode(IOptional<T?>? obj) {
        return (typeof(T?), obj?.HasValue, obj.GetValueOrDefault()).GetHashCode();
    }

    public override bool Equals(T? x, T? y) => CanonComparer.Equals(x, y);

    public override int GetHashCode(T? obj) => CanonComparer.GetHashCode(obj);

    /// <summary>
    /// Tests the underlying values of <see cref="a"/> and <see cref="b"/> for equality.
    /// <ul>
    /// <li>If <b>both</b> <see cref="a"/> and <see cref="b"/> are <b>empty</b>, then they are considered <b>equal</b>.</li>
    /// <li>If <b>both</b> <see cref="a"/> and <see cref="b"/> are <c>null</c>, then they are considered <b>equal</b>.</li>
    /// <li>If <b>both</b> <see cref="a"/> and <see cref="b"/> <i><b>contain</b></i> <c>null</c>, then they are considered <b>equal</b>.</li>
    /// <li>A <c>null</c> <see cref="IOptional{T}"/> is <b><i>NOT</i></b> considered equal to an <see cref="IOptional{T}"/> with a <c>null</c> <see cref="IOptional{T}.Value"/>!</li>
    /// </ul>
    /// </summary>
    /// <remarks>
    /// I made this so that I had the same logic across all of the different <see cref="System.IEquatable{T}"/> and <c>==</c>
    /// operator comparisons in <see cref="Optional{T}"/>, <see cref="FailableFunc{TValue}"/>, etc.
    /// <p/>
    /// In fancy language, this method provides a default equality comparator for <see cref="IOptional{T}"/> implementations.
    /// </remarks>
    /// <param name="a">aka "x", aka "left-hand side"</param>
    /// <param name="b">aka "y", aka "right-hand side"</param>
    /// <typeparam name="T">the actual type of the underlying <see cref="IOptional{T}.Value"/>s</typeparam>
    /// <returns>the equality of the underlying <see cref="IOptional{T}.Value"/>s of <see cref="a"/> and <see cref="b"/></returns>
    [ContractAnnotation("a:null, b:null => true")]
    [ContractAnnotation("a:null, b:notnull => false")]
    [ContractAnnotation("a:notnull, b:null => false")]
    public bool Equals(IOptional<T?>? a, IOptional<T?>? b) {
        // return true if EITHER:
        // - a & b are the same object, OR
        // - a & b are both null
        if (ReferenceEquals(a, b)) {
            return true;
        }

        // since at this point we know that they can't _both_ be null, if _either_ of them is null, return false
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) {
            return false;
        }

        // if a & b are both EITHER:
        // - not empty, OR
        // - empty
        if (a.HasValue == b.HasValue) {
            // if a is empty, then b is empty, and two empties are considered to match, so we return TRUE
            // otherwise, we compare the actual values
            return !a.HasValue || Equals(a.Value, b.Value);
        }

        return false;
    }

    /// <summary>
    /// Compares the <see cref="IOptional{T}.Value"/> of an <see cref="IOptional{T}"/> to a vanilla <typeparamref name="T"/> value
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>A null <see cref="IOptional{T}"/> should <b>not</b> be considered equal to a null <typeparamref name="T"/></li>
    /// </ul>
    /// </remarks>
    /// <param name="a">an <see cref="IOptional{T}"/></param>
    /// <param name="b">a vanilla <typeparamref name="T"/> value</param>
    /// <typeparam name="T">the underlying type being compared</typeparam>
    /// <returns>the equality of (<paramref name="a"/>.<see cref="IOptional{T}.Value"/>) and (<paramref name="b"/>)</returns>
    public bool Equals([NotNullWhen(true)] IOptional<T?>? a, T? b) => a is { HasValue: true } && Equals(a.Value, b);

    /// <summary>
    /// Compares a vanilla <typeparamref name="T"/> value to the underlying <see cref="IOptional{T}.Value"/> of an <see cref="IOptional{T}"/>.
    /// </summary>
    /// <remarks>
    /// Behind the scenes, this flips <paramref name="x"/> and <paramref name="b"/> and sends them to <see cref="Equals(FowlFever.BSharp.Optional.IOptional{T?}?,T?)"/>
    /// </remarks>
    /// <param name="x">a vanilla <typeparamref name="T"/> value</param>
    /// <param name="b">an <see cref="IOptional{T}"/></param>
    /// <typeparam name="T">the underlying type being compared</typeparam>
    /// <returns>the equality of (<paramref name="x"/>) and (<paramref name="b"/>.<see cref="IOptional{T}.Value"/>)</returns>
    public bool Equals(T? x, [NotNullWhen(true)] IOptional<T?>? b) => Equals(b, x);

    public bool Equals(IOptional<T?>? x, IHas<T?>?      y) => Equals(x,                     y.GetValueOrDefault());
    public bool Equals(IHas<T?>?      x, IOptional<T?>? y) => Equals(x.GetValueOrDefault(), y);
}