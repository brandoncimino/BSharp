using System.Diagnostics;
using System.Reflection;

using FowlFever.BSharp;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Sugar;

using JetBrains.Annotations;

namespace FowlFever.Clerical.Fluffy;

/// <summary>
/// The core implementation of <see cref="IValidatorMethod"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public record ValidatorMethod<T> : Wrapped<MethodInfo>, IValidatorMethod<T> {
    private readonly Lazy<Action<T?>>     _assertion;
    private readonly Lazy<Func<T?, bool>> _predicate;

    public          ValidatorStyle Style       { get; }
    public          string?        Description { get; }
    public override MethodInfo     Value       { get; }

    #region Constructors

    private ValidatorMethod(
        MethodInfo     method,
        ValidatorStyle style,
        string?        description,
        Action<T?>     assertion,
        Func<T?, bool> predicate
    ) {
        Style       = style;
        Description = description;
        Value       = method;
        _assertion  = Lazily.Get(assertion);
        _predicate  = Lazily.Get(predicate);
    }

    /// <summary>
    /// Constructs an <see cref="IValidatorMethod"/> from an <see cref="ValidatorStyle.Assertion"/>.
    /// </summary>
    /// <param name="assertion">an <see cref="ValidatorStyle.Assertion"/> method</param>
    /// <param name="description">an optional detailed description</param>
    /// <returns>a new <see cref="ValidatorMethod{TIn}"/></returns>
    internal ValidatorMethod(Action<T?> assertion, string? description = default) : this(
        assertion.Method,
        ValidatorStyle.Assertion,
        description,
        assertion,
        assertion.ToPredicate()
    ) { }

    /// <summary>
    /// Constructs a <see cref="ValidatorMethod{T}"/> from a <see cref="ValidatorStyle.Predicate"/>.
    /// </summary>
    /// <param name="predicate">a <see cref="ValidatorStyle.Predicate"/> method</param>
    /// <param name="description">an optional detailed description</param>
    /// <returns>a new <see cref="ValidatorMethod{T}"/></returns>
    internal ValidatorMethod(Func<T?, bool> predicate, string? description = default) : this(
        predicate.Method,
        ValidatorStyle.Predicate,
        description,
        predicate.ToAssertion(),
        predicate
    ) { }

    /// <summary>
    /// Constructs a new <see cref="ValidatorMethod{T}"/> from a <see cref="ValidatorStyle.Checkpoint"/>.
    /// </summary>
    /// <param name="checkpoint"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    internal ValidatorMethod(Func<T?, T?> checkpoint, string? description = default) : this(
        checkpoint.Method,
        ValidatorStyle.Checkpoint,
        description,
        checkpoint.ToAssertion(),
        checkpoint.ToPredicate()
    ) { }

    private static ValidatorMethod<T> From(Delegate delgato, string? description = default) =>
        delgato switch {
            Action<T?> asserter      => new ValidatorMethod<T>(asserter,   description),
            Func<T?, bool> predicate => new ValidatorMethod<T>(predicate,  description),
            Func<T?, T?> checkpoint  => new ValidatorMethod<T>(checkpoint, description),
            _                        => throw new ArgumentException($"{delgato.Prettify()} isn't an appropriate {nameof(ValidatorMethod<T>)}!"),
        };

    public static ValidatorMethod<T> From(MethodInfo method, string? description = default) {
        var delegateType = Validator.CreateDelegate(method);
        return From(delegateType, description.IfBlank(method.Name));
    }

    public static ValidatorMethod<T> From(Annotated<MethodInfo, ValidatorAttribute> annotatedMethod) {
        return From(annotatedMethod.Member, annotatedMethod.Attributes.Single().Description);
    }

    #endregion

    #region Verbs

    [PublicAPI] public void Assert(T? actual) => ((IValidatorMethod)this).Assert(actual);
    [PublicAPI] public bool Test(T?   actual) => ((IValidatorMethod)this).Test(actual);

    [StackTraceHidden]
    [PublicAPI]
    void IValidatorMethod.Assert(object? value) => _assertion.Value((T?)value);

    [StackTraceHidden]
    [PublicAPI]
    bool IValidatorMethod.Test(object? value) => _predicate.Value((T?)value);

    public IFailable           TryValidate(T?      actual) => Failables.Try(Assert, actual, Description.IfBlank(Value.Name));
    IFailable IValidatorMethod.TryValidate(object? value)  => TryValidate((T?)value);

    #endregion

    #region Casts

    public static implicit operator ValidatorMethod<T>(Action<T?>     action)    => new(action);
    public static implicit operator ValidatorMethod<T>(Func<T?, bool> predicate) => new(predicate);
    public static implicit operator ValidatorMethod<T>(Func<T?, T?>   func)      => new(func);

    public static implicit operator Action<T?>(ValidatorMethod<T?>     validator) => validator.Assert;
    public static implicit operator Func<T?, bool>(ValidatorMethod<T?> validator) => validator.Test;

    #endregion

    #region Equality

    protected override IComparer<MethodInfo?>         CanonComparer => MetadataTokenComparer.Instance;
    protected override IEqualityComparer<MethodInfo?> CanonEquality => MetadataTokenComparer.Instance;

    public static bool operator ==(ValidatorMethod<T>? left, IValidatorMethod?   right) => Equals(left, right);
    public static bool operator ==(IValidatorMethod?   left, ValidatorMethod<T>? right) => Equals(left, right);
    public static bool operator !=(IValidatorMethod?   left, ValidatorMethod<T>? right) => !Equals(left, right);
    public static bool operator !=(ValidatorMethod<T>? left, IValidatorMethod?   right) => !Equals(left, right);

    #endregion

    #region Formatting

    public override string Prettify(PrettificationSettings? settings = default) {
        return $"[{GetType().Prettify(settings)}]{Value.Prettify(settings)}";
    }

    #endregion
}