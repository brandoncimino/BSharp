using System;
using System.Collections;
using System.Collections.Generic;

namespace FowlFever.BSharp.Optional;

public class FailableFunc<TValue> : Failable, IFailableFunc<TValue>, IEquatable<IOptional<TValue>>, IEquatable<TValue> {
    private readonly Optional<TValue> _value;
    public           int              Count         => _value.Count;
    public           bool             HasValue      => _value.HasValue;
    public           TValue           Value         => _value.HasValue ? _value.Value : throw FailableException.FailedException(this, Excuse);
    public           object?          ValueOrExcuse => _value.HasValue ? _value.Value : Excuse;

    private FailableFunc(
        Optional<TValue> value,
        Exception?       excuse
    ) : base(excuse, default, default) {
        _value = value;
    }

    public static FailableFunc<TValue> Invoke(Func<TValue> failableFunc) {
        try {
            return new FailableFunc<TValue>(Optional.Of(failableFunc.Invoke()), default);
        }
        catch (Exception e) {
            return new FailableFunc<TValue>(default, e);
        }
    }

    public IEnumerator<TValue> GetEnumerator() {
        return _value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public bool Equals(IOptional<TValue> other) {
        return Optional.AreEqual(_value, other);
    }

    public bool Equals(TValue other) {
        return Optional.AreEqual(_value, other);
    }
}