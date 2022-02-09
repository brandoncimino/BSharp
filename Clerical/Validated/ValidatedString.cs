using System;

namespace FowlFever.Clerical.Validated;

/// <summary>
/// A base class for records that are wrappers around a single <see cref="string"/> value.
/// </summary>
/// <remarks>
/// The goal of this class is that working with a <see cref="ValidatedString"/> should be as close to working with a normal <see cref="string"/>
/// as possible.
/// <p/>
/// To this end, <see cref="ValidatedString"/> provides a simple <see cref="IEquatable{T}"/> implementation
/// and an <c>implicit operator</c> cast to <see cref="string"/>.
/// </remarks>
/// <param name="Value">the wrapped <see cref="string"/></param>
public abstract record ValidatedString(string Value) : IEquatable<string> {
    public bool Equals(string other) => Value == other;

    public static implicit operator string(ValidatedString self) {
        return self.Value;
    }

    public static bool operator ==(ValidatedString a, string b) => a.Equals(b);
    public static bool operator !=(ValidatedString a, string b) => !(a == b);

    public static bool operator ==(string a, ValidatedString b) => a.Equals(b);
    public static bool operator !=(string a, ValidatedString b) => !(a == b);
}