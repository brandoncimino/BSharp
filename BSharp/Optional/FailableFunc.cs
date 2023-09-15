using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Optional;

public sealed record FailableFunc<TValue> : Failable, IFailableFunc<TValue>, IEquatable<IOptional<TValue>>, IEquatable<TValue> {
    private readonly Optional<TValue> _value;
    int IReadOnlyCollection<TValue>.  Count         => ((IReadOnlyCollection<TValue>)_value).Count;
    public bool                       HasValue      => _value.HasValue;
    public TValue                     Value         => _value.HasValue ? _value.Value : throw FailableException.FailedException(this, Excuse);
    public object?                    ValueOrExcuse => _value.HasValue ? _value.Value : Excuse;

    private FailableFunc(
        Optional<TValue> value,
        Exception?       excuse,
        string?          description
    ) : base(excuse, default, default, description) {
        _value = value;
    }

    internal FailableFunc(IFailable failableAction, Optional<TValue> value)
        : base(failableAction) {
        _value = value;
    }

    public static FailableFunc<TValue> Invoke(Func<TValue> failableFunc, [CallerArgumentExpression("failableFunc")] string? expression = default) {
        try {
            return new FailableFunc<TValue>(Optional.Of(failableFunc.Invoke()), default, expression);
        }
        catch (Exception e) {
            return new FailableFunc<TValue>(default, e, expression);
        }
    }

    private static readonly OptionalEqualityComparer<TValue> Comparer = new();

    public IEnumerator<TValue> GetEnumerator()                   => _value.GetEnumerator();
    IEnumerator IEnumerable.   GetEnumerator()                   => GetEnumerator();
    public bool                Equals(IOptional<TValue?>? other) => Comparer.Equals(_value, other);
    public bool                Equals(TValue?             other) => Comparer.Equals(_value, other);

    public override string ToString() {
        return $"{Description} => {this.GetIcon()} [{ValueOrExcuse}]";
    }

    /// <inheritdoc cref="IFailable.Passed"/>
    public bool Passed => this.AsFailable().Passed;
    public bool Failed => this.AsFailable().Failed;
}