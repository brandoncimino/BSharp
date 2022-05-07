namespace FowlFever.Clerical.Validated;

/// <summary>
/// A base class for wrappers around a single <see cref="string"/> value.
/// </summary>
/// <remarks>
/// The goal of this class is that working with a <see cref="ValidatedString"/> should be as close to working with a normal <see cref="string"/>
/// as possible.
/// <p/>
/// To this end, <see cref="ValidatedString"/> provides a simple <see cref="IEquatable{T}"/> implementation
/// and an <c>implicit operator</c> cast to <see cref="string"/>.
///
/// 📎 We don't need to be exhaustive with our operator overloads like <c>==</c> and <c>!=</c> because of the implicit conversion to <see cref="string"/>.
/// However, we do have to specify <see cref="Equals(string)"/>, <see cref="CompareTo(string)"/>, etc. methods in order to satisfy the <see cref="IEquatable{T}"/> interfaces, etc.
/// </remarks>
public abstract class ValidatedString : IEquatable<string>,
                                        IComparable<string>,
                                        IComparable,
                                        IEquatable<ValidatedString>,
                                        IComparable<ValidatedString> {
    /// <summary>
    /// The wrapped <see cref="string"/>
    /// </summary>
    public readonly string Value;

    protected ValidatedString(string value) => Value = value;

    #region Operators

    public static implicit operator string(ValidatedString           self)                        => self.Value;
    public static                   bool operator ==(ValidatedString self, ValidatedString other) => self.Equals(other);
    public static                   bool operator !=(ValidatedString self, ValidatedString other) => self.Equals(other);

    #endregion

    #region CompareTo

    public int CompareTo(string          other) => string.Compare(Value, other,       StringComparison.Ordinal);
    public int CompareTo(ValidatedString other) => string.Compare(Value, other.Value, StringComparison.Ordinal);
    public int CompareTo(object          obj)   => Value.CompareTo(obj);

    #endregion

    #region Equals

    public          bool Equals(string           other) => Value == other;
    public          bool Equals(ValidatedString? other) => Value.Equals(other?.Value);
    public override bool Equals(object?          obj)   => Value.Equals(obj);

    #endregion

    public override int    GetHashCode() => Value.GetHashCode();
    public override string ToString()    => Value;
}