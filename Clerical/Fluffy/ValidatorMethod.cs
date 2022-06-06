using System.Reflection;

using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Sugar;

using JetBrains.Annotations;

namespace FowlFever.Clerical.Fluffy;

/// <summary>
/// The core implementation of <see cref="IValidatorMethod"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ValidatorMethod<T> : IValidatorMethod, IPrettifiable, IEquatable<IValidatorMethod>, IEquatable<MethodInfo> {
    private readonly Lazy<Action<T?>>     _assertion;
    private readonly Lazy<Func<T?, bool>> _predicate;
    private readonly Lazy<Func<T?, T?>>   _checkpoint;

    public ValidatorStyle Style  { get; }
    public MethodInfo     Method { get; }

    #region Constructors

    private ValidatorMethod(
        MethodInfo     method,
        ValidatorStyle style,
        Action<T?>     assertion,
        Func<T?, bool> predicate,
        Func<T?, T?>   checkpoint
    ) {
        Style       = style;
        Method      = method;
        _assertion  = Lazily.Get(assertion);
        _predicate  = Lazily.Get(predicate);
        _checkpoint = Lazily.Get(checkpoint);
    }

    /// <summary>
    /// Constructs an <see cref="IValidatorMethod"/> from an <see cref="ValidatorStyle.Assertion"/>.
    /// </summary>
    /// <param name="assertion">an <see cref="ValidatorStyle.Assertion"/> method</param>
    /// <returns>a new <see cref="ValidatorMethod{TIn}"/></returns>
    internal ValidatorMethod(Action<T?> assertion) : this(
        assertion.Method,
        ValidatorStyle.Assertion,
        assertion,
        assertion.ToPredicate(),
        assertion.ToCheckpoint()
    ) { }

    /// <summary>
    /// Constructs a <see cref="ValidatorMethod{T}"/> from a <see cref="ValidatorStyle.Predicate"/>.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    internal ValidatorMethod(Func<T?, bool> predicate) : this(
        predicate.Method,
        ValidatorStyle.Predicate,
        predicate.ToAssertion(),
        predicate,
        predicate.ToCheckpoint()
    ) { }

    /// <summary>
    /// Constructs a new <see cref="ValidatorMethod{T}"/> from a <see cref="ValidatorStyle.Checkpoint"/>.
    /// </summary>
    /// <param name="checkpoint"></param>
    /// <returns></returns>
    internal ValidatorMethod(Func<T?, T?> checkpoint) : this(
        checkpoint.Method,
        ValidatorStyle.Checkpoint,
        checkpoint.ToAssertion(),
        checkpoint.ToPredicate(),
        checkpoint
    ) { }

    private static ValidatorMethod<T> From(Delegate delgato) =>
        delgato switch {
            Action<T?> asserter      => new ValidatorMethod<T>(asserter),
            Func<T?, bool> predicate => new ValidatorMethod<T>(predicate),
            Func<T?, T?> checkpoint  => new ValidatorMethod<T>(checkpoint),
            _                        => throw new ArgumentException($"{delgato.Prettify()} isn't an appropriate {nameof(ValidatorMethod<T>)}!"),
        };

    public static ValidatorMethod<T> From(MethodInfo method) {
        var delegateType = Validator.CreateDelegate(method);
        return From(delegateType);
    }

    #endregion

    #region Verbs

    [PublicAPI] public void Assert(T actual) => Assert((object?)actual);
    [PublicAPI] public bool Try(T    actual) => Try((object?)actual);
    [PublicAPI] public T    Check(T  actual) => (T)Check((object?)actual)!;

    [PublicAPI] public void    Assert(object? value) => _assertion.Value((T?)value);
    [PublicAPI] public bool    Try(object?    value) => _predicate.Value((T?)value);
    [PublicAPI] public object? Check(object?  value) => _checkpoint.Value((T?)value);

    #endregion

    #region Casts

    public static implicit operator ValidatorMethod<T>(Action<T?>     action)    => new(action);
    public static implicit operator ValidatorMethod<T>(Func<T?, bool> predicate) => new(predicate);
    public static implicit operator ValidatorMethod<T>(Func<T?, T?>   func)      => new(func);

    public static implicit operator Action<T?>(ValidatorMethod<T?>     validator) => validator.Assert;
    public static implicit operator Func<T?, bool>(ValidatorMethod<T?> validator) => validator.Try;
    public static implicit operator Func<T?, T?>(ValidatorMethod<T?>   validator) => validator.Check;

    #endregion

    #region Equality

    private readonly IEqualityComparer<MethodInfo?> MethodComparer = new MetadataTokenComparer();

    public          bool Equals(IValidatorMethod? other) => MethodComparer.Equals(Method, other?.Method);
    public          bool Equals(MethodInfo        other) => MethodComparer.Equals(Method, other);
    public override bool Equals(object?           obj)   => MethodComparer.Equals(Method, obj as MethodInfo);
    public override int  GetHashCode()                   => MethodComparer.GetHashCode();

    public static bool operator ==(ValidatorMethod<T>? left, IValidatorMethod?   right) => Equals(left, right);
    public static bool operator ==(IValidatorMethod?   left, ValidatorMethod<T>? right) => Equals(left, right);
    public static bool operator !=(IValidatorMethod?   left, ValidatorMethod<T>? right) => !Equals(left, right);
    public static bool operator !=(ValidatorMethod<T>? left, IValidatorMethod?   right) => !Equals(left, right);

    #endregion

    #region Formatting

    public string Prettify(PrettificationSettings? settings = default) {
        return $"[{GetType().Prettify(settings)}]{Method.Prettify(settings)}";
    }

    public override string ToString() {
        var staticString = Method.IsStatic ? "static" : "instance";
        return $"[{nameof(ValidatorMethod<T>)}<{typeof(T).Name}> from {staticString} {Method.DeclaringType?.Name}.{Method.Name}]";
    }

    #endregion
}