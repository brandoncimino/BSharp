using System;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Optional;
using FowlFever.BSharp.Strings.Prettifiers;
using FowlFever.Implementors;

namespace FowlFever.BSharp.Exceptions;

/// <summary>
/// A fluent way to get to write fewer <see cref="Must"/> methods by writing <see cref="MusterExtensions"/> that are constrained to <typeparamref name="VALIDATED"/>.
/// </summary>
/// <typeparam name="SELF">the <see cref="Type"/> of the <see cref="TrueSelf"/></typeparam>
/// <typeparam name="VALIDATED">the <see cref="Type"/> of the <see cref="ValidationTarget"/></typeparam>
public readonly ref struct Muster<SELF, VALIDATED> {
    public Muster([NotNull] SELF? trueSelf, VALIDATED? validationTarget, string? parameterName, string? rejectedBy) {
        TrueSelf         = Must.NotBeNull(trueSelf, parameterName: parameterName, rejectedBy: rejectedBy);
        ParameterName    = parameterName;
        RejectedBy       = rejectedBy;
        ValidationTarget = Must.NotBeNull(validationTarget, parameterName: parameterName, rejectedBy: rejectedBy);
    }

    public Muster([NotNull] SELF? trueSelf, Func<SELF, VALIDATED> transformation, string? parameterName, string? rejectedBy) : this(
        Must.NotBeNull(trueSelf, parameterName: parameterName, rejectedBy: rejectedBy),
        transformation.Try(trueSelf)
                      .OrElseThrow(
                          exc => Must.Reject(
                              trueSelf,
                              $"An error occurred while retrieving {nameof(Muster<int, int>.ValidationTarget)}!",
                              parameterName,
                              rejectedBy,
                              causedBy: exc
                          )
                      ),
        parameterName,
        rejectedBy
    ) { }

    [NotNull] public SELF      TrueSelf         { get; }
    [NotNull] public VALIDATED ValidationTarget { get; }
    private          string?   ParameterName    { get; }
    private          string?   RejectedBy       { get; }

    internal RejectionException Reject(
        string?    details,
        Exception? causedBy = default,
        [CallerMemberName]
        string? caller = default
    ) {
        throw Must.Reject(ValidationTarget, details, ParameterName, RejectedBy, null, causedBy, caller);
    }

    public T As<T>(string? details = default) {
        return ValidationTarget switch {
            T t       => t,
            IHas<T> t => t.Value,
            _         => throw Must.Reject(ValidationTarget, details, ParameterName, RejectedBy, $"was not an instance of {typeof(T).PrettifyType(default)}"),
        };
    }
}

public static partial class Must {
    public static Muster<T, T> Have<T>(
        T? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? requiredBy = default
    ) => new(
        actualValue,
        actualValue,
        parameterName,
        requiredBy
    );

    public static Muster<SELF, VALIDATED> Have<SELF, VALIDATED>(
        SELF?                 hasActual,
        Func<SELF, VALIDATED> transformation,
        [CallerArgumentExpression("hasActual")]
        string? parameterName = default,
        [CallerMemberName]
        string? requiredBy = default
    ) => new(
        hasActual,
        transformation,
        parameterName,
        requiredBy
    );
}

public static class MusterExtensions {
    /// <summary>
    /// Creates a <see cref="Muster{SELF,VALIDATED}"/> that tests this <typeparamref name="T"/> value.
    /// </summary>
    /// <param name="self">this <typeparamref name="T"/> value</param>
    /// <param name="parameterName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="rejectedBy">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of <paramref name="self"/></typeparam>
    /// <returns>a new <see cref="Muster{SELF,VALIDATED}"/></returns>
    public static Muster<T, T> Must<T>(
        [NotNull] this T? self,
        [CallerArgumentExpression("self")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) => new(
        self,
        self,
        parameterName,
        rejectedBy
    );

    /// <summary>
    /// Creates a <see cref="Muster{SELF,VALIDATED}"/> that tests a <typeparamref name="VALIDATED"/> value derived from this <typeparamref name="SELF"/> value;
    /// </summary>
    /// <param name="self">this <typeparamref name="SELF"/> value</param>
    /// <param name="transformation">extracts the <typeparamref name="VALIDATED"/> value from <paramref name="self"/></param>
    /// <param name="parameterName">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="rejectedBy">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="SELF">the type of <paramref name="self"/></typeparam>
    /// <typeparam name="VALIDATED">the type that validations will be performed on</typeparam>
    /// <returns>a new <see cref="Muster{SELF,VALIDATED}"/></returns>
    public static Muster<SELF, VALIDATED> MustHave<SELF, VALIDATED>(
        [NotNull] this SELF?  self,
        Func<SELF, VALIDATED> transformation,
        [CallerArgumentExpression("self")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) => new(
        self,
        transformation,
        parameterName,
        rejectedBy
    );
}