using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Optional;

public sealed record FailableFunc<TValue> : Failable,
                                            IFailableFunc<TValue>,
                                            IEquatable<TValue> {
    private readonly ValueTuple<TValue>? _value;
    int IReadOnlyCollection<TValue>.     Count         => _value.HasValue ? 1 : 0;
    public bool                          HasValue      => _value.HasValue;
    public TValue                        Value         => _value.HasValue ? _value.Value.Item1 : throw FailableException.FailedException(this, Excuse);
    public object?                       ValueOrExcuse => _value.HasValue ? _value.Value.Item1 : Excuse;

    private FailableFunc(
        ValueTuple<TValue>? value,
        Exception?          excuse,
        string?             description
    ) : base(excuse, default, default, description) {
        _value = value;
    }

    internal FailableFunc(IFailable failableAction, ValueTuple<TValue>? value)
        : base(failableAction) {
        _value = value;
    }

    public static FailableFunc<TValue> Invoke(Func<TValue> failableFunc, [CallerArgumentExpression("failableFunc")] string? expression = default) {
        try {
            return new FailableFunc<TValue>(ValueTuple.Create(failableFunc.Invoke()), default, expression);
        }
        catch (Exception e) {
            return new FailableFunc<TValue>(default, e, expression);
        }
    }

    public IEnumerator<TValue> GetEnumerator()       => (_value is { Item1: { } i /*yes, this is abusive syntax*/ } ? Enumerable.Repeat(i, 1) : Enumerable.Empty<TValue>()).GetEnumerator();
    IEnumerator IEnumerable.   GetEnumerator()       => GetEnumerator();
    public bool                Equals(TValue? other) => _value?.Item1?.Equals(other) == true;

    public override string ToString() {
        return $"{Description} => {this.GetIcon()} [{ValueOrExcuse}]";
    }

    public T2 Handle<T2>(Func<TValue, T2> ifSuccess, Func<Exception, T2> ifFailure) {
        return Passed ? ifSuccess(Value) : ifFailure(Excuse);
    }
}