using System.Collections.Immutable;

namespace FowlFever.BSharp;

public readonly partial record struct ValueArray<T> :
    IEquatable<ImmutableArray<T>>,
    IEquatable<T>,
    IEquatable<T?>
    where T : struct, IEquatable<T> {
    #region Equality

    public bool Equals(ValueArray<T>     other) => Equals(other.AsSpan());
    public bool Equals(T                 other) => Length == 1 && this[0].Equals(other);
    public bool Equals(T?                other) => other  == null ? Length == 0 : Equals(other.Value);
    public bool Equals(ImmutableArray<T> other) => Equals(other.AsSpan());
    public bool Equals(ReadOnlySpan<T>   other) => AsSpan().SequenceEqual(other);

    #region Operators

    public static bool operator ==(ValueArray<T> a, ImmutableArray<T> b) => a.Equals(b);
    public static bool operator !=(ValueArray<T> a, ImmutableArray<T> b) => !(a == b);

    public static bool operator ==(ValueArray<T> a, T b) => a.Equals(b);
    public static bool operator !=(ValueArray<T> a, T b) => !(a == b);

    public static bool operator ==(T a, ValueArray<T> b) => b.Equals(a);
    public static bool operator !=(T a, ValueArray<T> b) => !(a == b);

    public static bool operator ==(ValueArray<T> a, T? b) => a.Equals(b);
    public static bool operator !=(ValueArray<T> a, T? b) => !(a == b);

    #endregion

    #endregion

    #region Hash Code

    public override int GetHashCode() {
        var hashCode = new HashCode();

        foreach (var it in this) {
            hashCode.Add(it);
        }

        return hashCode.GetHashCode();
    }

    #endregion
}