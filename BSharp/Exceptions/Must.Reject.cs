using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    [Pure]
    public static RejectionException Reject<T>(
        T?      actualValue,
        string? details,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        string?    rejectedBy = default,
        string?    reason     = default,
        Exception? causedBy   = default,
        [CallerMemberName]
        string? caller = default
    ) {
        return new RejectionException(
            actualValue,
            details,
            parameterName,
            rejectedBy,
            reason ?? $"must {caller}",
            causedBy
        );
    }

    [Pure]
    public static RejectionException RejectUnhandledSwitchType<T>(
        T? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        return new RejectionException(
            actualValue,
            parameterName,
            rejectedBy,
            reason: $"Value of type {actualValue?.GetType() ?? typeof(T)} was unhandled by any switch branch!"
        );
    }

    [Pure]
    public static RejectionException RejectWrongType<T>(
        T?   actualValue,
        Type desiredType,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        [CallerMemberName]
        string? rejectedBy = default
    ) {
        return new RejectionException(
            actualValue,
            parameterName,
            rejectedBy,
            $"Value of type {actualValue?.GetType() ?? typeof(T)} was not {desiredType}!"
        );
    }
}