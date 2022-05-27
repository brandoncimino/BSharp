using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Sugar;

namespace FowlFever.Clerical.Fluffy;

public class ValidatorMethod<T> {
    private readonly Lazy<Action<T>>     _asserter;
    private readonly Lazy<Func<T, bool>> _predicate;
    private readonly Lazy<Func<T, T>>    _checkpoint;
    public           Action<T>           Asserter   => _asserter.Value;
    public           Func<T, bool>       Predicate  => _predicate.Value;
    public           Func<T, T>          Checkpoint => _checkpoint.Value;

    #region Constructors

    private ValidatorMethod(Lazy<Action<T>> asserter, Lazy<Func<T, bool>> predicate, Lazy<Func<T, T>> checkpoint) {
        _asserter   = asserter;
        _predicate  = predicate;
        _checkpoint = checkpoint;
    }

    private ValidatorMethod(Action<T> asserter) : this(
        Lazily.Get(asserter),
        Lazily.Get(asserter.ToPredicate),
        Lazily.Get(asserter.ToCheckpoint)
    ) { }

    private ValidatorMethod(Func<T, bool> predicate) : this(
        Lazily.Get(predicate.ToAssertion),
        Lazily.Get(predicate),
        Lazily.Get(predicate.ToCheckpoint)
    ) { }

    private ValidatorMethod(Func<T, T> checkpoint) : this(
        Lazily.Get(checkpoint.ToAssertion),
        Lazily.Get(checkpoint.ToPredicate),
        Lazily.Get(checkpoint)
    ) { }

    public static ValidatorMethod<T> From(Delegate delgato) {
        return delgato switch {
            Action<T> asserter      => new ValidatorMethod<T>(asserter),
            Func<T, bool> predicate => new ValidatorMethod<T>(predicate),
            Func<T, T> checkpoint   => new ValidatorMethod<T>(checkpoint),
            _                       => throw new ArgumentException($"{delgato.Prettify()} isn't an appropriate {nameof(ValidatorMethod<T>)}!"),
        };
    }

    public static ValidatorMethod<T> From(MethodInfo method) {
        if (method.IsPredicate()) {
            return From(Delegate.CreateDelegate(typeof(Func<T, bool>), method));
        }

        if (method.IsCheckpoint()) {
            return From(Delegate.CreateDelegate(typeof(Func<T, T>), method));
        }

        if (method.IsVoid() && method.GetSingleParameter()?.ParameterType == method) {
            return From(Delegate.CreateDelegate(typeof(Action<T>), method));
        }

        throw new ArgumentException($"{method.Prettify()} isn't an appropriate {nameof(ValidatorMethod<T>)}!");
    }

    #endregion

    #region Verbs

    public void Assert(T actual) => Asserter(actual);
    public bool Try(T    actual) => Predicate(actual);
    public T    Check(T  actual) => Checkpoint(actual);

    #endregion

    #region Casts

    public static implicit operator ValidatorMethod<T>(Action<T>     action)    => new(action);
    public static implicit operator ValidatorMethod<T>(Func<T, bool> predicate) => new(predicate);
    public static implicit operator ValidatorMethod<T>(Func<T, T>    func)      => new(func);

    public static implicit operator Action<T>(ValidatorMethod<T>     validator) => validator.Asserter;
    public static implicit operator Func<T, bool>(ValidatorMethod<T> validator) => validator.Predicate;
    public static implicit operator Func<T, T>(ValidatorMethod<T>    validator) => validator.Checkpoint;

    #endregion
}

public static class Validator {
    private static readonly ConcurrentDictionary<Type, Lazy<object[]>> Cache = new();

    private static object[] _GetValidatorMethods<T>(Type type) {
        return typeof(T)
               .FindMembersWithAttribute<MethodInfo, ValidatorAttribute>()
               .Select(ValidatorMethod<T>.From)
               .Cast<object>()
               .ToArray();
    }

    public static IEnumerable<ValidatorMethod<T>> GetValidatorMethods<T>() {
        return Cache.GetOrAddLazily(typeof(T), _GetValidatorMethods<T>)
                    .Cast<ValidatorMethod<T>>();
    }

    /// <summary>
    /// Invokes all of the <see cref="GetValidatorMethods{T}"/> from the type <typeparamref name="T"/> against <paramref name="actual"/>.
    /// </summary>
    /// <param name="actual">the object being validated</param>
    /// <param name="parameterName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="rejectedBy">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of the validated object</typeparam>
    /// <returns>the object (if it passes validation)</returns>
    /// <exception cref="RejectionException">if the object fails validation</exception>
    public static T Validate<T>(
        T actual,
        [CallerArgumentExpression("actual")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        var results = TryValidate(actual).AsList();
        if (results.Any(it => it.Failed)) {
            throw Must.Reject(
                actual,
                parameterName,
                rejectedBy,
                results.JoinLines(indent: "  ")
            );
        }

        return actual;
    }

    /// <summary>
    /// Invokes all of the <see cref="GetValidatorMethods{T}"/> from the type <typeparamref name="T"/> against <paramref name="actual"/>.
    ///
    /// Returns the results as a collection of <see cref="IFailable"/>s.
    /// </summary>
    /// <param name="actual">the object being validated</param>
    public static IEnumerable<IFailable> TryValidate<T>(T actual) {
        return GetValidatorMethods<T>().Select(it => it.Asserter.Try(actual)).ToList();
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class ValidatorAttribute : BrandonAttribute {
    private ValidationStyle Style { get; }

    public ValidatorAttribute(ValidationStyle style = ValidationStyle.Inferred) {
        Style = style;
    }

    protected override void ValidateTarget_Hook(MemberInfo target) {
        var method = target.MustBe<MethodInfo>();
        if (Style == ValidationStyle.Inferred) {
            if (InferValidationStyle(method) == null) {
                throw this.RejectInvalidTarget(target, $"Couldn't infer the {nameof(ValidationStyle)}");
            }
        }
        else {
            if (Style.AppliesTo(method) == false) {
                throw this.RejectInvalidTarget(target, $"{target} didn't match the {nameof(ValidationStyle)}.{Style}");
            }
        }
    }

    private static ValidationStyle? InferValidationStyle(MethodInfo methodInfo) {
        return BEnum.GetValues<ValidationStyle>().SingleOrDefault(style => style.AppliesTo(methodInfo));
    }
}

public enum ValidationStyle {
    Inferred,
    /// <summary>
    /// A method with 1 input that returns a <see cref="bool"/>.
    /// </summary>
    Predicate,
    /// <summary>
    /// A method with 1 input and no return value that might throw an <see cref="Exception"/>.
    /// </summary>
    Assertion,
    /// <summary>
    /// A method with 1 input that returns the same object it took in.
    /// </summary>
    Checkpoint,
}

public static class ValidationStyleExtensions {
    public static bool AppliesTo(this ValidationStyle style, MethodInfo method) {
        return style switch {
            ValidationStyle.Predicate  => method.IsPredicate(),
            ValidationStyle.Assertion  => method.IsVoid(),
            ValidationStyle.Checkpoint => method.GetParameters().SingleOrDefault()?.ParameterType == method.ReturnType,
            ValidationStyle.Inferred   => InferValidationStyle(method)                            != null,
            _                          => throw BEnum.UnhandledSwitch(style),
        };
    }

    public static ValidationStyle? InferValidationStyle(MethodInfo methodInfo) {
        return BEnum.GetValues<ValidationStyle>()
                    .Where(it => it != ValidationStyle.Inferred)
                    .SingleOrDefault(style => style.AppliesTo(methodInfo));
    }

    #region From "Assertion" (Action<T>)

    public static Func<T, T> ToCheckpoint<T>(this Action<T> action) {
        return it => {
            action(it);
            return it;
        };
    }

    public static Func<T, bool> ToPredicate<T>(this Action<T> action) {
        return it => {
            try {
                action(it);
                return true;
            }
            catch {
                return false;
            }
        };
    }

    #endregion

    #region From "Checkpoint" (Func<T,T>)

    public static Action<T> ToAssertion<T>(this Func<T, T> checkpoint) => obj => checkpoint.Invoke(obj);

    public static Func<T, bool> ToPredicate<T>(this Func<T, T> checkpoint) {
        return it => {
            try {
                checkpoint(it);
                return true;
            }
            catch {
                return false;
            }
        };
    }

    #endregion

    #region From "Predicate" (Func<T, bool>)

    public static Action<T> ToAssertion<T>(this Func<T, bool> predicate) {
        return it => {
            var result = predicate(it);
            if (!result) {
                throw new ArgumentException($"{predicate.Prettify()} returned {result}!");
            }
        };
    }

    public static Func<T, T> ToCheckpoint<T>(this Func<T, bool> predicate) {
        return it => {
            predicate.ToCheckpoint().Invoke(it);
            return it;
        };
    }

    #endregion
}