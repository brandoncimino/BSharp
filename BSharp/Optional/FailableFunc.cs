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

    /// <summary>
    /// I don't <i>think</i> I want to override <see cref="object.GetHashCode"/> for this...but I dunno.
    /// I should probably read stuff like <a href="https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/336aedhh(v=vs.100)">implementing the Equals method</a>
    /// in more detail.
    ///
    /// Basically...the word <c>pragma</c> scares me.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(object other) {
        return other switch {
            TValue tv             => Equals(tv),
            IOptional<TValue> opt => Equals(opt),
            _                     => base.Equals(other) // this complains about reference equality...but why?
        };
    }

    public override string ToString() {
        return $"{base.ToString()} => {ValueOrExcuse}";
    }
}