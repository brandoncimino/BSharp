using System.Diagnostics;
using System.Reflection;

using FowlFever.BSharp;
using FowlFever.BSharp.Exceptions;
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

    [PublicAPI] public void Assert(T?              actual) => _assertion.Value(actual);
    [PublicAPI] public void Assert(IValidated<T?>? actual) => _assertion.Value(actual == null ? default : actual.ValidationTarget);
    [PublicAPI] public bool Test(T?                actual) => _predicate.Value(actual);
    [PublicAPI] public bool Test(IValidated<T?>?   actual) => _predicate.Value(actual == null ? default : actual.ValidationTarget);

    [StackTraceHidden]
    [PublicAPI]
    void IValidatorMethod.Assert(object? value) {
        if (value is IValidated<T> validated) {
            Assert(validated);
        }
        else {
            Assert((T?)value);
        }
    }

    [StackTraceHidden]
    [PublicAPI]
    bool IValidatorMethod.Test(object? value) => value switch {
        IValidated<T> validated => Test(validated),
        _                       => Test((T?)value),
    };

    public IFailable TryValidate(T?              actual) => Failables.Try(Assert, actual, Description.IfBlank(Value.Name));
    public IFailable TryValidate(IValidated<T?>? actual) => Failables.Try(Assert, actual == null ? default : actual.ValidationTarget);

    IFailable IValidatorMethod.TryValidate(object? value) =>
        value switch {
            T t             => TryValidate(t),
            IValidated<T> t => TryValidate(t),
            _               => throw Must.RejectUnhandledSwitchType(value),
        };

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