using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Enums;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Reflection;
using FowlFever.BSharp.Strings;
using FowlFever.Implementors;

using JetBrains.Annotations;

namespace FowlFever.Clerical.Fluffy;

/// <summary>
/// Contains <see cref="BindingFlags.Static"/> and <see cref="ExtensionAttribute"/> methods to work with <see cref="IValidatorMethod"/>s.
/// </summary>
/// <remarks>
/// TODO: This should have a cuter name! Both because I want one and also to avoid conflicts with <see cref="FluentValidation"/> stuff.
///     Maybe derived from "Approve"?
/// </remarks>
[Experimental(ExperimentalMessage)]
public static class Validator {
    internal const string ExperimentalMessage = "Assumming that Code Generators are viable, they would probably provide a much easier implementation of this idea than the current one, and would DEFINITELY be way more efficient.";

    private static readonly ConcurrentDictionary<Type, Lazy<IValidatorMethod[]>> Cache = new();

    private static IValidatorMethod[] _GetValidatorMethods(Type sourceType) {
        return sourceType.FindAnnotated<MethodInfo, ValidatorAttribute>()
                         .Select(Create)
                         .ToArray();
    }

    /// <summary>
    /// Retrieves all of the <see cref="MethodInfo"/>s annotated with <see cref="ValidatorAttribute"/> on the given <see cref="Type"/> <i>or its <see cref="ReflectionUtils.GetAncestors"/></i>.
    /// </summary>
    /// <param name="sourceType">the <see cref="Type"/> to check for <see cref="ValidatorAttribute"/>s</param>
    /// <returns>all of the retrieved <see cref="IValidatorMethod"/>s</returns>
    /// <remarks>
    /// TODO: construct <see cref="ValidatorMethod{T}"/>s that apply to the is the <see cref="IHas{T}"/>.<see cref="IHas{T}.Value"/>
    /// TODO: define <see cref="ValidatorMethod{T}"/>s using a <see cref="AttributeTargets.Class"/>-level <see cref="ValidatorAttribute"/>
    /// </remarks>
    public static IEnumerable<IValidatorMethod> GetValidatorMethods(Type? sourceType) {
        return sourceType == null ? Enumerable.Empty<IValidatorMethod>() : Cache.GetOrAddLazily(sourceType, _GetValidatorMethods);
    }

    /// <summary>
    /// Invokes all of the <see cref="GetValidatorMethods"/> from the type <typeparamref name="T"/> against <paramref name="actual"/>.
    /// </summary>
    /// <param name="actual">the object being validated</param>
    /// <param name="details">user-provided additional details</param>
    /// <param name="parameterName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="rejectedBy">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of the validated object</typeparam>
    /// <returns>the object (if it passes validation)</returns>
    /// <exception cref="RejectionException">if the object fails validation</exception>
    public static T Validate<T>(
        T       actual,
        string? details = default,
        [CallerArgumentExpression("actual")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        var results = TryValidate(actual).AsList();
        if (results.Any(it => it.Failed)) {
            throw Must.Reject(
                actual,
                details,
                parameterName,
                rejectedBy,
                new RapSheet(results).Prettify()
            );
        }

        return actual;
    }

    /// <summary>
    /// Invokes all of the <see cref="GetValidatorMethods"/> from the type <typeparamref name="T"/> against <paramref name="actual"/>.
    /// <p/>
    /// Returns the results as a collection of <see cref="IFailable"/>s.
    /// </summary>
    /// <param name="actual">the object being validated</param>
    [PublicAPI]
    public static RapSheet TryValidate<T>(T actual) {
        var charges = GetValidatorMethods(actual?.GetType())
            .Select(it => it.TryValidate(actual));
        return new RapSheet(charges);
    }

    /// <summary>
    /// Constructs an <see cref="IValidatorMethod"/> from a <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="method">a <see cref="MethodInfo"/> that might be an <see cref="IValidatorMethod"/></param>
    /// <param name="validatedType">the <see cref="Type"/> being validated. Defaults to the <see cref="MethodInfo"/>'s <see cref="GetValidatedType"/></param>
    /// <param name="description">an optional <see cref="IValidatorMethod.Description"/></param>
    /// <returns>a new <see cref="IValidatorMethod"/></returns>
    [PublicAPI]
    private static IValidatorMethod Create(MethodInfo method, Type? validatedType = default, string? description = default) {
        description   =   description.IfBlank(method.Name);
        validatedType ??= GetValidatedType(method);
        var validatorType = typeof(ValidatorMethod<>).MakeGenericType(validatedType);
        var delgato       = CreateDelegate(method);
        var built         = validatorType.Construct(delgato, description);
        return Must.Be<IValidatorMethod>(built);
    }

    [PublicAPI]
    private static IValidatorMethod Create(Annotated<MethodInfo, ValidatorAttribute> annotatedMethod) {
        return Create(annotatedMethod.Member, description: annotatedMethod.Attributes.Single().Description);
    }

    internal static ValidatorStyle InferValidatorStyle(MethodInfo methodInfo) {
        return BEnum.GetValues<ValidatorStyle>()
                    .Cast<ValidatorStyle?>()
                    .SingleOrDefault(style => style?.AppliesTo(methodInfo) == true)
                    .OrElseThrow(() => new ArgumentException($"{methodInfo.Prettify()} isn't an appropriate {nameof(ValidatorMethod<byte>)}!", (Exception?)default));
    }

    #region Delegate Conversions

    #region From "Assertion" (Action<T>)

    internal static Func<T, bool> ToPredicate<T>(this Action<T> action) {
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

    internal static Action<T> ToAssertion<T>(this Func<T, T> checkpoint) => obj => checkpoint.Invoke(obj);

    internal static Func<T, bool> ToPredicate<T>(this Func<T, T> checkpoint) {
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

    [StackTraceHidden]
    internal static Action<T> ToAssertion<T>(this Func<T, bool> predicate) {
        return it => {
            var result = predicate(it);
            if (!result) {
                throw new ArgumentException($"{predicate.Prettify()} returned {result}!");
            }
        };
    }

    [StackTraceHidden]
    internal static Func<T, T> ToCheckpoint<T>(this Func<T, bool> predicate) {
        return it => {
            predicate.ToCheckpoint().Invoke(it);
            return it;
        };
    }

    #endregion

    #endregion

    [StackTraceHidden]
    internal static Type GetValidatedType(MethodInfo method) {
        static Type? FromParameters(MethodInfo methodInfo) {
            return methodInfo.FindSingleParameter()?.ParameterType;
        }

        static Type FromDeclaringType(MemberInfo memberInfo) {
            return memberInfo.MustGetDeclaringType();
        }

        return FromParameters(method)
               ?? FromDeclaringType(method);
    }

    [StackTraceHidden]
    internal static Delegate CreateDelegate(MethodInfo method) {
        var style         = InferValidatorStyle(method);
        var validatedType = GetValidatedType(method);
        var delegateType  = style.GetDelegateType(validatedType);
        try {
            var delgato = Delegate.CreateDelegate(delegateType, method);
            return delgato;
        }
        catch (Exception e) {
            throw new ArgumentException($"Unable to create [{delegateType.Prettify()}] from the {method.Prettify()}\n\t{nameof(style)}: {style}\n\t{nameof(validatedType)}: {validatedType}", e);
        }
    }

    internal static bool AppliesTo(this ValidatorStyle style, MethodInfo method) {
        return style switch {
            ValidatorStyle.Predicate  => method.IsPredicate(),
            ValidatorStyle.Assertion  => method.IsVoid(),
            ValidatorStyle.Checkpoint => method.IsCheckpoint(),
            _                         => throw BEnum.UnhandledSwitch(style),
        };
    }

    private static Type GetDelegateType(this ValidatorStyle style, Type validatedType) {
        return style switch {
            ValidatorStyle.Assertion  => typeof(Action<>).MakeGenericType(validatedType),
            ValidatorStyle.Checkpoint => typeof(Func<,>).MakeGenericType(validatedType, validatedType),
            ValidatorStyle.Predicate  => typeof(Func<,>).MakeGenericType(validatedType, typeof(bool)),
            _                         => throw BEnum.UnhandledSwitch(style),
        };
    }
}