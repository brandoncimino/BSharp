using System.Diagnostics.CodeAnalysis;

using FowlFever.BSharp;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.Implementors;

namespace Ratified;

public enum MustRatify { Yes, No }

public sealed record RatifiedProp<T> : IHas<T>, IEquivalent<T>
    where T : notnull {
    private T                     _value;
    private IRatifier<T>          Ratifier      { get; }
    private IEqualityComparer<T?> ChangeChecker { get; }
    T IEquivalent<T>.             Equivalent    => Value;

    [MemberNotNull(nameof(_value))]
    public T Value {
        get => _value.MustNotBeNull();
        set {
            if (!ChangeChecker.Equals(_value, value)) {
                _value = Ratifier.GetRatified(value);
            }
        }
    }

    [MemberNotNull(nameof(_value))]
    public void Force(T dangerousValue) {
        Set(dangerousValue, MustRatify.No);
    }

    [MemberNotNull(nameof(_value))]
    public void Set(T value, MustRatify mustRatify) {
        switch (mustRatify) {
            case MustRatify.Yes:
                Value = value;
                break;
            case MustRatify.No:
                _value = value;
                break;
            default:
                throw BEnum.UnhandledSwitch(mustRatify);
        }
    }

    public RatifiedProp(IRatifier<T> ratifier, T initialValue, MustRatify ratifyInitialValue = MustRatify.Yes, IEqualityComparer<T?>? changeChecker = default) {
        Ratifier = ratifier;
        Set(initialValue, ratifyInitialValue);
        ChangeChecker = changeChecker ?? EqualityComparer<T?>.Default;
    }
}