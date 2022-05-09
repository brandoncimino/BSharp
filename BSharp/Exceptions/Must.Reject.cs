using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace FowlFever.BSharp.Exceptions;

public static partial class Must {
    [Pure]
    public static RejectionException Reject<T>(
        T? actualValue,
        [CallerArgumentExpression("actualValue")]
        string? parameterName = default,
        string? rejectedBy = default,
        string? reason     = default,
        [CallerMemberName]
        string? caller = default,
        Exception? causedBy = default
    ) {
        return new RejectionException(
            actualValue,
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
}