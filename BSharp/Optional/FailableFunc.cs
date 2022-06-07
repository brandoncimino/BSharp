using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Optional;

public record FailableFunc<TValue> : Failable, IFailableFunc<TValue>, IEquatable<IOptional<TValue>>, IEquatable<TValue> {
    private readonly Optional<TValue> _value;
    public           int              Count         => _value.Count;
    public           bool             HasValue      => _value.HasValue;
    public           TValue           Value         => _value.HasValue ? _value.Value : throw FailableException.FailedException(this, Excuse);
    public           object?          ValueOrExcuse => _value.HasValue ? _value.Value : Excuse;

    private FailableFunc(
        Optional<TValue> value,
        Exception?       excuse,
        string?          expression
    ) : base(excuse, default, default, expression) {
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

    public IEnumerator<TValue> GetEnumerator()                 => _value.GetEnumerator();
    IEnumerator IEnumerable.   GetEnumerator()                 => GetEnumerator();
    public bool                Equals(IOptional<TValue> other) => Optional.AreEqual(_value, other);
    public bool                Equals(TValue            other) => Optional.AreEqual(_value, other);

    public override string ToString() {
        return $"{Expression} => {this.GetIcon()} [{ValueOrExcuse}]";
    }
}