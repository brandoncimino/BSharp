using System.Diagnostics.Contracts;

namespace FowlFever.Implementors;

/// <summary>
/// A specialized <see cref="IHas{T}"/> for <see cref="string"/>.
/// </summary>
/// <remarks>
/// <b><see cref="IHasString"/> vs. <see cref="IHas{T}">IHas&lt;string&gt;</see></b>
/// <ul>
/// <li>Use <see cref="IHas{T}">IHas&lt;string&gt;</see> for parameters and variables, because it offers covariance and such.</li>
/// <li>Implement <see cref="IHasString"/> directly, because it has a more robust set of interfaces - in particular, the fancy <see cref="Equals{T}"/> method.</li>
/// </ul>
/// </remarks>
public interface IHasString : IHas<string>, IEquatable<string?>, IEquatable<IHas<string?>?>, IComparable<string?>, IComparable<IHas<string?>?> {
    [Pure] public bool Equals<T>(T? other) where T : IHas<string?>?;

    /// <inheritdoc cref="string.Compare(string,string,StringComparison)"/>
    [Pure]
    public int CompareTo(string? other, StringComparison comparisonType = StringComparison.Ordinal);

    [Pure] int IComparable<string?>.CompareTo(string? other) => CompareTo(other);

    /// <inheritdoc cref="string.Compare(string,string,StringComparison)"/>
    [Pure]
    public int CompareTo<T>(T? other, StringComparison comparisonType = StringComparison.Ordinal) where T : IHas<string?>?;

    [Pure] int IComparable<IHas<string?>?>.CompareTo(IHas<string?>? other) => CompareTo(other);
}