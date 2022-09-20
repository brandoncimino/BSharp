using System;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

public static partial class Reject {
    #region Parameter

    /// <summary>
    /// Creates a basic <see cref="RejectionException"/>.
    /// </summary>
    /// <param name="actualValue">the parameter that was rejected</param>
    /// <param name="details">a description of <i>why</i> <paramref name="actualValue"/> was rejected</param>
    /// <param name="innerException">the <see cref="Exception"/> (if any) that the <paramref name="actualValue"/> caused</param>
    /// <param name="_actualValue">see <see cref="CallerArgumentExpressionAttribute"/></param>
    /// <param name="_caller">see <see cref="CallerMemberNameAttribute"/></param>
    /// <typeparam name="T">the type of the <paramref name="actualValue"/></typeparam>
    /// <returns>a new <see cref="RejectionException"/></returns>
    [Pure]
    public static RejectionException Parameter<T>(
        T          actualValue,
        string?    details        = default,
        Exception? innerException = default,
        [CallerArgumentExpression("actualValue")]
        string? _actualValue = default,
        [CallerMemberName] string? _caller = default
    ) {
        return new RejectionException(
            actualValue,
            details,
            _actualValue,
            _caller,
            innerException: innerException
        );
    }

    /// <inheritdoc cref="Parameter{T}(T,string?,System.Exception?,string?,string?)"/>
    [Pure]
    public static RejectionException Parameter<T>(
        ReadOnlySpan<T> actualValue,
        string?         details        = default,
        Exception?      innerException = default,
        [CallerArgumentExpression("actualValue")]
        string? _actualValue = default,
        [CallerMemberName] string? _caller = default
    ) {
        var array = actualValue.ToArray();
        return new RejectionException(
            array,
            details,
            _actualValue,
            _caller,
            innerException: innerException
        ) {
            ActualValueString = $"{nameof(ReadOnlySpan<T>)}<{typeof(T).Name}> [{string.Join(", ", array)}]"
        };
    }

    /// <inheritdoc cref="Parameter{T}(T,string?,System.Exception?,string?,string?)"/>
    [Pure]
    public static RejectionException Parameter<T>(
        Span<T>    actualValue,
        string?    details        = default,
        Exception? innerException = default,
        [CallerArgumentExpression("actualValue")]
        string? _actualValue = default,
        [CallerMemberName] string? _caller = default
    ) {
        var array = actualValue.ToArray();
        return new RejectionException(
            array,
            details,
            _actualValue,
            _caller,
            innerException: innerException
        ) {
            ActualValueString = $"{nameof(Span<T>)}<{typeof(T).Name}> [{string.Join(", ", array)}]"
        };
    }

    #endregion
}